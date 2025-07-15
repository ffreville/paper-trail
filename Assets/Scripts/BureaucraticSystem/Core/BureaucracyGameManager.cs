using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BureaucracyGameManager : MonoBehaviour
{
    [Header("Game State")]
    public BureaucraticScenario currentScenario;
    public List<BureaucraticForm> activeForms = new List<BureaucraticForm>();
    public List<BureaucraticForm> completedForms = new List<BureaucraticForm>();
    
    [Header("Bureaucratic Chaos")]
    public int bureaucraticErrorCount = 0;
    public float chaosMultiplier = 1.0f;
    
    public static BureaucracyGameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartScenario(BureaucraticScenario scenario)
    {
        currentScenario = scenario;
        activeForms.Clear();
        completedForms.Clear();
        bureaucraticErrorCount = 0;
        
        // Ajouter le premier formulaire du scénario
        if (scenario.steps.Count > 0 && scenario.steps[0].requiredForm != null)
        {
            AddForm(scenario.steps[0].requiredForm);
        }
        
        Debug.Log($"Scenario started: {scenario.scenarioTitle}");
    }
    
    public void AddForm(BureaucraticForm form)
    {
        if (!activeForms.Contains(form))
        {
            activeForms.Add(form);
            Debug.Log($"New form added: {form.formTitle} ({form.formCode})");
        }
    }
    
    public void SubmitForm(BureaucraticForm form)
    {
        if (!form.IsFormValid())
        {
            bureaucraticErrorCount++;
            Debug.LogWarning($"Form {form.formTitle} has validation errors!");
            return;
        }
        
        // Déplacer vers les formulaires complétés
        activeForms.Remove(form);
        completedForms.Add(form);
        
        // Déclencher les formulaires en cascade
        var triggeredForms = form.GetTriggeredForms();
        foreach (var triggeredForm in triggeredForms)
        {
            AddForm(triggeredForm);
        }
        
        // Augmenter le chaos bureaucratique
        chaosMultiplier += 0.1f * triggeredForms.Count;
        
        Debug.Log($"Form submitted: {form.formTitle}. Triggered {triggeredForms.Count} additional forms.");
        
        // Vérifier si le scénario est terminé
        CheckScenarioCompletion();
    }
    
    private void CheckScenarioCompletion()
    {
        if (currentScenario != null && currentScenario.IsScenarioComplete())
        {
            Debug.Log($"Scenario completed: {currentScenario.scenarioTitle}");
            // Déclencher les événements de fin de scénario
        }
    }
    
    public void CreateBureaucraticIncident(string incidentType, BureaucraticForm sourceForm)
    {
        Debug.LogWarning($"Bureaucratic incident: {incidentType} caused by {sourceForm.formTitle}");
        
        // Logique pour propager l'incident dans le système
        bureaucraticErrorCount++;
        chaosMultiplier *= 1.2f;
        
        // Possibilité de créer de nouveaux formulaires de correction
    }
}
