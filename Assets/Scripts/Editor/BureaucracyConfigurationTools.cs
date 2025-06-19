#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public static class BureaucracyConfigurationTools
{
    private const string TEMPLATES_PATH = "Assets/PaperTrail/DocumentTemplates/";
    private const string SCENARIOS_PATH = "Assets/PaperTrail/Scenarios/";
    private const string DATA_GENERATOR_PATH = "Assets/PaperTrail/Data/";
    
    [MenuItem("Tools/Paper Trail/Configuration/Create Document Template")]
    public static void CreateDocumentTemplate()
    {
        EnsureDirectoryExists(TEMPLATES_PATH);
        
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "New Document Template";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Template description";
        template.baseBureaucracyLevel = 1;
        
        // Add default form fields
        template.formFields.Add(new FormField
        {
            fieldName = "Nom complet",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Entrez votre nom complet"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Date de début",
            fieldType = FormFieldType.Date,
            isRequired = true,
            placeholder = "JJ/MM/AAAA"
        });
        
        string path = TEMPLATES_PATH + "NewDocumentTemplate.asset";
        AssetDatabase.CreateAsset(template, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = template;
        
        Debug.Log($"Created new Document Template at: {path}");
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Create Bureaucracy Scenario")]
    public static void CreateBureaucracyScenario()
    {
        EnsureDirectoryExists(SCENARIOS_PATH);
        
        BureaucracyScenario scenario = ScriptableObject.CreateInstance<BureaucracyScenario>();
        scenario.scenarioName = "New Bureaucracy Scenario";
        scenario.scenarioDescription = "A new kafkaesque bureaucratic adventure";
        scenario.difficultyLevel = 1;
        scenario.targetDocumentsProcessed = 10;
        scenario.targetBureaucracyScore = 1000;
        scenario.timeLimit = 300f;
        
        // Add some default narrative texts
        scenario.progressiveNarrativeTexts.Add("Bienvenue dans le système bureaucratique français...");
        scenario.progressiveNarrativeTexts.Add("Les formulaires se multiplient...");
        scenario.progressiveNarrativeTexts.Add("L'absurdité atteint de nouveaux sommets...");
        
        scenario.absurdityEscalation.Add("Un formulaire pour demander l'autorisation de remplir un formulaire!");
        scenario.absurdityEscalation.Add("Maintenant il faut un certificat médical pour respirer!");
        scenario.absurdityEscalation.Add("Une validation notariale est requise pour cligner des yeux!");
        
        string path = SCENARIOS_PATH + "NewBureaucracyScenario.asset";
        AssetDatabase.CreateAsset(scenario, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = scenario;
        
        Debug.Log($"Created new Bureaucracy Scenario at: {path}");
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Create French Data Generator")]
    public static void CreateFrenchDataGenerator()
    {
        EnsureDirectoryExists(DATA_GENERATOR_PATH);
        
        FrenchDataGenerator generator = ScriptableObject.CreateInstance<FrenchDataGenerator>();
        generator.batchSize = 50;
        
        string path = DATA_GENERATOR_PATH + "FrenchDataGenerator.asset";
        AssetDatabase.CreateAsset(generator, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = generator;
        
        Debug.Log($"Created French Data Generator at: {path}");
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Generate Complete Vacation Scenario")]
    public static void GenerateCompleteVacationScenario()
    {
        EnsureDirectoryExists(TEMPLATES_PATH);
        EnsureDirectoryExists(SCENARIOS_PATH);
        
        // Create vacation request template
        DocumentTemplate vacationTemplate = CreateVacationRequestTemplate();
        
        // Create medical certificate template
        DocumentTemplate medicalTemplate = CreateMedicalCertificateTemplate();
        
        // Create approved doctors list template
        DocumentTemplate doctorsTemplate = CreateApprovedDoctorsListTemplate();
        
        // Create HR validation template
        DocumentTemplate hrTemplate = CreateHRValidationTemplate();
        
        // Create scenario that uses all these templates
        BureaucracyScenario scenario = ScriptableObject.CreateInstance<BureaucracyScenario>();
        scenario.scenarioName = "L'Enfer des Congés Payés";
        scenario.scenarioDescription = "Une simple demande de congé qui devient un cauchemar bureaucratique";
        scenario.difficultyLevel = 3;
        scenario.targetDocumentsProcessed = 15;
        scenario.targetBureaucracyScore = 2000;
        scenario.timeLimit = 600f; // 10 minutes
        
        // Add templates to scenario
        scenario.startingDocuments.Add(vacationTemplate);
        scenario.availableDocuments.Add(vacationTemplate);
        scenario.availableDocuments.Add(medicalTemplate);
        scenario.availableDocuments.Add(doctorsTemplate);
        scenario.availableDocuments.Add(hrTemplate);
        
        // Add narrative progression
        scenario.progressiveNarrativeTexts.Add("Bienvenue au Service des Ressources Humaines...");
        scenario.progressiveNarrativeTexts.Add("Votre demande de congé nécessite une validation médicale...");
        scenario.progressiveNarrativeTexts.Add("Le médecin doit être approuvé par notre liste officielle...");
        scenario.progressiveNarrativeTexts.Add("Les RH doivent valider votre choix de médecin...");
        scenario.progressiveNarrativeTexts.Add("Félicitations! Votre congé de 3 jours a généré 47 documents!");
        
        scenario.absurdityEscalation.Add("Un congé de 3 jours requiert un certificat médical... pour se reposer!");
        scenario.absurdityEscalation.Add("Le médecin doit être choisi dans une liste de 3 praticiens... tous en vacances!");
        scenario.absurdityEscalation.Add("Il faut maintenant une autorisation pour avoir l'autorisation d'avoir une autorisation!");
        
        string scenarioPath = SCENARIOS_PATH + "VacationNightmareScenario.asset";
        AssetDatabase.CreateAsset(scenario, scenarioPath);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = scenario;
        
        EditorUtility.DisplayDialog(
            "Scénario Généré!",
            "Le scénario complet 'L'Enfer des Congés Payés' a été créé avec 4 templates de documents et une cascade bureaucratique complète!",
            "Excellent!"
        );
        
        Debug.Log("Generated complete vacation nightmare scenario with all document templates!");
    }
    
    private static DocumentTemplate CreateVacationRequestTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Vacation Request Form";
        template.frenchTitle = "Demande de Congés Payés";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Standard vacation request form";
        template.frenchDescription = "Formulaire standard de demande de congés payés";
        template.baseBureaucracyLevel = 1;
        template.requiresStamp = true;
        template.requiresSignature = false;
        
        // Add form fields
        template.formFields.Add(new FormField
        {
            fieldName = "Employee Name",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Full name"
        });
        template.frenchFieldNames.Add("Nom de l'employé");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Start Date",
            fieldType = FormFieldType.Date,
            isRequired = true,
            placeholder = "DD/MM/YYYY"
        });
        template.frenchFieldNames.Add("Date de début");
        
        template.formFields.Add(new FormField
        {
            fieldName = "End Date",
            fieldType = FormFieldType.Date,
            isRequired = true,
            placeholder = "DD/MM/YYYY"
        });
        template.frenchFieldNames.Add("Date de fin");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Reason",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Reason for vacation"
        });
        template.frenchFieldNames.Add("Motif");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Duration",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "1 day", "2 days", "3 days", "1 week", "2 weeks" }
        });
        template.frenchFieldNames.Add("Durée");
        
        // Add trigger for medical certificate
        BureaucracyTrigger medicalTrigger = new BureaucracyTrigger
        {
            triggerName = "Medical Certificate Required",
            condition = TriggerCondition.Always,
            probability = 1f,
            newDocumentTypes = new string[] { "MedicalCertificateRequest" },
            bureaucracyScoreBonus = 50,
            triggerMessage = "Attention! Les congés de plus de 2 jours nécessitent un certificat médical!",
            canTriggerRecursively = true,
            maxCascadeDepth = 5
        };
        template.triggers.Add(medicalTrigger);
        
        string path = TEMPLATES_PATH + "VacationRequestTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        
        return template;
    }
    
    private static DocumentTemplate CreateMedicalCertificateTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Medical Certificate Request";
        template.frenchTitle = "Demande de Certificat Médical";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Request for medical certificate";
        template.frenchDescription = "Demande de certificat médical de complaisance";
        template.baseBureaucracyLevel = 2;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        // Add form fields
        template.formFields.Add(new FormField
        {
            fieldName = "Patient Name",
            fieldType = FormFieldType.Text,
            isRequired = true
        });
        template.frenchFieldNames.Add("Nom du patient");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Requested Certificate Type",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Fitness for vacation", "Rest certificate", "Stress leave justification" }
        });
        template.frenchFieldNames.Add("Type de certificat demandé");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Duration of Rest",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Number of days"
        });
        template.frenchFieldNames.Add("Durée du repos");
        
        // Trigger for approved doctors list
        BureaucracyTrigger doctorsTrigger = new BureaucracyTrigger
        {
            triggerName = "Approved Doctors List Required",
            condition = TriggerCondition.Always,
            probability = 1f,
            newDocumentTypes = new string[] { "ApprovedDoctorsList" },
            bureaucracyScoreBonus = 75,
            triggerMessage = "Le médecin doit être choisi dans notre liste officielle de praticiens agréés!",
            canTriggerRecursively = true
        };
        template.triggers.Add(doctorsTrigger);
        
        string path = TEMPLATES_PATH + "MedicalCertificateTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        
        return template;
    }
    
    private static DocumentTemplate CreateApprovedDoctorsListTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Approved Doctors Selection";
        template.frenchTitle = "Sélection de Médecin Agréé";
        template.documentType = DocumentType.ApprovedDoctorsList;
        template.description = "Selection from approved medical practitioners";
        template.frenchDescription = "Sélection parmi les praticiens médicaux agréés";
        template.baseBureaucracyLevel = 3;
        template.requiresStamp = false;
        template.requiresSignature = false;
        
        // Add form fields
        template.formFields.Add(new FormField
        {
            fieldName = "Selected Doctor",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Dr. Dupont (en vacances)", 
                "Dr. Martin (en congé maladie)", 
                "Dr. Bernard (retraité depuis 1987)" 
            }
        });
        template.frenchFieldNames.Add("Médecin sélectionné");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Doctor License Number",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "License number"
        });
        template.frenchFieldNames.Add("Numéro de licence");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Appointment Date",
            fieldType = FormFieldType.Date,
            isRequired = true
        });
        template.frenchFieldNames.Add("Date de rendez-vous");
        
        // Trigger for HR validation
        BureaucracyTrigger hrTrigger = new BureaucracyTrigger
        {
            triggerName = "HR Validation Required",
            condition = TriggerCondition.Always,
            probability = 1f,
            newDocumentTypes = new string[] { "HRValidationForm" },
            bureaucracyScoreBonus = 100,
            triggerMessage = "Les Ressources Humaines doivent valider votre choix de médecin!",
            canTriggerRecursively = false
        };
        template.triggers.Add(hrTrigger);
        
        string path = TEMPLATES_PATH + "ApprovedDoctorsTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        
        return template;
    }
    
    private static DocumentTemplate CreateHRValidationTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "HR Validation Form";
        template.frenchTitle = "Formulaire de Validation RH";
        template.documentType = DocumentType.HRValidationForm;
        template.description = "HR validation for doctor selection";
        template.frenchDescription = "Validation des Ressources Humaines pour la sélection du médecin";
        template.baseBureaucracyLevel = 4;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        // Add form fields
        template.formFields.Add(new FormField
        {
            fieldName = "HR Officer Name",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Responsible HR officer"
        });
        template.frenchFieldNames.Add("Nom de l'agent RH");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Validation Date",
            fieldType = FormFieldType.Date,
            isRequired = true
        });
        template.frenchFieldNames.Add("Date de validation");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Doctor Validation Status",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Approved with conditions", 
                "Conditionally approved", 
                "Approved pending further validation",
                "Temporarily approved subject to review"
            }
        });
        template.frenchFieldNames.Add("Statut de validation du médecin");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Additional Comments",
            fieldType = FormFieldType.TextArea,
            isRequired = false,
            placeholder = "Any additional observations"
        });
        template.frenchFieldNames.Add("Commentaires supplémentaires");
        
        template.formFields.Add(new FormField
        {
            fieldName = "Requires Director Approval",
            fieldType = FormFieldType.Checkbox,
            isRequired = false
        });
        template.frenchFieldNames.Add("Nécessite l'approbation du directeur");
        
        // Optional trigger for director approval (recursive madness!)
        BureaucracyTrigger directorTrigger = new BureaucracyTrigger
        {
            triggerName = "Director Approval Required",
            condition = TriggerCondition.FieldValue,
            conditionValue = "true", // If checkbox is checked
            probability = 0.3f, // 30% chance
            newDocumentTypes = new string[] { "VacationRequest" }, // Creates another vacation request!
            bureaucracyScoreBonus = 200,
            triggerMessage = "Le directeur exige maintenant une nouvelle demande de congé avec justification supplémentaire!",
            canTriggerRecursively = true,
            maxCascadeDepth = 10
        };
        template.triggers.Add(directorTrigger);
        
        string path = TEMPLATES_PATH + "HRValidationTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        
        return template;
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Generate French Citizens Database")]
    public static void GenerateFrenchCitizensDatabase()
    {
        // Find or create French Data Generator
        FrenchDataGenerator generator = AssetDatabase.LoadAssetAtPath<FrenchDataGenerator>("Assets/PaperTrail/Data/FrenchDataGenerator.asset");
        if (generator == null)
        {
            CreateFrenchDataGenerator();
            generator = AssetDatabase.LoadAssetAtPath<FrenchDataGenerator>("Assets/PaperTrail/Data/FrenchDataGenerator.asset");
        }
        
        if (generator == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not create or find FrenchDataGenerator!", "OK");
            return;
        }
        
        // Ask user how many citizens to generate
        int citizenCount = EditorUtility.DisplayDialogComplex(
            "Generate French Citizens",
            "How many French citizens would you like to generate?",
            "100 Citizens", "500 Citizens", "1000 Citizens"
        );
        
        int count = citizenCount == 0 ? 100 : citizenCount == 1 ? 500 : 1000;
        
        // Generate citizens
        List<FrenchCitizenData> citizens = new List<FrenchCitizenData>();
        for (int i = 0; i < count; i++)
        {
            citizens.Add(generator.GenerateRandomCitizen());
        }
        
        // Save to JSON file
        string json = JsonUtility.ToJson(new SerializableList<FrenchCitizenData>(citizens), true);
        string path = EditorUtility.SaveFilePanel("Save French Citizens Database", "", $"FrenchCitizens_{count}.json", "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            Debug.Log($"Generated and saved {count} French citizens to: {path}");
            
            EditorUtility.DisplayDialog(
                "Database Generated!",
                $"Successfully generated {count} French citizens and saved to:\n{path}",
                "Great!"
            );
        }
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Clear All Paper Trail Objects")]
    public static void ClearAllObjects()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Clear All Objects",
            "This will delete all Paper Trail objects in the scene. Are you sure?\n\n" +
            "This action cannot be undone!",
            "Yes, Clear All",
            "Cancel"
        );

        if (!confirm) return;

        // Find and destroy all Paper Trail objects in the scene
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null) 
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(canvas.gameObject);
        }

        UnityEngine.EventSystems.EventSystem eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null) 
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(eventSystem.gameObject);
        }

        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null) 
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(gameManager.gameObject);
        }

        DocumentManager documentManager = Object.FindObjectOfType<DocumentManager>();
        if (documentManager != null) 
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(documentManager.gameObject);
        }

        BureaucracySystem bureaucracySystem = Object.FindObjectOfType<BureaucracySystem>();
        if (bureaucracySystem != null) 
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(bureaucracySystem.gameObject);
        }

        DynamicConfigurationManager configManager = Object.FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(configManager.gameObject);
        }

        PhysiologicalNeedsManager needsManager = Object.FindObjectOfType<PhysiologicalNeedsManager>();
        if (needsManager != null)
        {
            EditorApplication.delayCall += () => Object.DestroyImmediate(needsManager.gameObject);
        }

        Debug.Log("All Paper Trail objects cleared!");
        
        EditorUtility.DisplayDialog(
            "Objects Cleared",
            "All Paper Trail objects have been removed from the scene.",
            "OK"
        );
    }

    // Helper method to ensure directory exists
    public static void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentPath = Path.GetDirectoryName(path);
            string folderName = Path.GetFileName(path);
            
            if (!string.IsNullOrEmpty(parentPath) && !AssetDatabase.IsValidFolder(parentPath))
            {
                EnsureDirectoryExists(parentPath);
            }
            
            AssetDatabase.CreateFolder(parentPath, folderName);
        }
    }
}
#endif
