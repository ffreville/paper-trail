using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Générateur principal de Canvas pour Paper Trail
/// Tools → Paper Trail → Generate Canvas UI
/// </summary>
public static class CanvasGeneratorMain
{
#if UNITY_EDITOR
    [MenuItem("Tools/Paper Trail/1 - Generate Canvas and Main Panel")]
    public static void GenerateCanvasAndMain()
    {
        Debug.Log("Step 1: Creating Canvas and Main Panel...");

        // Clear existing UI if needed
        Canvas existingCanvas = Object.FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            bool clearExisting = EditorUtility.DisplayDialog(
                "Existing Canvas Found",
                "A Canvas already exists. Do you want to replace it?",
                "Replace",
                "Cancel"
            );

            if (clearExisting)
            {
                Object.DestroyImmediate(existingCanvas.gameObject);
                // Also clear EventSystem
                EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
                if (eventSystem != null) Object.DestroyImmediate(eventSystem.gameObject);
            }
            else
            {
                Debug.Log("Canvas generation cancelled.");
                return;
            }
        }

        // Create Canvas and EventSystem
        Canvas mainCanvas = CreateCanvasAndEventSystem();

        // Create Main Panel
        GameObject mainPanel = CreateMainPanel(mainCanvas);

        Debug.Log("Step 1 Complete! Canvas and Main Panel created.");
        Debug.Log("Next: Tools → Paper Trail → 2 - Generate Inbox Panel");

        EditorUtility.DisplayDialog(
            "Step 1 Complete",
            "Canvas and Main Panel created!\n\nNext: Generate Inbox Panel (Step 2)",
            "OK"
        );
    }

    [MenuItem("Tools/Paper Trail/Generate System GameObjects")]
    public static void GenerateSystemObjects()
    {
        Debug.Log("Creating System GameObjects...");

        // Create GameManager
        GameObject gameManagerGO = new GameObject("GameManager");
        gameManagerGO.AddComponent<GameManager>();

        // Create DocumentManager
        GameObject documentManagerGO = new GameObject("DocumentManager");
        documentManagerGO.AddComponent<DocumentManager>();

        // Create BureaucracySystem
        GameObject bureaucracySystemGO = new GameObject("BureaucracySystem");
        bureaucracySystemGO.AddComponent<BureaucracySystem>();

        // AJOUTER CETTE PARTIE:
        // Create DynamicConfigurationManager
        GameObject configManagerGO = new GameObject("DynamicConfigurationManager");
        DynamicConfigurationManager configManager = configManagerGO.AddComponent<DynamicConfigurationManager>();

        // Auto-assign references
        GameManager gameManager = gameManagerGO.GetComponent<GameManager>();
        DocumentManager documentManager = documentManagerGO.GetComponent<DocumentManager>();
        BureaucracySystem bureaucracySystem = bureaucracySystemGO.GetComponent<BureaucracySystem>();

        // Connect references
        gameManager.documentManager = documentManager;
        gameManager.bureaucracySystem = bureaucracySystem;
        bureaucracySystem.documentManager = documentManager;

        Debug.Log("System GameObjects created with DynamicConfigurationManager!");
        Debug.Log("Don't forget to assign currentScenario and dataGenerator in the inspector!");
    }

    // Option B: Menu séparé pour créer juste le Configuration Manager
    [MenuItem("Tools/Paper Trail/Create Configuration Manager")]
    public static void CreateConfigurationManager()
    {
        GameObject configManagerGO = new GameObject("DynamicConfigurationManager");
        DynamicConfigurationManager configManager = configManagerGO.AddComponent<DynamicConfigurationManager>();

        Debug.Log("DynamicConfigurationManager created!");
        Debug.Log("Assign a BureaucracyScenario and FrenchDataGenerator in the inspector.");

        Selection.activeGameObject = configManagerGO;
    }

    [MenuItem("Tools/Paper Trail/Clear All Paper Trail Objects")]
    public static void ClearAllObjects()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Clear All Objects",
            "This will delete all Paper Trail objects. Are you sure?",
            "Yes, Clear All",
            "Cancel"
        );

        if (!confirm) return;

        // Find and destroy all Paper Trail objects
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null) Object.DestroyImmediate(canvas.gameObject);

        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem != null) Object.DestroyImmediate(eventSystem.gameObject);

        GameManager gameManager = Object.FindObjectOfType<GameManager>();
        if (gameManager != null) Object.DestroyImmediate(gameManager.gameObject);

        DocumentManager documentManager = Object.FindObjectOfType<DocumentManager>();
        if (documentManager != null) Object.DestroyImmediate(documentManager.gameObject);

        BureaucracySystem bureaucracySystem = Object.FindObjectOfType<BureaucracySystem>();
        if (bureaucracySystem != null) Object.DestroyImmediate(bureaucracySystem.gameObject);

        Debug.Log("All Paper Trail objects cleared!");
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
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        Debug.Log("Canvas and EventSystem created successfully!");
        return canvas;
    }

    private static GameObject CreateMainPanel(Canvas canvas)
    {
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

        Debug.Log("Main Panel created successfully!");
        return mainPanel;
    }
#endif
}