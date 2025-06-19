using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Générateur pour GameUI et NotificationPanel
/// </summary>
public static class UIFinalGenerator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Paper Trail/4 - Generate Game UI and Notifications")]
    public static void GenerateGameUIAndNotifications()
    {
        Debug.Log("Step 4: Creating Game UI and Notification Panel...");

        // Find Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Canvas Not Found",
                "Please run Step 1 first (Generate Canvas and Main Panel)",
                "OK"
            );
            return;
        }

        // Create Game UI and Notification Panel
        CreateGameUI(canvas);
        CreateNotificationPanel(canvas);

        Debug.Log("Step 4 Complete! All UI generated.");
        Debug.Log("Next: Generate System GameObjects or create prefabs");

        EditorUtility.DisplayDialog(
            "All Steps Complete!",
            "Game UI and Notifications created!\n\nUI Generation complete!\n\nNext: Add scripts and connect references.",
            "OK"
        );
    }

    private static void CreateGameUI(Canvas canvas)
    {
        GameObject gameUI = new GameObject("GameUI", typeof(RectTransform));
        gameUI.transform.SetParent(canvas.transform, false);

        // Add Image component
        Image img = gameUI.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.5f);

        // Position: Top right corner
        RectTransform rt = gameUI.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-170, -70);
        rt.sizeDelta = new Vector2(300, 120);

        // Add Layout Group
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

        // Add GameUI script component
        gameUI.AddComponent<GameUI>();

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
    }

    private static void CreateNotificationPanel(Canvas canvas)
    {
        GameObject notificationPanel = new GameObject("NotificationPanel", typeof(RectTransform));
        notificationPanel.transform.SetParent(canvas.transform, false);

        // Add Image component
        Image img = notificationPanel.AddComponent<Image>();
        img.color = new Color(1f, 0.6f, 0f, 0.9f); // Orange

        // Position: Top center
        RectTransform rt = notificationPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -50);
        rt.sizeDelta = new Vector2(400, 80);

        // Add Layout Group
        HorizontalLayoutGroup hlg = notificationPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(15, 15, 15, 15);
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlHeight = true;
        hlg.childControlWidth = false;

        // Create Notification Text
        GameObject textObj = new GameObject("NotificationText", typeof(RectTransform));
        textObj.transform.SetParent(notificationPanel.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Notification message here";
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;

        LayoutElement le = textObj.AddComponent<LayoutElement>();
        le.flexibleWidth = 1;

        // Create Close Button
        GameObject closeBtn = new GameObject("CloseNotificationButton", typeof(RectTransform));
        closeBtn.transform.SetParent(notificationPanel.transform, false);

        Button btn = closeBtn.AddComponent<Button>();
        Image btnImg = closeBtn.AddComponent<Image>();
        btnImg.color = new Color(0.96f, 0.26f, 0.21f, 1f);

        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.96f, 0.26f, 0.21f, 1f);
        cb.highlightedColor = new Color(1f, 0.4f, 0.3f, 1f);
        cb.pressedColor = new Color(0.8f, 0.2f, 0.1f, 1f);
        btn.colors = cb;

        LayoutElement btnLE = closeBtn.AddComponent<LayoutElement>();
        btnLE.minWidth = 60;
        btnLE.preferredWidth = 60;

        // Create Close Button Text
        GameObject closeTxtObj = new GameObject("Text", typeof(RectTransform));
        closeTxtObj.transform.SetParent(closeBtn.transform, false);

        TextMeshProUGUI closeTxt = closeTxtObj.AddComponent<TextMeshProUGUI>();
        closeTxt.text = "✕";
        closeTxt.fontSize = 18;
        closeTxt.alignment = TextAlignmentOptions.Center;
        closeTxt.color = Color.white;

        RectTransform closeTextRT = closeTxtObj.GetComponent<RectTransform>();
        closeTextRT.anchorMin = Vector2.zero;
        closeTextRT.anchorMax = Vector2.one;
        closeTextRT.offsetMin = Vector2.zero;
        closeTextRT.offsetMax = Vector2.zero;

        // Hide panel by default
        notificationPanel.SetActive(false);

        Debug.Log("Notification Panel created!");
    }
#endif
}