using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DynamicConfigurationManager : MonoBehaviour
{
    [Header("Configuration")]
    public BureaucracyScenario currentScenario;
    public List<DocumentTemplate> availableTemplates = new List<DocumentTemplate>();
    public FrenchDataGenerator dataGenerator;
    
    [Header("Runtime Settings")]
    public bool useFrenchLocalization = true;
    public float documentGenerationInterval = 30f;
    public int maxSimultaneousDocuments = 10;
    
    [Header("Events")]
    public System.Action<DocumentData> OnDynamicDocumentGenerated;
    public System.Action<BureaucracyTrigger> OnTriggerActivated;
    public System.Action<string> OnNarrativeUpdate;
    
    private DocumentManager documentManager;
    private List<FrenchCitizenData> activeCitizens = new List<FrenchCitizenData>();
    private Dictionary<string, DocumentTemplate> templateLookup = new Dictionary<string, DocumentTemplate>();
    private float lastGenerationTime;
    private int documentsProcessedThisSession = 0;
    
    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        documentManager = FindObjectOfType<DocumentManager>();
        
        if (currentScenario == null)
        {
            Debug.LogError("No BureaucracyScenario assigned!");
            return;
        }
        
        // Build template lookup
        BuildTemplateLookup();
        
        // Initialize citizen database
        InitializeCitizenDatabase();
        
        // Generate initial documents
        GenerateInitialDocuments();
        
        // Subscribe to document events
        if (documentManager != null)
        {
            documentManager.OnDocumentProcessed += OnDocumentProcessed;
        }
        
        Debug.Log($"Dynamic Configuration Manager initialized with scenario: {currentScenario.scenarioName}");
    }
    
    private void Update()
    {
        // Auto-generate new documents based on interval
        if (Time.time - lastGenerationTime > documentGenerationInterval)
        {
            if (documentManager.GetInboxCount() < maxSimultaneousDocuments)
            {
                GenerateRandomDocument();
                lastGenerationTime = Time.time;
            }
        }
        
        // Update narrative based on progress
        UpdateProgressiveNarrative();
    }
    
    private void BuildTemplateLookup()
    {
        templateLookup.Clear();
        
        // Add scenario-specific templates
        foreach (var template in currentScenario.availableDocuments)
        {
            if (template != null)
            {
                templateLookup[template.documentType.ToString()] = template;
            }
        }
        
        // Add additional available templates
        foreach (var template in availableTemplates)
        {
            if (template != null && !templateLookup.ContainsKey(template.documentType.ToString()))
            {
                templateLookup[template.documentType.ToString()] = template;
            }
        }
        
        Debug.Log($"Built template lookup with {templateLookup.Count} document types");
    }
    
    private void InitializeCitizenDatabase()
    {
        activeCitizens.Clear();
        
        if (currentScenario.citizenDatabase.Count > 0)
        {
            activeCitizens.AddRange(currentScenario.citizenDatabase);
        }
        else if (dataGenerator != null)
        {
            // Generate citizens on the fly
            activeCitizens.AddRange(dataGenerator.GenerateBatch());
        }
        
        Debug.Log($"Initialized with {activeCitizens.Count} active citizens");
    }
    
    private void GenerateInitialDocuments()
    {
        foreach (var template in currentScenario.startingDocuments)
        {
            if (template != null)
            {
                GenerateDocumentFromTemplate(template, GetRandomCitizen());
            }
        }
    }
    
    public void GenerateRandomDocument()
    {
        if (templateLookup.Count == 0 || activeCitizens.Count == 0) return;
        
        // Select random template
        var randomTemplate = templateLookup.Values.ElementAt(Random.Range(0, templateLookup.Count));
        var randomCitizen = GetRandomCitizen();
        
        GenerateDocumentFromTemplate(randomTemplate, randomCitizen);
    }
    
    public DocumentData GenerateDocumentFromTemplate(DocumentTemplate template, FrenchCitizenData citizen)
    {
        DocumentData document = new DocumentData();
        
        // Basic document info
        document.documentTitle = useFrenchLocalization && !string.IsNullOrEmpty(template.frenchTitle) 
            ? template.frenchTitle 
            : template.documentTitle;
            
        document.documentType = template.documentType;
        document.citizenName = $"{citizen.firstName} {citizen.lastName}";
        
        document.requestDetails = useFrenchLocalization && !string.IsNullOrEmpty(template.frenchDescription)
            ? template.frenchDescription
            : template.description;
            
        document.bureaucracyLevel = template.baseBureaucracyLevel;
        document.requiresStamp = template.requiresStamp;
        document.requiresSignature = template.requiresSignature;
        
        // Generate form fields from template
        GenerateFormFields(document, template, citizen);
        
        // Add to document manager
        documentManager.AddDocument(document);
        
        // Trigger any immediate cascades
        ProcessDocumentTriggers(document, template);
        
        OnDynamicDocumentGenerated?.Invoke(document);
        
        Debug.Log($"Generated document: {document.documentTitle} for {document.citizenName}");
        return document;
    }
    
    private void GenerateFormFields(DocumentData document, DocumentTemplate template, FrenchCitizenData citizen)
    {
        for (int i = 0; i < template.formFields.Count; i++)
        {
            var field = template.formFields[i];
            
            string fieldName = field.fieldName;
            
            // Use French localization if available
            if (useFrenchLocalization && i < template.frenchFieldNames.Count && 
                !string.IsNullOrEmpty(template.frenchFieldNames[i]))
            {
                fieldName = template.frenchFieldNames[i];
            }
            
            // Pre-fill some fields with citizen data
            string fieldValue = GetPrefilledValue(field, citizen);
            
            document.formFields[fieldName] = fieldValue;
        }
    }
    
    private string GetPrefilledValue(FormField field, FrenchCitizenData citizen)
    {
        string fieldName = field.fieldName.ToLower();
        
        // Auto-fill based on field name patterns
        if (fieldName.Contains("name") || fieldName.Contains("nom"))
        {
            return $"{citizen.firstName} {citizen.lastName}";
        }
        else if (fieldName.Contains("email") || fieldName.Contains("courriel"))
        {
            return citizen.email;
        }
        else if (fieldName.Contains("phone") || fieldName.Contains("téléphone"))
        {
            return citizen.phoneNumber;
        }
        else if (fieldName.Contains("address") || fieldName.Contains("adresse"))
        {
            return citizen.address;
        }
        else if (fieldName.Contains("birth") || fieldName.Contains("naissance"))
        {
            return citizen.birthDate;
        }
        else if (fieldName.Contains("profession") || fieldName.Contains("métier"))
        {
            return citizen.profession;
        }
        else if (fieldName.Contains("social") || fieldName.Contains("sécurité"))
        {
            return citizen.socialSecurityNumber;
        }
        
        // For dropdown fields, select random option
        if (field.fieldType == FormFieldType.Dropdown && field.dropdownOptions != null && field.dropdownOptions.Length > 0)
        {
            return field.dropdownOptions[Random.Range(0, field.dropdownOptions.Length)];
        }
        
        return ""; // Leave empty for manual filling
    }
    
    private void ProcessDocumentTriggers(DocumentData document, DocumentTemplate template)
    {
        foreach (var trigger in template.triggers)
        {
            if (ShouldTriggerActivate(trigger, document))
            {
                ActivateTrigger(trigger, document);
            }
        }
    }
    
    private bool ShouldTriggerActivate(BureaucracyTrigger trigger, DocumentData document)
    {
        // Check probability first
        if (Random.Range(0f, 1f) > trigger.probability)
        {
            return false;
        }
        
        switch (trigger.condition)
        {
            case TriggerCondition.Always:
                return true;
                
            case TriggerCondition.DocumentType:
                return document.documentType.ToString() == trigger.conditionValue;
                
            case TriggerCondition.BureaucracyLevel:
                if (int.TryParse(trigger.conditionValue, out int level))
                {
                    return document.bureaucracyLevel >= level;
                }
                break;
                
            case TriggerCondition.RandomChance:
                if (float.TryParse(trigger.conditionValue, out float chance))
                {
                    return Random.Range(0f, 1f) < chance;
                }
                break;
                
            case TriggerCondition.TimeOfDay:
                int currentHour = System.DateTime.Now.Hour;
                if (int.TryParse(trigger.conditionValue, out int targetHour))
                {
                    return currentHour == targetHour;
                }
                break;
                
            case TriggerCondition.DayOfWeek:
                string currentDay = System.DateTime.Now.DayOfWeek.ToString();
                return currentDay == trigger.conditionValue;
        }
        
        return false;
    }
    
    private void ActivateTrigger(BureaucracyTrigger trigger, DocumentData sourceDocument)
    {
        Debug.Log($"Activating trigger: {trigger.triggerName}");
        
        // Generate new documents
        foreach (string docType in trigger.newDocumentTypes)
        {
            if (templateLookup.ContainsKey(docType))
            {
                var template = templateLookup[docType];
                var citizen = GetCitizenByName(sourceDocument.citizenName);
                if (citizen != null)
                {
                    GenerateDocumentFromTemplate(template, citizen);
                }
            }
        }
        
        // Award bureaucracy score
        if (trigger.bureaucracyScoreBonus > 0)
        {
            GameManager.Instance.IncrementBureaucracyScore(trigger.bureaucracyScoreBonus);
        }
        
        // Show trigger message
        if (!string.IsNullOrEmpty(trigger.triggerMessage))
        {
            OnNarrativeUpdate?.Invoke(trigger.triggerMessage);
        }
        
        OnTriggerActivated?.Invoke(trigger);
    }
    
    private void OnDocumentProcessed(DocumentData document)
    {
        documentsProcessedThisSession++;
        
        // Find the template for this document type
        if (templateLookup.ContainsKey(document.documentType.ToString()))
        {
            var template = templateLookup[document.documentType.ToString()];
            ProcessDocumentTriggers(document, template);
        }
        
        // Update citizen desperation levels
        UpdateCitizenDesperation(document);
        
        // Check for scenario completion
        CheckScenarioCompletion();
    }
    
    private void UpdateCitizenDesperation(DocumentData document)
    {
        var citizen = GetCitizenByName(document.citizenName);
        if (citizen != null)
        {
            // Processing documents reduces desperation
            citizen.desperationLevel = Mathf.Max(0f, citizen.desperationLevel - 0.1f);
            citizen.previousRequestsCount++;
        }
    }
    
    private void UpdateProgressiveNarrative()
    {
        if (currentScenario.progressiveNarrativeTexts.Count == 0) return;
        
        // Update narrative based on documents processed
        int narrativeIndex = Mathf.Min(
            documentsProcessedThisSession / 5, // Every 5 documents
            currentScenario.progressiveNarrativeTexts.Count - 1
        );
        
        // Check for absurdity escalation
        if (currentScenario.absurdityEscalation.Count > 0)
        {
            int absurdityIndex = Mathf.Min(
                documentsProcessedThisSession / 10, // Every 10 documents
                currentScenario.absurdityEscalation.Count - 1
            );
            
            if (absurdityIndex < currentScenario.absurdityEscalation.Count)
            {
                string absurdityText = currentScenario.absurdityEscalation[absurdityIndex];
                OnNarrativeUpdate?.Invoke(absurdityText);
            }
        }
    }
    
    private void CheckScenarioCompletion()
    {
        bool documentsGoalMet = documentsProcessedThisSession >= currentScenario.targetDocumentsProcessed;
        bool scoreGoalMet = GameManager.Instance.bureaucracyScore >= currentScenario.targetBureaucracyScore;
        
        if (documentsGoalMet && scoreGoalMet)
        {
            OnScenarioCompleted();
        }
    }
    
    private void OnScenarioCompleted()
    {
        string completionMessage = $"Scénario '{currentScenario.scenarioName}' terminé!\n";
        completionMessage += $"Documents traités: {documentsProcessedThisSession}\n";
        completionMessage += $"Score bureaucratique: {GameManager.Instance.bureaucracyScore}";
        
        OnNarrativeUpdate?.Invoke(completionMessage);
        Debug.Log("Scenario completed!");
    }
    
    private FrenchCitizenData GetRandomCitizen()
    {
        if (activeCitizens.Count == 0) return null;
        
        // Weighted selection based on desperation level
        var weightedCitizens = activeCitizens.Where(c => c.desperationLevel > 0.1f).ToList();
        if (weightedCitizens.Count == 0)
        {
            weightedCitizens = activeCitizens;
        }
        
        return weightedCitizens[Random.Range(0, weightedCitizens.Count)];
    }
    
    private FrenchCitizenData GetCitizenByName(string fullName)
    {
        return activeCitizens.FirstOrDefault(c => $"{c.firstName} {c.lastName}" == fullName);
    }
    
    // Public methods for external control
    public void LoadScenario(BureaucracyScenario scenario)
    {
        currentScenario = scenario;
        Initialize();
    }
    
    public void AddCustomTemplate(DocumentTemplate template)
    {
        if (template != null && !templateLookup.ContainsKey(template.documentType.ToString()))
        {
            availableTemplates.Add(template);
            templateLookup[template.documentType.ToString()] = template;
            Debug.Log($"Added custom template: {template.documentTitle}");
        }
    }
    
    public void ForceGenerateDocument(DocumentType docType, string citizenName = null)
    {
        if (templateLookup.ContainsKey(docType.ToString()))
        {
            var template = templateLookup[docType.ToString()];
            var citizen = !string.IsNullOrEmpty(citizenName) 
                ? GetCitizenByName(citizenName) 
                : GetRandomCitizen();
                
            if (citizen != null)
            {
                GenerateDocumentFromTemplate(template, citizen);
            }
        }
    }
    
    public void SetGenerationInterval(float interval)
    {
        documentGenerationInterval = interval;
    }
    
    public void SetMaxSimultaneousDocuments(int max)
    {
        maxSimultaneousDocuments = max;
    }
    
    public List<string> GetAvailableDocumentTypes()
    {
        return templateLookup.Keys.ToList();
    }
    
    public DocumentTemplate GetTemplate(DocumentType docType)
    {
        string key = docType.ToString();
        return templateLookup.ContainsKey(key) ? templateLookup[key] : null;
    }
    
    public BureaucracyScenarioStatus GetScenarioStatus()
    {
        return new BureaucracyScenarioStatus
        {
            scenarioName = currentScenario?.scenarioName ?? "None",
            documentsProcessed = documentsProcessedThisSession,
            targetDocuments = currentScenario?.targetDocumentsProcessed ?? 0,
            currentScore = GameManager.Instance?.bureaucracyScore ?? 0,
            targetScore = currentScenario?.targetBureaucracyScore ?? 0,
            activeCitizensCount = activeCitizens.Count,
            availableTemplatesCount = templateLookup.Count,
            averageCitizenDesperation = activeCitizens.Count > 0 
                ? activeCitizens.Average(c => c.desperationLevel) 
                : 0f
        };
    }
}

