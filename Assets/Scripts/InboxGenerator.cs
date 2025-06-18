using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Générateur pour le panel Inbox
/// </summary>
public static class InboxGenerator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Paper Trail/2 - Generate Inbox Panel")]
    public static void GenerateInboxPanel()
    {
        Debug.Log("Step 2: Creating Inbox Panel...");

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

        // Create Inbox Panel
        CreateInboxPanel(mainPanel);

        Debug.Log("Step 2 Complete! Inbox Panel created.");
        Debug.Log("Next: Tools → Paper Trail → 3 - Generate Document Panel");

        EditorUtility.DisplayDialog(
            "Step 2 Complete",
            "Inbox Panel created!\n\nNext: Generate Document Panel (Step 3)",
            "OK"
        );
    }

    private static void CreateInboxPanel(GameObject parent)
    {
        GameObject inboxPanel = new GameObject("InboxPanel", typeof(RectTransform));
        inboxPanel.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = inboxPanel.AddComponent<Image>();
        img.color = Color.white;

        // Position: Left side, 400px wide, with margin
        RectTransform rt = inboxPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(220, 0); // 20px margin + 200px offset
        rt.sizeDelta = new Vector2(400, 0);

        // Create components
        CreateInboxHeader(inboxPanel);
        CreateInboxScrollView(inboxPanel);
        CreateRefreshButton(inboxPanel);

        // Add InboxUI script component
        inboxPanel.AddComponent<InboxUI>();

        Debug.Log("Inbox Panel created with all components!");
    }

    private static void CreateInboxHeader(GameObject parent)
    {
        GameObject header = new GameObject("Header", typeof(RectTransform));
        header.transform.SetParent(parent.transform, false);

        // Add Image component
        Image img = header.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Position at top
        RectTransform rt = header.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, -30);
        rt.sizeDelta = new Vector2(0, 60);

        // Create Title text
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

        // Create Count text
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

        Debug.Log("Inbox Header created!");
    }

    private static void CreateInboxScrollView(GameObject parent)
    {
        GameObject scrollView = new GameObject("ScrollView", typeof(RectTransform));
        scrollView.transform.SetParent(parent.transform, false);

        // Position between header and button
        RectTransform rt = scrollView.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 80); // Bottom space for refresh button
        rt.offsetMax = new Vector2(0, -60); // Top space for header

        // Add ScrollRect component
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<Image>().color = new Color(1, 1, 1, 0.1f);

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

        // Create Content (DocumentListParent)
        GameObject content = new GameObject("DocumentListParent", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);

        // Add Layout components
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

        // Configure ScrollRect
        scrollRect.content = contentRT;
        scrollRect.viewport = viewportRT;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20f;

        Debug.Log("Inbox ScrollView created!");
    }

    private static void CreateRefreshButton(GameObject parent)
    {
        GameObject refreshBtn = new GameObject("RefreshButton", typeof(RectTransform));
        refreshBtn.transform.SetParent(parent.transform, false);

        // Add Button and Image components
        Button btn = refreshBtn.AddComponent<Button>();
        Image img = refreshBtn.AddComponent<Image>();
        img.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // Set button colors
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = colors;

        // Position at bottom
        RectTransform rt = refreshBtn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(0, 40);
        rt.sizeDelta = new Vector2(-20, 60);

        // Create Button text
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

        Debug.Log("Refresh Button created!");
    }
#endif
}