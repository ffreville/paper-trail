using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Générateur de prefabs pour Paper Trail
/// </summary>
public static class PrefabGenerator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Paper Trail/5 - Create DocumentItem Prefab")]
    public static void CreateDocumentItemPrefab()
    {
        Debug.Log("Creating DocumentItem Prefab...");

        // Create temporary parent
        GameObject tempParent = new GameObject("TempParent");
        Canvas tempCanvas = tempParent.AddComponent<Canvas>();

        try
        {
            GameObject documentItem = CreateDocumentItemStructure(tempParent.transform);

            // Create the prefab
            bool success = CreatePrefabAsset(documentItem, "DocumentItem");

            if (success)
            {
                Debug.Log("DocumentItem prefab created successfully!");
                EditorUtility.DisplayDialog(
                    "DocumentItem Prefab Created",
                    "DocumentItem prefab saved to Assets/Prefabs/\n\nNext: Create InputField Prefab",
                    "OK"
                );
            }
        }
        finally
        {
            // Cleanup
            Object.DestroyImmediate(tempParent);
        }
    }

    [MenuItem("Tools/Paper Trail/6 - Create InputField Prefab")]
    public static void CreateInputFieldPrefab()
    {
        Debug.Log("Creating InputField Prefab...");

        // Create temporary parent
        GameObject tempParent = new GameObject("TempParent");
        Canvas tempCanvas = tempParent.AddComponent<Canvas>();

        try
        {
            GameObject inputField = CreateInputFieldStructure(tempParent.transform);

            // Create the prefab
            bool success = CreatePrefabAsset(inputField, "InputFieldPrefab");

            if (success)
            {
                Debug.Log("InputField prefab created successfully!");
                EditorUtility.DisplayDialog(
                    "InputField Prefab Created",
                    "InputField prefab saved to Assets/Prefabs/\n\nAll prefabs created! Now connect references.",
                    "OK"
                );
            }
        }
        finally
        {
            // Cleanup
            Object.DestroyImmediate(tempParent);
        }
    }

    [MenuItem("Tools/Paper Trail/7 - Create Both Prefabs")]
    public static void CreateBothPrefabs()
    {
        CreateDocumentItemPrefab();
        CreateInputFieldPrefab();

        EditorUtility.DisplayDialog(
            "All Prefabs Created",
            "Both DocumentItem and InputField prefabs created!\n\nYou can now connect all references in the Inspector.",
            "OK"
        );
    }

    private static GameObject CreateDocumentItemStructure(Transform parent)
    {
        // Create main DocumentItem object
        GameObject documentItem = new GameObject("DocumentItem", typeof(RectTransform));
        documentItem.transform.SetParent(parent, false);

        // Set size
        RectTransform mainRT = documentItem.GetComponent<RectTransform>();
        mainRT.sizeDelta = new Vector2(380, 80);

        // Add Button component
        Button btn = documentItem.AddComponent<Button>();
        Image mainImg = documentItem.AddComponent<Image>();
        mainImg.color = Color.white;

        // Button colors
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.94f, 0.94f, 0.94f, 1f);
        cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
        btn.colors = cb;

        // Add Horizontal Layout Group
        HorizontalLayoutGroup hlg = documentItem.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(10, 10, 5, 5);
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlHeight = true;
        hlg.childControlWidth = false;

        // Create StatusIcon
        CreateStatusIcon(documentItem.transform);

        // Create InfoPanel
        CreateInfoPanel(documentItem.transform);

        // Create StatusText
        CreateStatusText(documentItem.transform);

        // Add DocumentItemUI script
        DocumentItemUI itemUI = documentItem.AddComponent<DocumentItemUI>();

        // Connect references automatically
        ConnectDocumentItemReferences(itemUI, documentItem);

        Debug.Log("DocumentItem structure created");
        return documentItem;
    }

    private static void CreateStatusIcon(Transform parent)
    {
        GameObject statusIcon = new GameObject("StatusIcon", typeof(RectTransform));
        statusIcon.transform.SetParent(parent, false);

        // Add Image component
        Image img = statusIcon.AddComponent<Image>();
        img.color = Color.yellow; // Default pending color

        // Add Layout Element
        LayoutElement le = statusIcon.AddComponent<LayoutElement>();
        le.minWidth = 30;
        le.preferredWidth = 30;
        le.minHeight = 30;
        le.preferredHeight = 30;
        le.flexibleWidth = 0;
    }

    private static void CreateInfoPanel(Transform parent)
    {
        GameObject infoPanel = new GameObject("InfoPanel", typeof(RectTransform));
        infoPanel.transform.SetParent(parent, false);

        // Add transparent image
        Image img = infoPanel.AddComponent<Image>();
        img.color = Color.clear;

        // Add Vertical Layout Group
        VerticalLayoutGroup vlg = infoPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 2;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;

        // Add Layout Element
        LayoutElement le = infoPanel.AddComponent<LayoutElement>();
        le.flexibleWidth = 1;

        // Create TitleText
        GameObject titleText = new GameObject("TitleText", typeof(RectTransform));
        titleText.transform.SetParent(infoPanel.transform, false);

        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "Document Title";
        titleTMP.fontSize = 14;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Left;
        titleTMP.color = Color.black;
        titleTMP.overflowMode = TextOverflowModes.Ellipsis;
        titleTMP.enableWordWrapping = false;

        // Create CitizenText
        GameObject citizenText = new GameObject("CitizenText", typeof(RectTransform));
        citizenText.transform.SetParent(infoPanel.transform, false);

        TextMeshProUGUI citizenTMP = citizenText.AddComponent<TextMeshProUGUI>();
        citizenTMP.text = "From: Citizen Name";
        citizenTMP.fontSize = 12;
        citizenTMP.alignment = TextAlignmentOptions.Left;
        citizenTMP.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        citizenTMP.overflowMode = TextOverflowModes.Ellipsis;
        citizenTMP.enableWordWrapping = false;
    }

    private static void CreateStatusText(Transform parent)
    {
        GameObject statusText = new GameObject("StatusText", typeof(RectTransform));
        statusText.transform.SetParent(parent, false);

        TextMeshProUGUI statusTMP = statusText.AddComponent<TextMeshProUGUI>();
        statusTMP.text = "Pending";
        statusTMP.fontSize = 11;
        statusTMP.alignment = TextAlignmentOptions.Right; // Correction
        statusTMP.color = new Color(0.27f, 0.27f, 0.27f, 1f);

        // Add Layout Element
        LayoutElement le = statusText.AddComponent<LayoutElement>();
        le.minWidth = 80;
        le.preferredWidth = 80;
        le.flexibleWidth = 0;
    }

    private static void ConnectDocumentItemReferences(DocumentItemUI itemUI, GameObject documentItem)
    {
        // Find and connect all references
        itemUI.titleText = documentItem.transform.Find("InfoPanel/TitleText").GetComponent<TextMeshProUGUI>();
        itemUI.citizenText = documentItem.transform.Find("InfoPanel/CitizenText").GetComponent<TextMeshProUGUI>();
        itemUI.statusText = documentItem.transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
        itemUI.statusIcon = documentItem.transform.Find("StatusIcon").GetComponent<Image>();
        itemUI.selectButton = documentItem.GetComponent<Button>();

        Debug.Log("DocumentItem references connected automatically");
    }

    private static GameObject CreateInputFieldStructure(Transform parent)
    {
        // Create main InputField object
        GameObject inputFieldPrefab = new GameObject("InputFieldPrefab", typeof(RectTransform));
        inputFieldPrefab.transform.SetParent(parent, false);

        // Set size
        RectTransform mainRT = inputFieldPrefab.GetComponent<RectTransform>();
        mainRT.sizeDelta = new Vector2(350, 60);

        // Add background image
        Image mainImg = inputFieldPrefab.AddComponent<Image>();
        mainImg.color = new Color(0.98f, 0.98f, 0.98f, 1f);

        // Add Horizontal Layout Group
        HorizontalLayoutGroup hlg = inputFieldPrefab.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(5, 5, 5, 5);
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlHeight = true;
        hlg.childControlWidth = false;

        // Create Label
        CreateFieldLabel(inputFieldPrefab.transform);

        // Create InputField
        CreateTMPInputField(inputFieldPrefab.transform);

        Debug.Log("InputField structure created");
        return inputFieldPrefab;
    }

    private static void CreateFieldLabel(Transform parent)
    {
        GameObject label = new GameObject("Label", typeof(RectTransform));
        label.transform.SetParent(parent, false);

        TextMeshProUGUI labelTMP = label.AddComponent<TextMeshProUGUI>();
        labelTMP.text = "Field Name:";
        labelTMP.fontSize = 14;
        labelTMP.alignment = TextAlignmentOptions.Left; // Correction
        labelTMP.color = Color.black;

        // Add Layout Element
        LayoutElement le = label.AddComponent<LayoutElement>();
        le.minWidth = 120;
        le.preferredWidth = 120;
        le.flexibleWidth = 0;
    }

    private static void CreateTMPInputField(Transform parent)
    {
        GameObject inputField = new GameObject("InputField", typeof(RectTransform));
        inputField.transform.SetParent(parent, false);

        // Add TMP_InputField component
        TMP_InputField tmpInput = inputField.AddComponent<TMP_InputField>();

        // Add background image
        Image inputImg = inputField.AddComponent<Image>();
        inputImg.color = Color.white;
        inputImg.type = Image.Type.Sliced;

        // Add Layout Element
        LayoutElement le = inputField.AddComponent<LayoutElement>();
        le.minWidth = 200;
        le.flexibleWidth = 1;

        // Create Text Area
        GameObject textArea = new GameObject("Text Area", typeof(RectTransform));
        textArea.transform.SetParent(inputField.transform, false);
        textArea.AddComponent<RectMask2D>();

        RectTransform textAreaRT = textArea.GetComponent<RectTransform>();
        textAreaRT.anchorMin = Vector2.zero;
        textAreaRT.anchorMax = Vector2.one;
        textAreaRT.offsetMin = new Vector2(10, 6);
        textAreaRT.offsetMax = new Vector2(-10, -7);

        // Create Text component
        GameObject textComponent = new GameObject("Text", typeof(RectTransform));
        textComponent.transform.SetParent(textArea.transform, false);

        TextMeshProUGUI textTMP = textComponent.AddComponent<TextMeshProUGUI>();
        textTMP.text = "";
        textTMP.fontSize = 14;
        textTMP.color = Color.black;
        textTMP.alignment = TextAlignmentOptions.Left; // Correction
        textTMP.enableWordWrapping = false;
        textTMP.overflowMode = TextOverflowModes.Ellipsis;

        RectTransform textRT = textComponent.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        // Create Placeholder
        GameObject placeholder = new GameObject("Placeholder", typeof(RectTransform));
        placeholder.transform.SetParent(inputField.transform, false);

        TextMeshProUGUI placeholderTMP = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderTMP.text = "Enter text...";
        placeholderTMP.fontSize = 14;
        placeholderTMP.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderTMP.alignment = TextAlignmentOptions.Left; // Correction
        placeholderTMP.enableWordWrapping = false;

        RectTransform placeholderRT = placeholder.GetComponent<RectTransform>();
        placeholderRT.anchorMin = Vector2.zero;
        placeholderRT.anchorMax = Vector2.one;
        placeholderRT.offsetMin = new Vector2(10, 6);
        placeholderRT.offsetMax = new Vector2(-10, -7);

        // Configure TMP_InputField
        tmpInput.textComponent = textTMP;
        tmpInput.placeholder = placeholderTMP;
        tmpInput.lineType = TMP_InputField.LineType.SingleLine;
        tmpInput.characterLimit = 0;
    }

    private static bool CreatePrefabAsset(GameObject prefabObject, string prefabName)
    {
        try
        {
            // Create Prefabs folder if it doesn't exist
            string prefabsPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabsPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // Create the prefab
            string prefabPath = $"{prefabsPath}/{prefabName}.prefab";

            // Remove existing prefab if it exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
            }

            // Create new prefab
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefabObject, prefabPath);

            if (prefabAsset != null)
            {
                Debug.Log($"Prefab saved: {prefabPath}");

                // Refresh and select the prefab
                AssetDatabase.Refresh();
                Selection.activeObject = prefabAsset;
                EditorGUIUtility.PingObject(prefabAsset);

                return true;
            }
            else
            {
                Debug.LogError($"Failed to create prefab: {prefabName}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating prefab {prefabName}: {e.Message}");
            return false;
        }
    }
#endif
}