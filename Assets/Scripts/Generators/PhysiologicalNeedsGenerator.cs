#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public static class PhysiologicalNeedsGenerator
{
    [MenuItem("Tools/Paper Trail/Generate Physiological Needs System")]
    public static void GeneratePhysiologicalNeedsSystem()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Generate Physiological Needs System",
            "This will add the physiological needs system to your Paper Trail project.\n\n" +
            "Features:\n" +
            "• Hunger, Thirst, Bladder, Energy, Stress bars\n" +
            "• Action buttons (Toilet, Lunch, Coffee, Rest)\n" +
            "• Emergency notifications\n" +
            "• Time display and work status\n" +
            "• Visual and audio effects\n" +
            "• Bureaucratic consequences for all actions!\n\n" +
            "The system will be integrated with your existing UI.",
            "Generate System",
            "Cancel"
        );

        if (!confirm) return;

        Debug.Log("=== GENERATING PHYSIOLOGICAL NEEDS SYSTEM ===");

        CreatePhysiologicalNeedsManager();
        CreateNeedsUI();
        IntegrateWithExistingUI();
        
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Physiological Needs System Generated!",
            "The physiological needs system has been successfully added!\n\n" +
            "Features added:\n" +
            "✅ PhysiologicalNeedsManager\n" +
            "✅ Needs display UI with bars and icons\n" +
            "✅ Action buttons with bureaucratic consequences\n" +
            "✅ Emergency notification system\n" +
            "✅ Time and work status display\n" +
            "✅ Integration with existing Paper Trail systems\n\n" +
            "Now your employees must manage their basic needs while drowning in paperwork!",
            "Excellent!"
        );

        Debug.Log("=== PHYSIOLOGICAL NEEDS SYSTEM COMPLETE ===");
    }

    private static void CreatePhysiologicalNeedsManager()
    {
        Debug.Log("Creating PhysiologicalNeedsManager...");

        // Check if already exists
        PhysiologicalNeedsManager existingManager = Object.FindObjectOfType<PhysiologicalNeedsManager>();
        if (existingManager != null)
        {
            Debug.Log("PhysiologicalNeedsManager already exists, skipping creation.");
            return;
        }

        // Create new GameObject for the manager
        GameObject managerGO = new GameObject("PhysiologicalNeedsManager");
        PhysiologicalNeedsManager manager = managerGO.AddComponent<PhysiologicalNeedsManager>();

        // Configure default settings
        manager.enableBureaucraticToilets = true;
        manager.enableLunchBreakDocuments = true;
        manager.enableCoffeeBreakPermits = true;
        manager.workDayStartHour = 8f;
        manager.workDayEndHour = 17f;
        manager.lunchBreakStart = 12f;
        manager.lunchBreakDuration = 1f;

        Debug.Log("✅ PhysiologicalNeedsManager created and configured");
    }

    private static void CreateNeedsUI()
    {
        Debug.Log("Creating Physiological Needs UI...");

        // Find Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "No Canvas found! Please create a Canvas first.", "OK");
            return;
        }

        // Create main needs panel
        GameObject needsPanel = CreateNeedsPanel(canvas);
        
        // Create needs bars
        CreateNeedsBars(needsPanel);
        
        // Create action buttons
        CreateActionButtons(needsPanel);
        
        // Create emergency notification
        CreateEmergencyNotification(canvas);
        
        // Create time display
        CreateTimeDisplay(needsPanel);

        // Add PhysiologicalNeedsUI component
        PhysiologicalNeedsUI needsUI = needsPanel.AddComponent<PhysiologicalNeedsUI>();
        ConnectUIReferences(needsUI, needsPanel);

        Debug.Log("✅ Physiological Needs UI created");
    }

    private static GameObject CreateNeedsPanel(Canvas canvas)
    {
        GameObject needsPanel = new GameObject("PhysiologicalNeedsPanel", typeof(RectTransform));
        needsPanel.transform.SetParent(canvas.transform, false);

        // Add background
        Image bg = needsPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Position: Left side of screen
        RectTransform rt = needsPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(150, 0);
        rt.sizeDelta = new Vector2(280, 0);

        // Add layout group
        VerticalLayoutGroup vlg = needsPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.padding = new RectOffset(15, 15, 15, 15);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        return needsPanel;
    }

    private static void CreateNeedsBars(GameObject parent)
    {
        // Title
        CreateLabel(parent, "BESOINS PHYSIOLOGIQUES", 16, FontStyles.Bold, Color.white);

        // Hunger bar
        CreateNeedBar(parent, "Faim", "🍽️", Color.green, "HungerBar");
        
        // Thirst bar
        CreateNeedBar(parent, "Soif", "💧", Color.blue, "ThirstBar");
        
        // Bladder bar
        CreateNeedBar(parent, "Vessie", "🚽", Color.yellow, "BladderBar");
        
        // Energy bar
        CreateNeedBar(parent, "Énergie", "⚡", Color.cyan, "EnergyBar");
        
        // Stress bar
        CreateNeedBar(parent, "Stress", "😰", Color.red, "StressBar");
    }

    private static void CreateNeedBar(GameObject parent, string labelText, string emoji, Color barColor, string barName)
    {
        GameObject barContainer = new GameObject(barName + "Container", typeof(RectTransform));
        barContainer.transform.SetParent(parent.transform, false);

        // Set height
        RectTransform containerRT = barContainer.GetComponent<RectTransform>();
        containerRT.sizeDelta = new Vector2(0, 40);

        // Add horizontal layout
        HorizontalLayoutGroup hlg = barContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandHeight = true;

        // Create icon
        GameObject iconObj = new GameObject("Icon", typeof(RectTransform));
        iconObj.transform.SetParent(barContainer.transform, false);
        
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.text = emoji;
        iconText.fontSize = 20;
        iconText.alignment = TextAlignmentOptions.Center;
        
        RectTransform iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.sizeDelta = new Vector2(30, 30);
        
        LayoutElement iconLE = iconObj.AddComponent<LayoutElement>();
        iconLE.minWidth = 30;
        iconLE.preferredWidth = 30;

        // Create label
        GameObject labelObj = new GameObject("Label", typeof(RectTransform));
        labelObj.transform.SetParent(barContainer.transform, false);
        
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 12;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelLE = labelObj.AddComponent<LayoutElement>();
        labelLE.minWidth = 50;
        labelLE.preferredWidth = 50;

        // Create slider (bar)
        GameObject sliderObj = new GameObject(barName, typeof(RectTransform));
        sliderObj.transform.SetParent(barContainer.transform, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.interactable = false;

        // Slider background
        Image sliderBG = sliderObj.AddComponent<Image>();
        sliderBG.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Create fill area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillAreaRT = fillArea.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = Vector2.zero;
        fillAreaRT.offsetMax = Vector2.zero;

        // Create fill
        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = barColor;
        
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        // Connect slider
        slider.fillRect = fillRT;

        // Add layout element to slider
        LayoutElement sliderLE = sliderObj.AddComponent<LayoutElement>();
        sliderLE.flexibleWidth = 1;
    }

    private static void CreateActionButtons(GameObject parent)
    {
        // Spacer
        CreateSpacer(parent, 20);

        // Actions title
        CreateLabel(parent, "ACTIONS", 14, FontStyles.Bold, Color.yellow);

        // Toilet button
        CreateActionButton(parent, "TOILETTES\n(Urgent)", "ToiletButton", new Color(1f, 0.6f, 0.2f));

        // Lunch button
        CreateActionButton(parent, "DÉJEUNER\n(12h-13h)", "LunchButton", new Color(0.3f, 0.7f, 0.3f));

        // Coffee button
        CreateActionButton(parent, "CAFÉ\n(Caféine)", "CoffeeButton", new Color(0.6f, 0.3f, 0.1f));

        // Rest button
        CreateActionButton(parent, "REPOS\n(Risqué)", "RestButton", new Color(0.4f, 0.4f, 0.7f));
    }

    private static void CreateActionButton(GameObject parent, string buttonText, string buttonName, Color buttonColor)
    {
        GameObject buttonObj = new GameObject(buttonName, typeof(RectTransform));
        buttonObj.transform.SetParent(parent.transform, false);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonColor;

        // Configure button colors
        ColorBlock cb = button.colors;
        cb.normalColor = buttonColor;
        cb.highlightedColor = Color.Lerp(buttonColor, Color.white, 0.2f);
        cb.pressedColor = Color.Lerp(buttonColor, Color.black, 0.2f);
        cb.disabledColor = Color.gray;
        button.colors = cb;

        // Set button size
        RectTransform buttonRT = buttonObj.GetComponent<RectTransform>();
        buttonRT.sizeDelta = new Vector2(0, 50);

        // Create button text
        GameObject textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI buttonTextComponent = textObj.AddComponent<TextMeshProUGUI>();
        buttonTextComponent.text = buttonText;
        buttonTextComponent.fontSize = 11;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;
        buttonTextComponent.color = Color.white;
        buttonTextComponent.fontStyle = FontStyles.Bold;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }

    private static void CreateTimeDisplay(GameObject parent)
    {
        // Spacer
        CreateSpacer(parent, 20);

        // Time title
        CreateLabel(parent, "HORLOGE ADMINISTRATIVE", 12, FontStyles.Bold, Color.cyan);

        // Current time
        CreateLabel(parent, "08:00", 18, FontStyles.Bold, Color.white, "TimeText");

        // Work status
        CreateLabel(parent, "HEURES DE TRAVAIL", 10, FontStyles.Normal, Color.white, "WorkStatusText");
    }

    private static void CreateEmergencyNotification(Canvas canvas)
    {
        GameObject emergencyPanel = new GameObject("EmergencyNotificationPanel", typeof(RectTransform));
        emergencyPanel.transform.SetParent(canvas.transform, false);

        // Add background image
        Image bg = emergencyPanel.AddComponent<Image>();
        bg.color = new Color(1f, 0f, 0f, 0.9f);

        // Position: Center screen
        RectTransform rt = emergencyPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.2f, 0.3f);
        rt.anchorMax = new Vector2(0.8f, 0.7f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Add layout group
        VerticalLayoutGroup vlg = emergencyPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 20;
        vlg.padding = new RectOffset(30, 30, 30, 30);
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        // Emergency text
        GameObject textObj = new GameObject("EmergencyText", typeof(RectTransform));
        textObj.transform.SetParent(emergencyPanel.transform, false);

        TextMeshProUGUI emergencyText = textObj.AddComponent<TextMeshProUGUI>();
        emergencyText.text = "🚨 URGENCE PHYSIOLOGIQUE ! 🚨";
        emergencyText.fontSize = 20;
        emergencyText.fontStyle = FontStyles.Bold;
        emergencyText.alignment = TextAlignmentOptions.Center;
        emergencyText.color = Color.white;
        emergencyText.enableWordWrapping = true;

        // Close button
        GameObject closeButtonObj = new GameObject("EmergencyCloseButton", typeof(RectTransform));
        closeButtonObj.transform.SetParent(emergencyPanel.transform, false);

        Button closeButton = closeButtonObj.AddComponent<Button>();
        Image closeButtonImage = closeButtonObj.AddComponent<Image>();
        closeButtonImage.color = new Color(0.8f, 0.8f, 0.8f);

        RectTransform closeButtonRT = closeButtonObj.GetComponent<RectTransform>();
        closeButtonRT.sizeDelta = new Vector2(150, 40);

        // Close button text
        GameObject closeTextObj = new GameObject("Text", typeof(RectTransform));
        closeTextObj.transform.SetParent(closeButtonObj.transform, false);

        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "COMPRIS !";
        closeText.fontSize = 14;
        closeText.fontStyle = FontStyles.Bold;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.black;

        RectTransform closeTextRT = closeTextObj.GetComponent<RectTransform>();
        closeTextRT.anchorMin = Vector2.zero;
        closeTextRT.anchorMax = Vector2.one;
        closeTextRT.offsetMin = Vector2.zero;
        closeTextRT.offsetMax = Vector2.zero;

        // Hide by default
        emergencyPanel.SetActive(false);
    }

    private static void CreateLabel(GameObject parent, string text, float fontSize, FontStyles fontStyle, Color color, string objectName = null)
    {
        string name = objectName ?? "Label_" + text.Replace(" ", "");
        GameObject labelObj = new GameObject(name, typeof(RectTransform));
        labelObj.transform.SetParent(parent.transform, false);

        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;

        // Set height based on font size
        RectTransform labelRT = labelObj.GetComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(0, fontSize + 5);
    }

    private static void CreateSpacer(GameObject parent, float height)
    {
        GameObject spacer = new GameObject("Spacer", typeof(RectTransform));
        spacer.transform.SetParent(parent.transform, false);

        RectTransform spacerRT = spacer.GetComponent<RectTransform>();
        spacerRT.sizeDelta = new Vector2(0, height);
    }

    private static void ConnectUIReferences(PhysiologicalNeedsUI needsUI, GameObject needsPanel)
    {
        // Find and connect slider references
        needsUI.hungerBar = FindChildByName<Slider>(needsPanel, "HungerBar");
        needsUI.thirstBar = FindChildByName<Slider>(needsPanel, "ThirstBar");
        needsUI.bladderBar = FindChildByName<Slider>(needsPanel, "BladderBar");
        needsUI.energyBar = FindChildByName<Slider>(needsPanel, "EnergyBar");
        needsUI.stressBar = FindChildByName<Slider>(needsPanel, "StressBar");

        // Find and connect button references
        needsUI.toiletButton = FindChildByName<Button>(needsPanel, "ToiletButton");
        needsUI.lunchButton = FindChildByName<Button>(needsPanel, "LunchButton");
        needsUI.coffeeButton = FindChildByName<Button>(needsPanel, "CoffeeButton");
        needsUI.restButton = FindChildByName<Button>(needsPanel, "RestButton");

        // Find time display references
        needsUI.timeText = FindChildByName<TextMeshProUGUI>(needsPanel, "TimeText");
        needsUI.workStatusText = FindChildByName<TextMeshProUGUI>(needsPanel, "WorkStatusText");

        // Find emergency notification references
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            needsUI.emergencyPanel = FindChildByName<GameObject>(canvas.gameObject, "EmergencyNotificationPanel");
            if (needsUI.emergencyPanel != null)
            {
                needsUI.emergencyText = FindChildByName<TextMeshProUGUI>(needsUI.emergencyPanel, "EmergencyText");
                needsUI.emergencyCloseButton = FindChildByName<Button>(needsUI.emergencyPanel, "EmergencyCloseButton");
            }
        }

        Debug.Log("✅ UI references connected");
    }

    private static T FindChildByName<T>(GameObject parent, string name) where T : Component
    {
        Transform found = FindChildRecursive(parent.transform, name);
        if (found != null)
        {
            if (typeof(T) == typeof(GameObject))
                return found.gameObject as T;
            else
                return found.GetComponent<T>();
        }
        return null;
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private static void IntegrateWithExistingUI()
    {
        Debug.Log("Integrating with existing Paper Trail UI...");

        // Find existing GameUI to add needs integration
        GameUI gameUI = Object.FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            // Add needs status to configuration panel
            AddNeedsStatusToGameUI(gameUI);
        }

        // Connect with DocumentUI for productivity penalties
        DocumentUI documentUI = Object.FindObjectOfType<DocumentUI>();
        if (documentUI != null)
        {
            AddProductivityIndicatorToDocumentUI(documentUI);
        }

        Debug.Log("✅ Integration with existing UI complete");
    }

    private static void AddNeedsStatusToGameUI(GameUI gameUI)
    {
        if (gameUI.configurationPanel == null) return;

        // Add needs status text to configuration panel
        GameObject needsStatusObj = new GameObject("NeedsStatusText", typeof(RectTransform));
        needsStatusObj.transform.SetParent(gameUI.configurationPanel.transform, false);

        TextMeshProUGUI needsStatus = needsStatusObj.AddComponent<TextMeshProUGUI>();
        needsStatus.text = "État Physiologique: Normal";
        needsStatus.fontSize = 12;
        needsStatus.color = Color.white;
        needsStatus.alignment = TextAlignmentOptions.Center;

        RectTransform needsStatusRT = needsStatusObj.GetComponent<RectTransform>();
        needsStatusRT.sizeDelta = new Vector2(300, 30);

        Debug.Log("✅ Needs status added to GameUI configuration panel");
    }

    private static void AddProductivityIndicatorToDocumentUI(DocumentUI documentUI)
    {
        // Find the main document panel
        Transform documentPanel = documentUI.transform;
        
        // Add productivity indicator
        GameObject productivityObj = new GameObject("ProductivityIndicator", typeof(RectTransform));
        productivityObj.transform.SetParent(documentPanel, false);

        // Position at top-right of document panel
        RectTransform productivityRT = productivityObj.GetComponent<RectTransform>();
        productivityRT.anchorMin = new Vector2(1, 1);
        productivityRT.anchorMax = new Vector2(1, 1);
        productivityRT.anchoredPosition = new Vector2(-80, -20);
        productivityRT.sizeDelta = new Vector2(150, 30);

        // Add background
        Image productivityBG = productivityObj.AddComponent<Image>();
        productivityBG.color = new Color(0, 0, 0, 0.5f);

        // Add text
        TextMeshProUGUI productivityText = productivityObj.AddComponent<TextMeshProUGUI>();
        productivityText.text = "Productivité: 100%";
        productivityText.fontSize = 10;
        productivityText.color = Color.green;
        productivityText.alignment = TextAlignmentOptions.Center;

        Debug.Log("✅ Productivity indicator added to DocumentUI");
    }

    [MenuItem("Tools/Paper Trail/Generate Toilet Documents Templates")]
    public static void GenerateToiletDocumentTemplates()
    {
        Debug.Log("Creating bureaucratic toilet document templates...");

        // Ensure templates directory exists
        string templatesPath = "Assets/PaperTrail/DocumentTemplates/PhysiologicalNeeds/";
        //BureaucracyConfigurationTools.EnsureDirectoryExists(templatesPath);

        CreateToiletPermitTemplate(templatesPath);
        CreateLunchPermitTemplate(templatesPath);
        CreateCoffeePermitTemplate(templatesPath);
        CreateRestPermitTemplate(templatesPath);
        CreateMedicalExaminationTemplate(templatesPath);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Physiological Document Templates Created!",
            "The following bureaucratic nightmare templates have been created:\n\n" +
            "✅ Toilet Permission Form (T-001)\n" +
            "✅ Lunch Authorization Form (ALM-003)\n" +
            "✅ Coffee Consumption Permit (CAFF-007)\n" +
            "✅ Rest Period Application (REP-009)\n" +
            "✅ Medical Examination Requirement (MED-101)\n\n" +
            "Each template includes absurd form fields and triggers even more documents!\n" +
            "Your employees will now need permission for basic human functions!",
            "Diabolical!"
        );
    }

    private static void CreateToiletPermitTemplate(string basePath)
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Toilet Permission Form T-001";
        template.frenchTitle = "Formulaire T-001 - Autorisation d'Utilisation des Sanitaires";
        template.documentType = DocumentType.CitizenComplaint;
        template.description = "Official authorization for temporary workstation abandonment for physiological purposes";
        template.frenchDescription = "Autorisation officielle d'abandon temporaire de poste pour besoins physiologiques";
        template.baseBureaucracyLevel = 3;
        template.requiresStamp = true;
        template.requiresSignature = true;
        template.processingTimeMinutes = 15; // 15 minutes to process a toilet request!

        // Absurd form fields
        template.formFields.Add(new FormField
        {
            fieldName = "Urgency Level",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Modérée (peut attendre 30 min)", 
                "Critique (peut attendre 10 min)", 
                "CATASTROPHIQUE (immédiat)" 
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Estimated Duration",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "En minutes (maximum autorisé: 5)",
            maxLength = 2,
            validationRule = "numeric"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Medical Justification",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Expliquez en détail la nécessité physiologique urgente et les conséquences médicales potentielles en cas de refus",
            maxLength = 500
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Previous Toilet Visits Today",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "Nombre de visites déjà effectuées aujourd'hui"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Liquid Consumption Declaration",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Listez tous les liquides consommés dans les 4 dernières heures avec volumes exacts"
        });

        // Bureaucratic cascade trigger
        BureaucracyTrigger supervisorApproval = new BureaucracyTrigger
        {
            triggerName = "Supervisor Approval Required",
            condition = TriggerCondition.Always,
            probability = 1f,
            newDocumentTypes = new string[] { "HRValidationForm" },
            bureaucracyScoreBonus = 75,
            triggerMessage = "Votre demande de toilettes doit être approuvée par votre superviseur direct !",
            canTriggerRecursively = true
        };
        template.triggers.Add(supervisorApproval);

        // Medical examination trigger for frequent requests
        BureaucracyTrigger medicalExam = new BureaucracyTrigger
        {
            triggerName = "Medical Examination Required",
            condition = TriggerCondition.FieldValue,
            conditionValue = "3", // If more than 3 visits
            probability = 0.8f,
            newDocumentTypes = new string[] { "MedicalCertificateRequest" },
            bureaucracyScoreBonus = 100,
            triggerMessage = "Plus de 3 visites de toilettes par jour nécessitent un examen médical approfondi !",
            canTriggerRecursively = true
        };
        template.triggers.Add(medicalExam);

        string path = basePath + "ToiletPermitTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        Debug.Log("✅ Toilet Permit Template created");
    }

    private static void CreateLunchPermitTemplate(string basePath)
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Lunch Authorization Form ALM-003";
        template.frenchTitle = "Formulaire ALM-003 - Autorisation de Sustentation Nutritionnelle";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Official permit for mid-day nutritional intake";
        template.frenchDescription = "Permis officiel d'ingestion nutritionnelle de mi-journée";
        template.baseBureaucracyLevel = 2;
        template.requiresStamp = false;
        template.requiresSignature = true;

        template.formFields.Add(new FormField
        {
            fieldName = "Meal Category",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Sandwich réglementaire (catégorie A)", 
                "Salade administrative (catégorie B)", 
                "Soupe bureaucratique (catégorie C)",
                "Repas libre (DÉROGATION REQUISE)"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Consumption Location",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Salle de pause autorisée", 
                "Bureau (dérogation spéciale)", 
                "Cafétéria municipale",
                "Extérieur (INTERDIT sauf urgence)"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Nutritional Necessity Justification",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Expliquez pourquoi votre organisme nécessite une nutrition à cette heure précise"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Colleague Coverage",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Nom du collègue qui surveillera votre bureau pendant votre absence"
        });

        string path = basePath + "LunchPermitTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        Debug.Log("✅ Lunch Permit Template created");
    }

    private static void CreateCoffeePermitTemplate(string basePath)
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Coffee Consumption Permit CAFF-007";
        template.frenchTitle = "Permis CAFF-007 - Autorisation de Consommation de Caféine";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Legal authorization for caffeine substance consumption during work hours";
        template.frenchDescription = "Autorisation légale de consommation de substance stimulante pendant les heures de travail";
        template.baseBureaucracyLevel = 4;
        template.requiresStamp = true;
        template.requiresSignature = true;

        template.formFields.Add(new FormField
        {
            fieldName = "Caffeine Type",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Café noir réglementaire (0.8% caféine max)", 
                "Thé administratif (0.4% caféine max)", 
                "Cappuccino (dérogation spéciale requise)",
                "Espresso (CONTRÔLÉ - licence requise)"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Addiction Level Declaration",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Aucune dépendance", 
                "Dépendance légère (1-2 cafés/jour)", 
                "Dépendance modérée (3-5 cafés/jour)",
                "Dépendance sévère (6+ cafés/jour - ALERTE MÉDICALE)"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Productivity Impact Analysis",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Démontrez mathématiquement comment la caféine améliorera votre productivité administrative"
        });

        // Medical certificate trigger for high addiction levels
        BureaucracyTrigger medicalCert = new BureaucracyTrigger
        {
            triggerName = "Addiction Medical Certificate",
            condition = TriggerCondition.FieldValue,
            conditionValue = "Dépendance sévère",
            probability = 1f,
            newDocumentTypes = new string[] { "MedicalCertificateRequest" },
            bureaucracyScoreBonus = 150,
            triggerMessage = "Dépendance sévère détectée ! Certificat médical obligatoire pour addiction à la caféine !",
            canTriggerRecursively = true
        };
        template.triggers.Add(medicalCert);

        string path = basePath + "CoffeePermitTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        Debug.Log("✅ Coffee Permit Template created");
    }

    private static void CreateRestPermitTemplate(string basePath)
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Rest Period Application REP-009";
        template.frenchTitle = "Demande REP-009 - Application de Période de Repos";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Formal request for temporary cessation of productivity";
        template.frenchDescription = "Demande formelle de cessation temporaire de productivité";
        template.baseBureaucracyLevel = 5;
        template.requiresStamp = true;
        template.requiresSignature = true;

        template.formFields.Add(new FormField
        {
            fieldName = "Fatigue Level Assessment",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Légèrement somnolent", 
                "Modérément épuisé", 
                "Sévèrement fatigué",
                "Comateux mais conscient"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Sleep Deprivation Cause",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Expliquez en détail pourquoi vous n'avez pas dormi suffisamment (justificatifs requis)"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Rest Duration Requested",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "Durée en minutes (maximum autorisé: 10)"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Productivity Recovery Guarantee",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Garantissez par écrit que ce repos améliorera votre rendement administratif"
        });

        string path = basePath + "RestPermitTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        Debug.Log("✅ Rest Permit Template created");
    }

    private static void CreateMedicalExaminationTemplate(string basePath)
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Medical Examination Requirement MED-101";
        template.frenchTitle = "Exigence MED-101 - Examen Médical Obligatoire";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Mandatory medical examination for excessive physiological needs";
        template.frenchDescription = "Examen médical obligatoire pour besoins physiologiques excessifs";
        template.baseBureaucracyLevel = 6;
        template.requiresStamp = true;
        template.requiresSignature = true;

        template.formFields.Add(new FormField
        {
            fieldName = "Examination Type Required",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Examen urologique complet", 
                "Analyse nutritionnelle approfondie", 
                "Évaluation de dépendance caféine",
                "Bilan fatigue chronique",
                "Examen psychiatrique (stress)"
            }
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Medical History Declaration",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Historique médical complet depuis la naissance (pages supplémentaires autorisées)"
        });

        template.formFields.Add(new FormField
        {
            fieldName = "Family Medical History",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Historique médical familial sur 3 générations minimum"
        });

        string path = basePath + "MedicalExaminationTemplate.asset";
        AssetDatabase.CreateAsset(template, path);
        Debug.Log("✅ Medical Examination Template created");
    }
}
#endif
