using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DocumentManager : MonoBehaviour
{
    [Header("Document Storage")]
    public List<DocumentData> allDocuments = new List<DocumentData>();
    public List<DocumentData> inboxDocuments = new List<DocumentData>();
    public List<DocumentData> processedDocuments = new List<DocumentData>();

    [Header("Events")]
    public System.Action<DocumentData> OnDocumentAdded;
    public System.Action<DocumentData> OnDocumentProcessed;
    public System.Action<DocumentData> OnDocumentRejected;

    // Nouvelle référence au système dynamique
    private DynamicConfigurationManager configurationManager;

    private void Start()
    {
        // Trouve le DynamicConfigurationManager automatiquement
        configurationManager = FindObjectOfType<DynamicConfigurationManager>();

        if (configurationManager == null)
        {
            Debug.LogWarning("No DynamicConfigurationManager found! Some features may not work.");
        }
    }

    public void AddDocument(DocumentData document)
    {
        allDocuments.Add(document);
        inboxDocuments.Add(document);

        Debug.Log($"New document added: {document.documentTitle}");
        OnDocumentAdded?.Invoke(document);
    }

    public void ProcessDocument(DocumentData document)
    {
        if (!inboxDocuments.Contains(document))
        {
            Debug.LogWarning("Trying to process document not in inbox!");
            return;
        }

        document.status = DocumentStatus.InProgress;
        document.lastModified = System.DateTime.Now;

        // ✅ NOUVELLE LOGIQUE : Utilise DynamicConfigurationManager
        if (configurationManager != null)
        {
            // Le DynamicConfigurationManager gère automatiquement les triggers
            // lors de la génération de documents, pas besoin d'appel explicite ici
            Debug.Log($"Document processing handled by DynamicConfigurationManager: {document.documentTitle}");
        }
        else
        {
            // ❌ ANCIENNE LOGIQUE : BureaucracySystem (maintenant supprimée)
            // BureaucracySystem bureaucracySystem = FindObjectOfType<BureaucracySystem>();
            // if (bureaucracySystem != null)
            // {
            //     bureaucracySystem.ProcessDocument(document);
            // }

            Debug.LogWarning("No DynamicConfigurationManager found - document processed without triggers");
        }

        // Move to processed
        inboxDocuments.Remove(document);
        processedDocuments.Add(document);

        // Finalise le document
        document.status = DocumentStatus.Completed;

        // Mise à jour des scores
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DocumentProcessed();
        }

        // Événement pour les autres systèmes
        OnDocumentProcessed?.Invoke(document);

        Debug.Log($"Document processed: {document.documentTitle}");
    }

    public void RejectDocument(DocumentData document, string reason)
    {
        if (!inboxDocuments.Contains(document))
        {
            Debug.LogWarning("Trying to reject document not in inbox!");
            return;
        }

        document.status = DocumentStatus.Rejected;
        document.lastModified = System.DateTime.Now;

        // Ajoute la raison du rejet dans les détails
        document.requestDetails += $"\n\n❌ REJECTED: {reason}";

        inboxDocuments.Remove(document);
        processedDocuments.Add(document);

        // Pénalité pour rejet
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncrementBureaucracyScore(-5); // Pénalité
        }

        OnDocumentRejected?.Invoke(document);
        Debug.Log($"Document rejected: {document.documentTitle} - Reason: {reason}");
    }

    // Nouvelles méthodes pour la compatibilité avec le système dynamique
    public void ProcessDocumentWithTemplate(DocumentData document)
    {
        if (configurationManager == null)
        {
            // Fallback vers la méthode normale
            ProcessDocument(document);
            return;
        }

        // Trouve le template pour ce type de document
        var template = configurationManager.GetTemplate(document.documentType);
        if (template != null)
        {
            Debug.Log($"Processing document with template: {template.documentTitle}");

            // Les triggers du template sont automatiquement gérés
            // par le DynamicConfigurationManager lors de OnDocumentProcessed
        }

        // Traitement normal
        ProcessDocument(document);
    }

    public List<DocumentData> GetDocumentsByType(DocumentType type)
    {
        return allDocuments.Where(doc => doc.documentType == type).ToList();
    }

    public List<DocumentData> GetDocumentsByStatus(DocumentStatus status)
    {
        return allDocuments.Where(doc => doc.status == status).ToList();
    }

    public DocumentData GetDocumentById(string id)
    {
        return allDocuments.FirstOrDefault(doc => doc.documentId == id);
    }

    public int GetInboxCount()
    {
        return inboxDocuments.Count;
    }

    public int GetProcessedCount()
    {
        return processedDocuments.Count;
    }

    public int GetTotalDocuments()
    {
        return allDocuments.Count;
    }

    // Méthodes utilitaires pour les statistiques
    public Dictionary<DocumentType, int> GetDocumentCountByType()
    {
        var counts = new Dictionary<DocumentType, int>();

        foreach (var doc in allDocuments)
        {
            if (counts.ContainsKey(doc.documentType))
                counts[doc.documentType]++;
            else
                counts[doc.documentType] = 1;
        }

        return counts;
    }

    public float GetProcessingEfficiency()
    {
        if (allDocuments.Count == 0) return 0f;

        int completedDocs = processedDocuments.Count(d => d.status == DocumentStatus.Completed);
        return (float)completedDocs / allDocuments.Count;
    }

    // Debug info
    public string GetManagerStatus()
    {
        string status = $"DocumentManager Status:\n";
        status += $"Total Documents: {GetTotalDocuments()}\n";
        status += $"In Inbox: {GetInboxCount()}\n";
        status += $"Processed: {GetProcessedCount()}\n";
        status += $"Efficiency: {GetProcessingEfficiency():P1}\n";
        status += $"ConfigManager: {(configurationManager != null ? "✅" : "❌")}";

        return status;
    }
}