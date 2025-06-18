using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Générateur pour le panel Document
/// </summary>
public static class DocumentGenerator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Paper Trail/3 - Generate Document Panel")]
    public static void GenerateDocumentPanel()
    {
        Debug.Log("Step 3: Creating Document Panel...");

        // Find MainPanel
        GameObject mainPanel = GameObject.Find("MainPanel");
        if (mainPanel == null)
        {
            EditorUtility.DisplayDialog(
                "MainPanel Not Found",
                "Please run Step 1 first (Generate Canvas and Main Panel)",
                "OK"
            );
            return;
        }

        // Create Document Panel
        CreateDocumentPanel(mainPanel);

        Debug.Log("Step 3 Complete! Document Panel created.");
        Debug.Log("Next: Tools → Paper Trail → 4 - Generate Game UI");

        EditorUtility.DisplayDialog(
            "Step 3 Complete",
            "Document Panel created!\n\nNext: Generate Game UI (Step 4)",
            "OK"
        );
    }

    private static void CreateDocumentPanel(GameObject parent)
    {
        GameObject documentPanel = new GameObject("DocumentPanel", typeof(RectTransform));
        documentPanel.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = documentPanel.AddComponent<Image>();
        img.color = Color.white;

        // Position: Right side with margin
        RectTransform rt = documentPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-420, 0); // 400px width + 20px margin
        rt.sizeDelta = new Vector2(380, 0);

        // Create components
        CreateDocumentHeader(documentPanel);
        CreateDocumentBody(documentPanel);
        CreateActionButtons(documentPanel);

        // Add DocumentUI script component
        documentPanel.AddComponent<DocumentUI>();

        Debug.Log("Document Panel created with all components!");
    }

    private static void CreateDocumentHeader(GameObject parent)
    {
        GameObject header = new GameObject("DocumentHeader", typeof(RectTransform));
        header.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = header.AddComponent<Image>();
        img.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        // Position at top
        RectTransform rt = header.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, -60);
        rt.sizeDelta = new Vector2(0, 120);

        // Add Layout Group
        VerticalLayoutGroup vlg = header.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.padding = new RectOffset(15, 15, 10, 10);
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        // Create header texts
        CreateHeaderText(header, "DocumentTitleText", "Select a document", 18, FontStyles.Bold);
        CreateHeaderText(header, "CitizenNameText", "", 14, FontStyles.Normal);
        CreateHeaderText(header, "DocumentTypeText", "", 14, FontStyles.Normal);
        CreateHeaderText(header, "StatusText", "", 14, FontStyles.Normal);

        Debug.Log("Document Header created!");
    }

    private static void CreateHeaderText(GameObject parent, string name, string text, float fontSize, FontStyles style)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform));
        textObj.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.black;
    }

    private static void CreateDocumentBody(GameObject parent)
    {
        GameObject body = new GameObject("DocumentBody", typeof(RectTransform));
        body.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = body.AddComponent<Image>();
        img.color = new Color(0.98f, 0.98f, 0.98f, 1f);

        // Position between header and buttons
        RectTransform rt = body.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 80); // Bottom space for buttons
        rt.offsetMax = new Vector2(0, -120); // Top space for header

        // Create Details Text
        GameObject detailsObj = new GameObject("DetailsText", typeof(RectTransform));
        detailsObj.transform.SetParent(body.transform, false);

        TextMeshProUGUI details = detailsObj.AddComponent<TextMeshProUGUI>();
        details.text = "Select a document to view details";
        details.fontSize = 14;
        details.alignment = TextAlignmentOptions.TopLeft;
        details.enableWordWrapping = true;
        details.color = Color.black;

        RectTransform detailsRT = detailsObj.GetComponent<RectTransform>();
        detailsRT.anchorMin = new Vector2(0, 1);
        detailsRT.anchorMax = new Vector2(1, 1);
        detailsRT.anchoredPosition = new Vector2(0, -50);
        detailsRT.sizeDelta = new Vector2(-20, 80);

        // Create Form Fields ScrollView
        CreateFormFieldsScrollView(body);

        Debug.Log("Document Body created!");
    }

    private static void CreateFormFieldsScrollView(GameObject parent)
    {
        GameObject scrollView = new GameObject("FormFieldsScrollView", typeof(RectTransform));
        scrollView.transform.SetParent(parent.transform, false);

        // Position below details text
        RectTransform rt = scrollView.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(10, 10);
        rt.offsetMax = new Vector2(-10, -90); // Space for details text

        // Add ScrollRect component
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<Image>().color = Color.clear;

        // Create Viewport
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

        // Create Content (FormFieldsParent)
        GameObject content = new GameObject("FormFieldsParent", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);

        // Add Layout components
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
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

        // Configure ScrollRect
        scrollRect.content = contentRT;
        scrollRect.viewport = viewportRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        Debug.Log("Form Fields ScrollView created!");
    }

    private static void CreateActionButtons(GameObject parent)
    {
        GameObject actionButtons = new GameObject("ActionButtons", typeof(RectTransform));
        actionButtons.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = actionButtons.AddComponent<Image>();
        img.color = new Color(0.96f, 0.96f, 0.96f, 1f);

        // Position at bottom
        RectTransform rt = actionButtons.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(0, 40);
        rt.sizeDelta = new Vector2(0, 80);

        // Add Layout Group
        HorizontalLayoutGroup hlg = actionButtons.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(20, 20, 10, 10);
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = false;

        // Create buttons
        CreateActionButton(actionButtons, "ProcessButton", "PROCESS", new Color(0.3f, 0.69f, 0.31f, 1f));
        CreateActionButton(actionButtons, "RejectButton", "REJECT", new Color(0.96f, 0.26f, 0.21f, 1f));
        CreateActionButton(actionButtons, "StampButton", "STAMP", new Color(0.13f, 0.59f, 0.95f, 1f));
        CreateActionButton(actionButtons, "SignButton", "SIGN", new Color(1f, 0.6f, 0f, 1f));

        Debug.Log("Action Buttons created!");
    }

    private static void CreateActionButton(GameObject parent, string name, string text, Color color)
    {
        GameObject buttonObj = new GameObject(name, typeof(RectTransform));
        buttonObj.transform.SetParent(parent.transform, false);

        // Add Button and Image components
        Button btn = buttonObj.AddComponent<Button>();
        Image img = buttonObj.AddComponent<Image>();
        img.color = color;

        // Set button colors
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
        cb.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        btn.colors = cb;

        // Create Button text
        GameObject textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }
#endif
}