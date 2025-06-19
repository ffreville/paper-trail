using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestionnaire des règles globales de bureaucratie
/// La logique spécifique aux documents est maintenant dans DynamicConfigurationManager
/// </summary>
public class BureaucracySystem : MonoBehaviour
{
    [Header("Global Bureaucracy Rules")]
    public int maxDocumentsBeforeAbandonment = 8;
    public float citizenPatienceTime = 120f; // 2 minutes in real time
    public float systemOverloadThreshold = 15f; // Nombre de documents simultanés

    [Header("System Effects")]
    public bool enableSystemOverload = true;
    public bool enableCitizenAbandonment = true;

    [Header("References")]
    public DocumentManager documentManager;

    private DynamicConfigurationManager configManager;
    private Dictionary<string, CitizenRequest> activeCitizenRequests = new Dictionary<string, CitizenRequest>();
    private Dictionary<string, float> citizenPatienceTimers = new Dictionary<string, float>();

    private void Start()
    {
        // Trouve les références automatiquement
        if (documentManager == null)
            documentManager = FindObjectOfType<DocumentManager>();

        configManager = FindObjectOfType<DynamicConfigurationManager>();

        // Subscribe aux événements du système dynamique
        if (configManager != null)
        {
            configManager.OnDynamicDocumentGenerated += OnDocumentGenerated;
            configManager.OnTriggerActivated += OnTriggerActivated;
        }

        if (documentManager != null)
        {
            documentManager.OnDocumentProcessed += OnDocumentProcessed;
        }

        Debug.Log("BureaucracySystem initialized - Global rules only");
    }

    private void Update()
    {
        if (enableCitizenAbandonment)
        {
            UpdateCitizenPatience();
        }

        if (enableSystemOverload)
        {
            CheckSystemOverload();
        }
    }

    private void OnDocumentGenerated(DocumentData document)
    {
        // Démarre le timer de patience pour ce citoyen
        StartCitizenPatienceTimer(document.citizenName);

        Debug.Log($"Global Bureaucracy: Document generated for {document.citizenName}");
    }

    private void OnTriggerActivated(BureaucracyTrigger trigger)
    {
        // Effets globaux des triggers (pas de logique spécifique)
        Debug.Log($"Global Bureaucracy: Trigger activated - {trigger.triggerName}");

        // Peut réduire la patience globale quand les cascades s'activent
        if (trigger.canTriggerRecursively)
        {
            ReduceGlobalPatience(0.1f);
        }
    }

    private void OnDocumentProcessed(DocumentData document)
    {
        // Arrête le timer de patience pour ce citoyen
        StopCitizenPatienceTimer(document.citizenName);

        Debug.Log($"Global Bureaucracy: Document processed for {document.citizenName}");
    }

    private void StartCitizenPatienceTimer(string citizenName)
    {
        if (!citizenPatienceTimers.ContainsKey(citizenName))
        {
            citizenPatienceTimers[citizenName] = citizenPatienceTime;
        }
    }

    private void StopCitizenPatienceTimer(string citizenName)
    {
        if (citizenPatienceTimers.ContainsKey(citizenName))
        {
            citizenPatienceTimers.Remove(citizenName);
        }
    }

    private void UpdateCitizenPatience()
    {
        List<string> citizensToRemove = new List<string>();

        foreach (var kvp in citizenPatienceTimers)
        {
            string citizenName = kvp.Key;
            float timeRemaining = kvp.Value;

            timeRemaining -= Time.deltaTime;
            citizenPatienceTimers[citizenName] = timeRemaining;

            if (timeRemaining <= 0)
            {
                // Citoyen abandonné !
                OnCitizenAbandoned(citizenName);
                citizensToRemove.Add(citizenName);
            }
        }

        // Supprime les citoyens qui ont abandonné
        foreach (string citizenName in citizensToRemove)
        {
            citizenPatienceTimers.Remove(citizenName);
        }
    }

    private void OnCitizenAbandoned(string citizenName)
    {
        Debug.Log($"🚫 Citizen abandoned: {citizenName} (patience expired)");

        // Incrémente le compteur d'abandons
        if (GameManager.Instance != null)
        {
            GameManager.Instance.citizensAbandoned++;
        }

        // Effet narratif
        string abandonMessage = $"💔 {citizenName} has given up and left the office!\n";
        abandonMessage += "The bureaucratic maze has claimed another victim...";

        configManager?.OnNarrativeUpdate?.Invoke(abandonMessage);
    }

    private void CheckSystemOverload()
    {
        if (documentManager == null) return;

        int currentDocuments = documentManager.GetInboxCount();

        if (currentDocuments >= systemOverloadThreshold)
        {
            OnSystemOverloaded();
        }
    }

    private void OnSystemOverloaded()
    {
        Debug.Log("🔥 SYSTEM OVERLOAD! Too many documents!");

        // Réduit la patience de tous les citoyens
        ReduceGlobalPatience(0.5f);

        // Message d'alerte
        string overloadMessage = "🚨 SYSTEM OVERLOAD! 🚨\n";
        overloadMessage += "The bureaucratic machine is grinding to a halt!\n";
        overloadMessage += "Citizens are getting increasingly impatient...";

        configManager?.OnNarrativeUpdate?.Invoke(overloadMessage);
    }

    private void ReduceGlobalPatience(float reduction)
    {
        List<string> citizens = new List<string>(citizenPatienceTimers.Keys);

        foreach (string citizen in citizens)
        {
            citizenPatienceTimers[citizen] -= reduction * citizenPatienceTime;
            citizenPatienceTimers[citizen] = Mathf.Max(0, citizenPatienceTimers[citizen]);
        }
    }

    // Méthodes utilitaires publiques
    public int GetActiveCitizensCount()
    {
        return citizenPatienceTimers.Count;
    }

    public float GetAveragePatienceLevel()
    {
        if (citizenPatienceTimers.Count == 0) return 1f;

        float totalPatience = 0f;
        foreach (var patience in citizenPatienceTimers.Values)
        {
            totalPatience += patience / citizenPatienceTime; // Normalise entre 0-1
        }

        return totalPatience / citizenPatienceTimers.Count;
    }

    public bool IsSystemOverloaded()
    {
        return documentManager != null && documentManager.GetInboxCount() >= systemOverloadThreshold;
    }

    // Méthodes pour ajuster les règles en runtime
    public void SetPatienceTime(float newTime)
    {
        citizenPatienceTime = newTime;
        Debug.Log($"Citizen patience time set to {newTime} seconds");
    }

    public void SetOverloadThreshold(int newThreshold)
    {
        systemOverloadThreshold = newThreshold;
        Debug.Log($"System overload threshold set to {newThreshold} documents");
    }

    // Debug info
    private void OnGUI()
    {
        if (Application.isEditor && citizenPatienceTimers.Count > 0)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Active Citizens: {GetActiveCitizensCount()}");
            GUILayout.Label($"Average Patience: {GetAveragePatienceLevel():P0}");
            GUILayout.Label($"System Overloaded: {IsSystemOverloaded()}");
            GUILayout.EndArea();
        }
    }
}