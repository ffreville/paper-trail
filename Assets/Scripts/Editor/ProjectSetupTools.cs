#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public static class ProjectSetupTools
{
    [MenuItem("Tools/Paper Trail/Setup Complete Project")]
    public static void SetupCompleteProject()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Setup Paper Trail Project",
            "This will setup the complete Paper Trail bureaucracy system.\n\n" +
            "Features to be created:\n" +
            "• Dynamic Configuration System\n" +
            "• Document Templates and Scenarios\n" +
            "• French Data Generator\n" +
            "• Physiological Needs System\n" +
            "• Natural Light Deprivation\n" +
            "• Complete UI System\n" +
            "• Sample scenarios and documents\n\n" +
            "This is the recommended setup for new projects.",
            "Setup Project",
            "Cancel"
        );

        if (!confirm) return;

        Debug.Log("=== SETTING UP PAPER TRAIL PROJECT ===");

        SetupProjectStructure();
        SetupManagerGameObjects();
        SetupUISystem();
        SetupConfigurationSystem();
        SetupPhysiologicalNeeds();
        CreateSampleContent();
        
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Project Setup Complete!",
            "Paper Trail has been completely set up!\n\n" +
            "✅ Dynamic Configuration System\n" +
            "✅ Manager GameObjects created\n" +
            "✅ Complete UI system\n" +
            "✅ Physiological needs (6 needs)\n" +
            "✅ Natural light deprivation\n" +
            "✅ Sample scenarios and templates\n" +
            "✅ French citizens database\n\n" +
            "Your bureaucratic nightmare is ready!",
            "Perfect!"
        );

        Debug.Log("=== PAPER TRAIL PROJECT SETUP COMPLETE ===");
    }

    private static void SetupProjectStructure()
    {
        Debug.Log("Setting up project structure...");

        // Create all necessary directories
        EnsureDirectoryExists("Assets/PaperTrail/");
        EnsureDirectoryExists("Assets/PaperTrail/DocumentTemplates/");
        EnsureDirectoryExists("Assets/PaperTrail/Scenarios/");
        EnsureDirectoryExists("Assets/PaperTrail/Data/");
        EnsureDirectoryExists("Assets/PaperTrail/Prefabs/");
        EnsureDirectoryExists("Assets/PaperTrail/Prefabs/FormFields/");
        EnsureDirectoryExists("Assets/PaperTrail/Resources/");

        Debug.Log("✅ Project structure created");
    }

    private static void SetupManagerGameObjects()
    {
        Debug.Log("Setting up manager GameObjects...");

        // Create GameManager
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManagerGO = new GameObject("GameManager");
            GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
            Debug.Log("✅ GameManager created");
        }

        // Create DocumentManager
        if (Object.FindObjectOfType<DocumentManager>() == null)
        {
            GameObject documentManagerGO = new GameObject("DocumentManager");
            DocumentManager docManager = documentManagerGO.AddComponent<DocumentManager>();
            Debug.Log("✅ DocumentManager created");
        }

        // Create BureaucracySystem
        if (Object.FindObjectOfType<BureaucracySystem>() == null)
        {
            GameObject bureaucracySystemGO = new GameObject("BureaucracySystem");
            BureaucracySystem bureaucracySystem = bureaucracySystemGO.AddComponent<BureaucracySystem>();
            Debug.Log("✅ BureaucracySystem created");
        }

        // Create DynamicConfigurationManager
        if (Object.FindObjectOfType<DynamicConfigurationManager>() == null)
        {
            GameObject configManagerGO = new GameObject("DynamicConfigurationManager");
            DynamicConfigurationManager configManager = configManagerGO.AddComponent<DynamicConfigurationManager>();
            configManager.useFrenchLocalization = true;
            configManager.documentGenerationInterval = 30f;
            configManager.maxSimultaneousDocuments = 10;
            Debug.Log("✅ DynamicConfigurationManager created");
        }

        // Connect references
        ConnectManagerReferences();
    }

    private static void ConnectManagerReferences()
    {
        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        DocumentManager docManager = Object.FindObjectOfType<DocumentManager>();
        BureaucracySystem bureaucracySystem = Object.FindObjectOfType<BureaucracySystem>();
        DynamicConfigurationManager configManager = Object.FindObjectOfType<DynamicConfigurationManager>();

        if (gameManager != null)
        {
            gameManager.documentManager = docManager;
            gameManager.bureaucracySystem = bureaucracySystem;
            gameManager.configurationManager = configManager;
            EditorUtility.SetDirty(gameManager);
        }

        if (docManager != null)
        {
            docManager.configurationManager = configManager;
            EditorUtility.SetDirty(docManager);
        }

        if (bureaucracySystem != null)
        {
            bureaucracySystem.documentManager = docManager;
            bureaucracySystem.configurationManager = configManager;
            EditorUtility.SetDirty(bureaucracySystem);
        }

        Debug.Log("✅ Manager references connected");
    }

    private static void SetupUISystem()
    {
        Debug.Log("Setting up UI system...");

        // Find Canvas or create one
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            CreateCanvasAndEventSystem();
        }

        // Generate UI components step by step
        CreateMainPanel();
        CreateInboxPanel();
        CreateDocumentPanel();
        CreateGameUI();
        CreateNotificationPanel();

        Debug.Log("✅ UI system created");
    }

    private static Canvas CreateCanvasAndEventSystem()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        // Canvas Scaler
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Graphic Raycaster
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create EventSystem
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        Debug.Log("Canvas and EventSystem created successfully!");
        return canvas;
    }

    private static void CreateMainPanel()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject mainPanel = new GameObject("MainPanel", typeof(RectTransform));
        mainPanel.transform.SetParent(canvas.transform, false);

        // Add Image component
        Image img = mainPanel.AddComponent<Image>();
        img.color = new Color(0.96f, 0.96f, 0.86f, 1f); // Beige paper color

        // Set to fill entire canvas
        RectTransform rt = mainPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Debug.Log("Main Panel created!");
    }

    private static void CreateInboxPanel()
    {
        GameObject mainPanel = GameObject.Find("MainPanel");
        if (mainPanel == null) return;

        GameObject inboxPanel = new GameObject("InboxPanel", typeof(RectTransform));
        inboxPanel.transform.SetParent(mainPanel.transform, false);

        // Add Image component
        Image img = inboxPanel.AddComponent<Image>();
        img.color = Color.white;

        // Position: Left side, 400px wide, with margin
        RectTransform rt = inboxPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(220, 0);
        rt.sizeDelta = new Vector2(400, 0);

        // Create inbox components
        CreateInboxHeader(inboxPanel);
        CreateInboxScrollView(inboxPanel);
        CreateInboxRefreshButton(inboxPanel);

        // Add InboxUI component
        InboxUI inboxUI = inboxPanel.AddComponent<InboxUI>();
        
        // Set up references
        inboxUI.documentListParent = inboxPanel.transform.Find("ScrollView/Viewport/DocumentListParent");
        inboxUI.inboxCountText = inboxPanel.transform.Find("Header/InboxCountText").GetComponent<TextMeshProUGUI>();
        inboxUI.refreshButton = inboxPanel.transform.Find("RefreshButton").GetComponent<Button>();

        Debug.Log("Inbox Panel created!");
    }

    private static void CreateInboxHeader(GameObject parent)
    {
        GameObject header = new GameObject("Header", typeof(RectTransform));
        header.transform.SetParent(parent.transform, false);

        Image img = header.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        RectTransform rt = header.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, -30);
        rt.sizeDelta = new Vector2(0, 60);

        // Create Title
        GameObject titleObj = new GameObject("Title", typeof(RectTransform));
        titleObj.transform.SetParent(header.transform, false);

        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "INBOX";
        title.fontSize = 24;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = Color.black;

        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 0);
        titleRT.anchorMax = new Vector2(0.7f, 1);
        titleRT.offsetMin = new Vector2(20, 0);
        titleRT.offsetMax = new Vector2(0, 0);

        // Create Count
        GameObject countObj = new GameObject("InboxCountText", typeof(RectTransform));
        countObj.transform.SetParent(header.transform, false);

        TextMeshProUGUI count = countObj.AddComponent<TextMeshProUGUI>();
        count.text = "Inbox (0)";
        count.fontSize = 14;
        count.alignment = TextAlignmentOptions.Center;
        count.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        RectTransform countRT = countObj.GetComponent<RectTransform>();
        countRT.anchorMin = new Vector2(0.7f, 0);
        countRT.anchorMax = new Vector2(1, 1);
        countRT.offsetMin = new Vector2(0, 0);
        countRT.offsetMax = new Vector2(-20, 0);
    }

    private static void CreateInboxScrollView(GameObject parent)
    {
        GameObject scrollView = new GameObject("ScrollView", typeof(RectTransform));
        scrollView.transform.SetParent(parent.transform, false);

        RectTransform rt = scrollView.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 80);
        rt.offsetMax = new Vector2(0, -60);

        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<Image>().color = new Color(1, 1, 1, 0.1f);

        // Viewport
        GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
        viewport.transform.SetParent(scrollView.transform, false);
        viewport.AddComponent<Image>().color = Color.clear;

        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        RectTransform viewportRT = viewport.GetComponent<RectTransform>();
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.offsetMin = Vector2.zero;
        viewportRT.offsetMax = Vector2.zero;

        // Content
        GameObject content = new GameObject("DocumentListParent", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 0);

        scrollRect.content = contentRT;
        scrollRect.viewport = viewportRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20f;
    }

    private static void CreateInboxRefreshButton(GameObject parent)
    {
        GameObject refreshBtn = new GameObject("RefreshButton", typeof(RectTransform));
        refreshBtn.transform.SetParent(parent.transform, false);

        Button btn = refreshBtn.AddComponent<Button>();
        Image img = refreshBtn.AddComponent<Image>();
        img.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        RectTransform rt = refreshBtn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(0, 40);
        rt.sizeDelta = new Vector2(-20, 60);

        GameObject textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(refreshBtn.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Refresh";
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }

    private static void CreateDocumentPanel()
    {
        GameObject mainPanel = GameObject.Find("MainPanel");
        if (mainPanel == null) return;

        GameObject documentPanel = new GameObject("DocumentPanel", typeof(RectTransform));
        documentPanel.transform.SetParent(mainPanel.transform, false);

        Image img = documentPanel.AddComponent<Image>();
        img.color = Color.white;

        RectTransform rt = documentPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-420, 0);
        rt.sizeDelta = new Vector2(380, 0);

        // Add DocumentUI component
        DocumentUI documentUI = documentPanel.AddComponent<DocumentUI>();

        Debug.Log("Document Panel created!");
    }

    private static void CreateGameUI()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject gameUI = new GameObject("GameUI", typeof(RectTransform));
        gameUI.transform.SetParent(canvas.transform, false);

        Image img = gameUI.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.5f);

        RectTransform rt = gameUI.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-170, -70);
        rt.sizeDelta = new Vector2(300, 120);

        VerticalLayoutGroup vlg = gameUI.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childAlignment = TextAnchor.UpperRight;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        // Create score texts
        CreateScoreText(gameUI, "BureaucracyScoreText", "Bureaucracy Score: 0", Color.white, FontStyles.Bold, 14);
        CreateScoreText(gameUI, "DocumentsProcessedText", "Documents Processed: 0", Color.yellow, FontStyles.Normal, 12);
        CreateScoreText(gameUI, "CitizensServedText", "Citizens Served: 0", Color.green, FontStyles.Normal, 12);

        // Add GameUI component
        GameUI gameUIComponent = gameUI.AddComponent<GameUI>();

        Debug.Log("Game UI created!");
    }

    private static void CreateScoreText(GameObject parent, string name, string text, Color color, FontStyles style, float fontSize)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform));
        textObj.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.color = color;

        RectTransform labelRT = textObj.GetComponent<RectTransform>();
        labelRT.sizeDelta = new Vector2(0, fontSize + 5);
    }

    private static void CreateNotificationPanel()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject notificationPanel = new GameObject("NotificationPanel", typeof(RectTransform));
        notificationPanel.transform.SetParent(canvas.transform, false);

        Image img = notificationPanel.AddComponent<Image>();
        img.color = new Color(1f, 0.6f, 0f, 0.9f);

        RectTransform rt = notificationPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -50);
        rt.sizeDelta = new Vector2(400, 80);

        // Hide by default
        notificationPanel.SetActive(false);

        Debug.Log("Notification Panel created!");
    }

    private static void SetupConfigurationSystem()
    {
        Debug.Log("Setting up configuration system...");

        // Create French data generator
        CreateFrenchDataGenerator();

        // Create sample document templates
        CreateSampleDocumentTemplates();

        // Create sample scenario
        CreateSampleScenario();

        Debug.Log("✅ Configuration system setup complete");
    }

    private static void CreateFrenchDataGenerator()
    {
        EnsureDirectoryExists("Assets/PaperTrail/Data/");

        FrenchDataGenerator generator = ScriptableObject.CreateInstance<FrenchDataGenerator>();
        generator.batchSize = 50;

        string path = "Assets/PaperTrail/Data/FrenchDataGenerator.asset";
        AssetDatabase.CreateAsset(generator, path);

        Debug.Log("✅ French Data Generator created");
    }

    private static void CreateSampleDocumentTemplates()
    {
        EnsureDirectoryExists("Assets/PaperTrail/DocumentTemplates/");

        // Create vacation request template
        DocumentTemplate vacationTemplate = ScriptableObject.CreateInstance<DocumentTemplate>();
        vacationTemplate.documentTitle = "Vacation Request";
        vacationTemplate.frenchTitle = "Demande de Congés Payés";
        vacationTemplate.documentType = DocumentType.VacationRequest;
        vacationTemplate.description = "Standard vacation request form";
        vacationTemplate.baseBureaucracyLevel = 1;
        vacationTemplate.requiresStamp = true;

        // Add form fields
        vacationTemplate.formFields.Add(new FormField
        {
            fieldName = "Employee Name",
            fieldType = FormFieldType.Text,
            isRequired = true,
            placeholder = "Full name"
        });

        vacationTemplate.formFields.Add(new FormField
        {
            fieldName = "Start Date",
            fieldType = FormFieldType.Date,
            isRequired = true,
            placeholder = "DD/MM/YYYY"
        });

        vacationTemplate.formFields.Add(new FormField
        {
            fieldName = "End Date",
            fieldType = FormFieldType.Date,
            isRequired = true,
            placeholder = "DD/MM/YYYY"
        });

        string templatePath = "Assets/PaperTrail/DocumentTemplates/VacationRequestTemplate.asset";
        AssetDatabase.CreateAsset(vacationTemplate, templatePath);

        Debug.Log("✅ Sample document templates created");
    }

    private static void CreateSampleScenario()
    {
        EnsureDirectoryExists("Assets/PaperTrail/Scenarios/");

        BureaucracyScenario scenario = ScriptableObject.CreateInstance<BureaucracyScenario>();
        scenario.scenarioName = "Basic Bureaucracy Training";
        scenario.scenarioDescription = "Introduction to bureaucratic procedures";
        scenario.difficultyLevel = 1;
        scenario.targetDocumentsProcessed = 5;
        scenario.targetBureaucracyScore = 500;
        scenario.timeLimit = 300f;

        // Add some French citizens
        FrenchDataGenerator generator = AssetDatabase.LoadAssetAtPath<FrenchDataGenerator>("Assets/PaperTrail/Data/FrenchDataGenerator.asset");
        if (generator != null)
        {
            for (int i = 0; i < 10; i++)
            {
                scenario.citizenDatabase.Add(generator.GenerateRandomCitizen());
            }
        }

        string scenarioPath = "Assets/PaperTrail/Scenarios/BasicScenario.asset";
        AssetDatabase.CreateAsset(scenario, scenarioPath);

        Debug.Log("✅ Sample scenario created");
    }

    private static void SetupPhysiologicalNeeds()
    {
        Debug.Log("Setting up physiological needs system...");

        // Create PhysiologicalNeedsManager
        if (Object.FindObjectOfType<PhysiologicalNeedsManager>() == null)
        {
            GameObject needsManagerGO = new GameObject("PhysiologicalNeedsManager");
            PhysiologicalNeedsManager needsManager = needsManagerGO.AddComponent<PhysiologicalNeedsManager>();
            needsManager.enableBureaucraticToilets = true;
            needsManager.enableLunchBreakDocuments = true;
            needsManager.enableCoffeeBreakPermits = true;
            Debug.Log("✅ PhysiologicalNeedsManager created");
        }

        // Add Natural Light Extension
        PhysiologicalNeedsManager existingManager = Object.FindObjectOfType<PhysiologicalNeedsManager>();
        if (existingManager != null && existingManager.GetComponent<NaturalLightNeedExtension>() == null)
        {
            NaturalLightNeedExtension lightExtension = existingManager.gameObject.AddComponent<NaturalLightNeedExtension>();
            lightExtension.enableNaturalLightNeed = true;
            lightExtension.windowsAreBlocked = true;
            lightExtension.windowPermitsRequired = true;
            Debug.Log("✅ Natural Light Extension added");
        }

        Debug.Log("✅ Physiological needs system complete");
    }

    private static void CreateSampleContent()
    {
        Debug.Log("Creating sample content...");

        // Assign sample scenario to GameManager
        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.currentScenario == null)
        {
            BureaucracyScenario scenario = AssetDatabase.LoadAssetAtPath<BureaucracyScenario>("Assets/PaperTrail/Scenarios/BasicScenario.asset");
            if (scenario != null)
            {
                gameManager.currentScenario = scenario;
                EditorUtility.SetDirty(gameManager);
                Debug.Log("✅ Sample scenario assigned to GameManager");
            }
        }

        Debug.Log("✅ Sample content created");
    }

    [MenuItem("Tools/Paper Trail/Validate Project Setup")]
    public static void ValidateProjectSetup()
    {
        List<string> success = new List<string>();
        List<string> issues = new List<string>();

        // Check for managers
        if (Object.FindObjectOfType<GameManager>() != null)
            success.Add("✅ GameManager found");
        else
            issues.Add("❌ GameManager missing");

        if (Object.FindObjectOfType<DocumentManager>() != null)
            success.Add("✅ DocumentManager found");
        else
            issues.Add("❌ DocumentManager missing");

        if (Object.FindObjectOfType<BureaucracySystem>() != null)
            success.Add("✅ BureaucracySystem found");
        else
            issues.Add("❌ BureaucracySystem missing");

        if (Object.FindObjectOfType<DynamicConfigurationManager>() != null)
            success.Add("✅ DynamicConfigurationManager found");
        else
            issues.Add("❌ DynamicConfigurationManager missing");

        // Check for UI
        if (Object.FindObjectOfType<InboxUI>() != null)
            success.Add("✅ InboxUI found");
        else
            issues.Add("❌ InboxUI missing");

        if (Object.FindObjectOfType<DocumentUI>() != null)
            success.Add("✅ DocumentUI found");
        else
            issues.Add("❌ DocumentUI missing");

        if (Object.FindObjectOfType<GameUI>() != null)
            success.Add("✅ GameUI found");
        else
            issues.Add("❌ GameUI missing");

        // Check for physiological needs
        if (Object.FindObjectOfType<PhysiologicalNeedsManager>() != null)
            success.Add("✅ PhysiologicalNeedsManager found");
        else
            issues.Add("⚠️ PhysiologicalNeedsManager missing (optional)");

        // Check for assets
        string[] templateGuids = AssetDatabase.FindAssets("t:DocumentTemplate");
        if (templateGuids.Length > 0)
            success.Add($"✅ Found {templateGuids.Length} DocumentTemplate(s)");
        else
            issues.Add("❌ No DocumentTemplate assets found");

        string[] scenarioGuids = AssetDatabase.FindAssets("t:BureaucracyScenario");
        if (scenarioGuids.Length > 0)
            success.Add($"✅ Found {scenarioGuids.Length} BureaucracyScenario(s)");
        else
            issues.Add("❌ No BureaucracyScenario assets found");

        // Generate report
        string report = "=== PAPER TRAIL PROJECT VALIDATION ===\n\n";
        
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
            report += "Use Tools > Paper Trail > Setup Complete Project to fix these issues.";
        }
        else
        {
            report += "🎉 ALL SYSTEMS OPERATIONAL! 🎉\n";
            report += "Your bureaucratic nightmare is ready to unleash!";
        }

        Debug.Log(report);

        string dialogTitle = issues.Count > 0 ? "Project Issues Found" : "Project Setup Valid!";
        EditorUtility.DisplayDialog(dialogTitle, report, "OK");
    }

    [MenuItem("Tools/Paper Trail/Clean Legacy Files")]
    public static void CleanLegacyFiles()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Clean Legacy Files",
            "This will remove old Enhanced* files that are no longer needed.\n\n" +
            "Make sure you have backed up your project first!",
            "Clean Files",
            "Cancel"
        );

        if (!confirm) return;

        Debug.Log("=== CLEANING LEGACY FILES ===");

        // Find all Enhanced* files
        string[] allScripts = AssetDatabase.FindAssets("t:MonoScript");
        List<string> enhancedFiles = new List<string>();

        foreach (string guid in allScripts)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            if (fileName.StartsWith("Enhanced") && path.Contains("Scripts"))
            {
                enhancedFiles.Add(path);
            }
        }

        if (enhancedFiles.Count > 0)
        {
            string fileList = string.Join("\n• ", enhancedFiles);
            bool confirmDelete = EditorUtility.DisplayDialog(
                "Enhanced Files Found",
                $"Found {enhancedFiles.Count} Enhanced files:\n\n• {fileList}\n\nDelete these files?",
                "Delete",
                "Cancel"
            );

            if (confirmDelete)
            {
                int deletedCount = 0;
                foreach (string filePath in enhancedFiles)
                {
                    if (AssetDatabase.DeleteAsset(filePath))
                    {
                        Debug.Log($"Deleted: {filePath}");
                        deletedCount++;
                    }
                }

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog(
                    "Legacy Cleanup Complete",
                    $"Successfully deleted {deletedCount} Enhanced files.\n\n" +
                    "Your project now uses clean, non-prefixed class names!",
                    "Great!"
                );
            }
        }
        else
        {
            EditorUtility.DisplayDialog(
                "No Legacy Files Found",
                "No Enhanced* files found to clean up.\n\nYour project is already clean!",
                "Perfect!"
            );
        }

        Debug.Log("=== LEGACY CLEANUP COMPLETE ===");
    }

    [MenuItem("Tools/Paper Trail/Export Project Template")]
    public static void ExportProjectTemplate()
    {
        Debug.Log("Exporting Paper Trail project template...");

        // Create a comprehensive project export
        string exportData = GenerateProjectExportData();
        
        string path = EditorUtility.SaveFilePanel(
            "Export Paper Trail Template", 
            "", 
            "PaperTrailProject.json", 
            "json"
        );
        
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, exportData);
            
            EditorUtility.DisplayDialog(
                "Project Template Exported",
                $"Paper Trail project template exported to:\n{path}\n\n" +
                "This file contains all scenarios, templates, and configuration data.",
                "Excellent!"
            );
            
            Debug.Log($"Project template exported to: {path}");
        }
    }

    private static string GenerateProjectExportData()
    {
        var exportData = new
        {
            projectName = "Paper Trail - Bureaucracy Simulator",
            version = "1.0.0",
            exportDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            
            // Count assets
            documentTemplates = AssetDatabase.FindAssets("t:DocumentTemplate").Length,
            scenarios = AssetDatabase.FindAssets("t:BureaucracyScenario").Length,
            dataGenerators = AssetDatabase.FindAssets("t:FrenchDataGenerator").Length,
            
            // System status
            managers = new
            {
                gameManager = Object.FindObjectOfType<GameManager>() != null,
                documentManager = Object.FindObjectOfType<DocumentManager>() != null,
                bureaucracySystem = Object.FindObjectOfType<BureaucracySystem>() != null,
                configurationManager = Object.FindObjectOfType<DynamicConfigurationManager>() != null
            },
            
            systems = new
            {
                physiologicalNeeds = Object.FindObjectOfType<PhysiologicalNeedsManager>() != null,
                naturalLight = Object.FindObjectOfType<NaturalLightNeedExtension>() != null
            },
            
            ui = new
            {
                inboxUI = Object.FindObjectOfType<InboxUI>() != null,
                documentUI = Object.FindObjectOfType<DocumentUI>() != null,
                gameUI = Object.FindObjectOfType<GameUI>() != null,
                needsUI = Object.FindObjectOfType<PhysiologicalNeedsUI>() != null
            }
        };

        return JsonUtility.ToJson(exportData, true);
    }

    [MenuItem("Tools/Paper Trail/About")]
    public static void ShowAbout()
    {
        EditorUtility.DisplayDialog(
            "About Paper Trail",
            "Paper Trail - Bureaucracy Simulator\n" +
            "Version 1.0.0\n\n" +
            "A kafkaesque bureaucracy simulation where every human need\n" +
            "requires paperwork and every action generates more forms.\n\n" +
            "Features:\n" +
            "• Dynamic document template system\n" +
            "• 6 physiological needs (including natural light!)\n" +
            "• French citizen database with 1000+ combinations\n" +
            "• Bureaucratic cascade generation\n" +
            "• Seasonal depression simulation\n" +
            "• Window opening permits\n" +
            "• Complete UI generation tools\n\n" +
            "Created with Unity and powered by bureaucratic madness.\n\n" +
            "Remember: In Paper Trail, even breathing requires a permit!",
            "Close"
        );
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
