#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
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
        FrenchDataGenerator generator = Resources.Load<FrenchDataGenerator>("FrenchDataGenerator");
        if (generator == null)
        {
            CreateFrenchDataGenerator();
            generator = Resources.Load<FrenchDataGenerator>("FrenchDataGenerator");
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
    
    [MenuItem("Tools/Paper Trail/Configuration/Create Prefab Templates")]
    public static void CreatePrefabTemplates()
    {
        string prefabPath = "Assets/PaperTrail/Prefabs/";
        EnsureDirectoryExists(prefabPath);
        
        // Create form field prefabs
        CreateFormFieldPrefabs(prefabPath);
        
        // Create document item prefab
        CreateDocumentItemPrefab(prefabPath);
        
        EditorUtility.DisplayDialog(
            "Prefabs Created!",
            "All necessary prefab templates have been created in:\n" + prefabPath,
            "Perfect!"
        );
    }
    
    private static void CreateFormFieldPrefabs(string basePath)
    {
        string fieldPath = basePath + "FormFields/";
        EnsureDirectoryExists(fieldPath);
        
        // Text Field Prefab
        CreateTextFieldPrefab(fieldPath + "TextFieldPrefab.prefab");
        
        // Dropdown Field Prefab
        CreateDropdownFieldPrefab(fieldPath + "DropdownFieldPrefab.prefab");
        
        // Checkbox Field Prefab
        CreateCheckboxFieldPrefab(fieldPath + "CheckboxFieldPrefab.prefab");
        
        // Date Field Prefab
        CreateDateFieldPrefab(fieldPath + "DateFieldPrefab.prefab");
        
        // TextArea Field Prefab
        CreateTextAreaFieldPrefab(fieldPath + "TextAreaFieldPrefab.prefab");
    }
    
    private static void CreateTextFieldPrefab(string path)
    {
        GameObject fieldGO = new GameObject("TextFieldPrefab");
        
        // Add RectTransform
        RectTransform rt = fieldGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(350, 60);
        
        // Add VerticalLayoutGroup
        VerticalLayoutGroup vlg = fieldGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.childForceExpandHeight = false;
        vlg.childControlHeight = false;
        
        // Create Label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(fieldGO.transform);
        TMPro.TextMeshProUGUI label = labelGO.AddComponent<TMPro.TextMeshProUGUI>();
        label.text = "Field Label";
        label.fontSize = 14;
        label.color = Color.black;
        
        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(350, 20);
        
        // Create InputField
        GameObject inputGO = new GameObject("InputField");
        inputGO.transform.SetParent(fieldGO.transform);
        
        UnityEngine.UI.Image inputBG = inputGO.AddComponent<UnityEngine.UI.Image>();
        inputBG.color = Color.white;
        
        TMPro.TMP_InputField inputField = inputGO.AddComponent<TMPro.TMP_InputField>();
        
        // Create Text component for input
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(inputGO.transform);
        TMPro.TextMeshProUGUI inputText = textGO.AddComponent<TMPro.TextMeshProUGUI>();
        inputText.color = Color.black;
        inputText.fontSize = 12;
        
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 0);
        textRT.offsetMax = new Vector2(-10, 0);
        
        // Create Placeholder
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(inputGO.transform);
        TMPro.TextMeshProUGUI placeholder = placeholderGO.AddComponent<TMPro.TextMeshProUGUI>();
        placeholder.text = "Enter text...";
        placeholder.color = new Color(0.5f, 0.5f, 0.5f);
        placeholder.fontSize = 12;
        
        RectTransform placeholderRT = placeholderGO.GetComponent<RectTransform>();
        placeholderRT.anchorMin = Vector2.zero;
        placeholderRT.anchorMax = Vector2.one;
        placeholderRT.offsetMin = new Vector2(10, 0);
        placeholderRT.offsetMax = new Vector2(-10, 0);
        
        // Configure InputField
        inputField.textComponent = inputText;
        inputField.placeholder = placeholder;
        
        RectTransform inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.sizeDelta = new Vector2(350, 30);
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(fieldGO, path);
        Object.DestroyImmediate(fieldGO);
    }
    
    private static void CreateDropdownFieldPrefab(string path)
    {
        GameObject fieldGO = new GameObject("DropdownFieldPrefab");
        
        // Add RectTransform and layout
        RectTransform rt = fieldGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(350, 60);
        
        VerticalLayoutGroup vlg = fieldGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.childForceExpandHeight = false;
        vlg.childControlHeight = false;
        
        // Create Label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(fieldGO.transform);
        TMPro.TextMeshProUGUI label = labelGO.AddComponent<TMPro.TextMeshProUGUI>();
        label.text = "Dropdown Label";
        label.fontSize = 14;
        label.color = Color.black;
        
        // Create Dropdown
        GameObject dropdownGO = new GameObject("Dropdown");
        dropdownGO.transform.SetParent(fieldGO.transform);
        
        UnityEngine.UI.Image dropdownBG = dropdownGO.AddComponent<UnityEngine.UI.Image>();
        dropdownBG.color = Color.white;
        
        TMPro.TMP_Dropdown dropdown = dropdownGO.AddComponent<TMPro.TMP_Dropdown>();
        
        // Create Label for dropdown
        GameObject dropdownLabelGO = new GameObject("Label");
        dropdownLabelGO.transform.SetParent(dropdownGO.transform);
        TMPro.TextMeshProUGUI dropdownLabel = dropdownLabelGO.AddComponent<TMPro.TextMeshProUGUI>();
        dropdownLabel.text = "Option 1";
        dropdownLabel.color = Color.black;
        dropdownLabel.fontSize = 12;
        
        RectTransform dropdownLabelRT = dropdownLabelGO.GetComponent<RectTransform>();
        dropdownLabelRT.anchorMin = Vector2.zero;
        dropdownLabelRT.anchorMax = Vector2.one;
        dropdownLabelRT.offsetMin = new Vector2(10, 0);
        dropdownLabelRT.offsetMax = new Vector2(-25, 0);
        
        // Create Arrow
        GameObject arrowGO = new GameObject("Arrow");
        arrowGO.transform.SetParent(dropdownGO.transform);
        UnityEngine.UI.Image arrow = arrowGO.AddComponent<UnityEngine.UI.Image>();
        arrow.color = Color.black;
        
        RectTransform arrowRT = arrowGO.GetComponent<RectTransform>();
        arrowRT.anchorMin = new Vector2(1, 0.5f);
        arrowRT.anchorMax = new Vector2(1, 0.5f);
        arrowRT.sizeDelta = new Vector2(20, 20);
        arrowRT.anchoredPosition = new Vector2(-15, 0);
        
        // Configure dropdown
        dropdown.captionText = dropdownLabel;
        dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("Option 1"));
        dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("Option 2"));
        dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("Option 3"));
        
        RectTransform dropdownRT = dropdownGO.GetComponent<RectTransform>();
        dropdownRT.sizeDelta = new Vector2(350, 30);
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(fieldGO, path);
        Object.DestroyImmediate(fieldGO);
    }
    
    private static void CreateCheckboxFieldPrefab(string path)
    {
        GameObject fieldGO = new GameObject("CheckboxFieldPrefab");
        
        // Add RectTransform and layout
        RectTransform rt = fieldGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(350, 30);
        
        HorizontalLayoutGroup hlg = fieldGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childForceExpandWidth = false;
        hlg.childControlWidth = false;
        
        // Create Toggle (Checkbox)
        GameObject toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(fieldGO.transform);
        
        UnityEngine.UI.Image toggleBG = toggleGO.AddComponent<UnityEngine.UI.Image>();
        toggleBG.color = Color.white;
        
        UnityEngine.UI.Toggle toggle = toggleGO.AddComponent<UnityEngine.UI.Toggle>();
        
        // Create Checkmark
        GameObject checkmarkGO = new GameObject("Checkmark");
        checkmarkGO.transform.SetParent(toggleGO.transform);
        UnityEngine.UI.Image checkmark = checkmarkGO.AddComponent<UnityEngine.UI.Image>();
        checkmark.color = Color.green;
        
        RectTransform checkmarkRT = checkmarkGO.GetComponent<RectTransform>();
        checkmarkRT.anchorMin = Vector2.zero;
        checkmarkRT.anchorMax = Vector2.one;
        checkmarkRT.offsetMin = Vector2.zero;
        checkmarkRT.offsetMax = Vector2.zero;
        
        // Configure toggle
        toggle.graphic = checkmark;
        
        RectTransform toggleRT = toggleGO.GetComponent<RectTransform>();
        toggleRT.sizeDelta = new Vector2(20, 20);
        
        // Create Label
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(fieldGO.transform);
        TMPro.TextMeshProUGUI label = labelGO.AddComponent<TMPro.TextMeshProUGUI>();
        label.text = "Checkbox Label";
        label.fontSize = 14;
        label.color = Color.black;
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(fieldGO, path);
        Object.DestroyImmediate(fieldGO);
    }
    
    private static void CreateDateFieldPrefab(string path)
    {
        // Similar to text field but with date validation
        CreateTextFieldPrefab(path);
        // The validation will be handled by the DynamicDocumentUI component
    }
    
    private static void CreateTextAreaFieldPrefab(string path)
    {
        // Similar to text field but with multiline capability
        CreateTextFieldPrefab(path);
        // The multiline setting will be configured in DynamicDocumentUI
    }
    
    private static void CreateDocumentItemPrefab(string path)
    {
        GameObject itemGO = new GameObject("DocumentItemPrefab");
        
        // Add RectTransform
        RectTransform rt = itemGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(380, 80);
        
        // Add Background Image
        UnityEngine.UI.Image bg = itemGO.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.95f, 0.95f, 0.95f);
        
        // Add Button component
        UnityEngine.UI.Button button = itemGO.AddComponent<UnityEngine.UI.Button>();
        
        // Add VerticalLayoutGroup
        VerticalLayoutGroup vlg = itemGO.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(10, 10, 5, 5);
        vlg.spacing = 2;
        vlg.childForceExpandHeight = false;
        vlg.childControlHeight = false;
        
        // Create Title Text
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(itemGO.transform);
        TMPro.TextMeshProUGUI title = titleGO.AddComponent<TMPro.TextMeshProUGUI>();
        title.text = "Document Title";
        title.fontSize = 14;
        title.fontStyle = TMPro.FontStyles.Bold;
        title.color = Color.black;
        
        // Create Citizen Text
        GameObject citizenGO = new GameObject("CitizenText");
        citizenGO.transform.SetParent(itemGO.transform);
        TMPro.TextMeshProUGUI citizen = citizenGO.AddComponent<TMPro.TextMeshProUGUI>();
        citizen.text = "From: Citizen Name";
        citizen.fontSize = 12;
        citizen.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Create Status Text
        GameObject statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(itemGO.transform);
        TMPro.TextMeshProUGUI status = statusGO.AddComponent<TMPro.TextMeshProUGUI>();
        status.text = "Status: Pending";
        status.fontSize = 11;
        status.color = Color.blue;
        
        // Add DocumentItemUI script
        DocumentItemUI itemUI = itemGO.AddComponent<DocumentItemUI>();
        itemUI.titleText = title;
        itemUI.citizenText = citizen;
        itemUI.statusText = status;
        itemUI.selectButton = button;
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(itemGO, path);
        Object.DestroyImmediate(itemGO);
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Setup Complete Project Structure")]
    public static void SetupCompleteProjectStructure()
    {
        // Create all necessary directories
        EnsureDirectoryExists("Assets/PaperTrail/");
        EnsureDirectoryExists(TEMPLATES_PATH);
        EnsureDirectoryExists(SCENARIOS_PATH);
        EnsureDirectoryExists(DATA_GENERATOR_PATH);
        EnsureDirectoryExists("Assets/PaperTrail/Prefabs/");
        EnsureDirectoryExists("Assets/PaperTrail/Prefabs/FormFields/");
        EnsureDirectoryExists("Assets/PaperTrail/Resources/");
        
        // Generate complete vacation scenario
        GenerateCompleteVacationScenario();
        
        // Create French data generator
        CreateFrenchDataGenerator();
        
        // Create prefab templates
        CreatePrefabTemplates();
        
        // Generate citizens database
        GenerateFrenchCitizensDatabase();
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "Project Setup Complete!",
            "Paper Trail project structure has been completely set up with:\n\n" +
            "✓ Document Templates\n" +
            "✓ Bureaucracy Scenarios\n" +
            "✓ French Data Generator\n" +
            "✓ Form Field Prefabs\n" +
            "✓ Citizens Database\n" +
            "✓ Complete Vacation Nightmare Scenario\n\n" +
            "You can now start customizing your bureaucratic nightmare!",
            "Magnifique!"
        );
        
        Debug.Log("=== PAPER TRAIL PROJECT SETUP COMPLETE ===");
        Debug.Log("All systems ready for bureaucratic chaos!");
    }
    
    [MenuItem("Tools/Paper Trail/Configuration/Validate Configuration")]
    public static void ValidateConfiguration()
    {
        List<string> issues = new List<string>();
        List<string> success = new List<string>();
        
        // Check for templates
        string[] templateGuids = AssetDatabase.FindAssets("t:DocumentTemplate");
        if (templateGuids.Length == 0)
        {
            issues.Add("❌ No DocumentTemplate assets found");
        }
        else
        {
            success.Add($"✅ Found {templateGuids.Length} DocumentTemplate(s)");
        }
        
        // Check for scenarios
        string[] scenarioGuids = AssetDatabase.FindAssets("t:BureaucracyScenario");
        if (scenarioGuids.Length == 0)
        {
            issues.Add("❌ No BureaucracyScenario assets found");
        }
        else
        {
            success.Add($"✅ Found {scenarioGuids.Length} BureaucracyScenario(s)");
        }
        
        // Check for data generator
        string[] generatorGuids = AssetDatabase.FindAssets("t:FrenchDataGenerator");
        if (generatorGuids.Length == 0)
        {
            issues.Add("❌ No FrenchDataGenerator assets found");
        }
        else
        {
            success.Add($"✅ Found {generatorGuids.Length} FrenchDataGenerator(s)");
        }
        
        // Check for DynamicConfigurationManager in scene
        DynamicConfigurationManager configManager = Object.FindObjectOfType<DynamicConfigurationManager>();
        if (configManager == null)
        {
            issues.Add("❌ No DynamicConfigurationManager found in scene");
        }
        else
        {
            success.Add("✅ DynamicConfigurationManager found in scene");
            
            if (configManager.currentScenario == null)
            {
                issues.Add("❌ DynamicConfigurationManager has no scenario assigned");
            }
            else
            {
                success.Add($"✅ Scenario assigned: {configManager.currentScenario.scenarioName}");
            }
        }
        
        // Generate report
        string report = "=== PAPER TRAIL CONFIGURATION VALIDATION ===\n\n";
        
        if (success.Count > 0)
        {
            report += "SUCCESS:\n";
            foreach (string s in success)
            {
                report += s + "\n";
            }
            report += "\n";
        }
        
        if (issues.Count > 0)
        {
            report += "ISSUES FOUND:\n";
            foreach (string issue in issues)
            {
                report += issue + "\n";
            }
            report += "\n";
            report += "Use Tools > Paper Trail > Configuration > Setup Complete Project Structure to fix these issues.";
        }
        else
        {
            report += "🎉 ALL SYSTEMS OPERATIONAL! 🎉\n";
            report += "Ready for bureaucratic mayhem!";
        }
        
        Debug.Log(report);
        
        string dialogTitle = issues.Count > 0 ? "Configuration Issues Found" : "Configuration Valid!";
        EditorUtility.DisplayDialog(dialogTitle, report, "OK");
    }
    
    private static void EnsureDirectoryExists(string path)
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
