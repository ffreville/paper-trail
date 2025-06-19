using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class NaturalLightUIExtension
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Paper Trail/Add Natural Light Need System")]
    public static void AddNaturalLightSystem()
    {
        bool confirm = UnityEditor.EditorUtility.DisplayDialog(
            "Add Natural Light Need",
            "This will add the 'Natural Light Deprivation' system to your Paper Trail project.\n\n" +
            "Features:\n" +
            "• 6th physiological need: Natural Light\n" +
            "• Seasonal cycles affecting depression\n" +
            "• Window opening permits and bureaucracy\n" +
            "• Seasonal Affective Disorder documents\n" +
            "• Light therapy bureaucracy cascade\n" +
            "• Visual depression effects\n" +
            "• Sunlight particle effects\n\n" +
            "Your employees will now need PERMISSION to see the sun!",
            "Add Natural Light Hell",
            "Cancel"
        );

        if (!confirm) return;

        Debug.Log("=== ADDING NATURAL LIGHT DEPRIVATION SYSTEM ===");

        CreateNaturalLightExtension();
        AddNaturalLightToUI();
        CreateNaturalLightDocumentTemplates();
        
        UnityEditor.AssetDatabase.Refresh();

        UnityEditor.EditorUtility.DisplayDialog(
            "Natural Light System Added!",
            "The Natural Light Deprivation system has been successfully added!\n\n" +
            "New features:\n" +
            "✅ Natural Light bar (6th need)\n" +
            "✅ Window opening button with quota (2/day max)\n" +
            "✅ Seasonal depression cycles\n" +
            "✅ Window permits (FORM-WIN-001)\n" +
            "✅ Light therapy documents (LUM-505)\n" +
            "✅ Seasonal Affective Disorder forms (SAD-404)\n" +
            "✅ Visual depression effects\n" +
            "✅ Sunlight particles when window opens\n\n" +
            "Now even SUNLIGHT requires paperwork! 🌞📋",
            "Diabolically Perfect!"
        );

        Debug.Log("=== NATURAL LIGHT SYSTEM COMPLETE ===");
    }

    private static void CreateNaturalLightExtension()
    {
        // Find PhysiologicalNeedsManager
        PhysiologicalNeedsManager needsManager = Object.FindObjectOfType<PhysiologicalNeedsManager>();
        if (needsManager == null)
        {
            Debug.LogError("PhysiologicalNeedsManager not found! Please generate the basic needs system first.");
            return;
        }

        // Add NaturalLightNeedExtension to the same GameObject
        if (needsManager.GetComponent<NaturalLightNeedExtension>() == null)
        {
            NaturalLightNeedExtension extension = needsManager.gameObject.AddComponent<NaturalLightNeedExtension>();
            extension.enableNaturalLightNeed = true;
            extension.windowsAreBlocked = true;
            extension.fluorescentLightsOnly = true;
            extension.windowPermitsRequired = true;
            
            Debug.Log("✅ NaturalLightNeedExtension added");
        }
    }

    private static void AddNaturalLightToUI()
    {
        // Find PhysiologicalNeedsUI
        PhysiologicalNeedsUI needsUI = Object.FindObjectOfType<PhysiologicalNeedsUI>();
        if (needsUI == null)
        {
            Debug.LogError("PhysiologicalNeedsUI not found!");
            return;
        }

        GameObject needsPanel = needsUI.gameObject;
        
        // Add Natural Light bar to the existing needs panel
        AddNaturalLightBar(needsPanel);
        
        // Add Window controls
        AddWindowControls(needsPanel);
        
        // Add seasonal display
        AddSeasonalDisplay(needsPanel);
        
        // Add NaturalLightUI component
        if (needsPanel.GetComponent<NaturalLightUI>() == null)
        {
            needsPanel.AddComponent<NaturalLightUI>();
        }
        
        Debug.Log("✅ Natural Light UI added to existing needs panel");
    }

    private static void AddNaturalLightBar(GameObject parent)
    {
        // Create Natural Light bar (similar to existing need bars)
        GameObject lightBarContainer = new GameObject("NaturalLightBarContainer", typeof(RectTransform));
        lightBarContainer.transform.SetParent(parent.transform, false);

        RectTransform containerRT = lightBarContainer.GetComponent<RectTransform>();
        containerRT.sizeDelta = new Vector2(0, 40);

        HorizontalLayoutGroup hlg = lightBarContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandHeight = true;

        // Icon
        GameObject iconObj = new GameObject("NaturalLightIcon", typeof(RectTransform));
        iconObj.transform.SetParent(lightBarContainer.transform, false);
        
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.text = "☀️";
        iconText.fontSize = 20;
        iconText.alignment = TextAlignmentOptions.Center;
        
        RectTransform iconRT = iconObj.GetComponent<RectTransform>();
        iconRT.sizeDelta = new Vector2(30, 30);
        
        LayoutElement iconLE = iconObj.AddComponent<LayoutElement>();
        iconLE.minWidth = 30;
        iconLE.preferredWidth = 30;

        // Label
        GameObject labelObj = new GameObject("Label", typeof(RectTransform));
        labelObj.transform.SetParent(lightBarContainer.transform, false);
        
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Soleil";
        label.fontSize = 12;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelLE = labelObj.AddComponent<LayoutElement>();
        labelLE.minWidth = 50;
        labelLE.preferredWidth = 50;

        // Slider
        GameObject sliderObj = new GameObject("NaturalLightBar", typeof(RectTransform));
        sliderObj.transform.SetParent(lightBarContainer.transform, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.interactable = false;

        Image sliderBG = sliderObj.AddComponent<Image>();
        sliderBG.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Fill area and fill
        CreateSliderFill(sliderObj, slider, Color.yellow);

        LayoutElement sliderLE = sliderObj.AddComponent<LayoutElement>();
        sliderLE.flexibleWidth = 1;
    }

    private static void AddWindowControls(GameObject parent)
    {
        // Add spacer
        CreateSpacer(parent, 15);
        
        // Title
        CreateLabel(parent, "CONTRÔLE LUMINEUX", 12, FontStyles.Bold, Color.cyan);
        
        // Window button
        GameObject windowBtn = new GameObject("WindowButton", typeof(RectTransform));
        windowBtn.transform.SetParent(parent.transform, false);

        Button button = windowBtn.AddComponent<Button>();
        Image buttonImage = windowBtn.AddComponent<Image>();
        buttonImage.color = new Color(1f, 0.8f, 0.2f); // Orange/yellow

        RectTransform buttonRT = windowBtn.GetComponent<RectTransform>();
        buttonRT.sizeDelta = new Vector2(0, 60);

        // Button text
        GameObject textObj = new GameObject("WindowButtonText", typeof(RectTransform));
        textObj.transform.SetParent(windowBtn.transform, false);

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "OUVRIR\nFENÊTRE";
        buttonText.fontSize = 11;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.black;
        buttonText.fontStyle = FontStyles.Bold;

        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Quota text
        CreateLabel(parent, "Ouvertures: 2/2", 10, FontStyles.Normal, Color.white, "WindowQuotaText");
    }

    private static void AddSeasonalDisplay(GameObject parent)
    {
        CreateSpacer(parent, 15);
        CreateLabel(parent, "SAISON ADMINISTRATIVE", 12, FontStyles.Bold, Color.green);
        CreateLabel(parent, "Printemps", 14, FontStyles.Normal, Color.white, "SeasonText");
        CreateLabel(parent, "Réglementation active", 9, FontStyles.Italic, Color.gray, "SeasonEffectText");
    }

    private static void CreateSliderFill(GameObject sliderObj, Slider slider, Color fillColor)
    {
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillAreaRT = fillArea.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = Vector2.zero;
        fillAreaRT.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        slider.fillRect = fillRT;
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

    private static void CreateNaturalLightDocumentTemplates()
    {
        // Use the existing generator from NaturalLightNeedExtension
        Debug.Log("Natural Light document templates will be created by the extension system");
    }
#endif
}
