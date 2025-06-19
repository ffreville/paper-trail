using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI citizenText;
    public TextMeshProUGUI statusText;
    public Image statusIcon;
    public Button selectButton;

    [Header("Status Colors")]
    public Color pendingColor = Color.yellow;
    public Color inProgressColor = Color.blue;
    public Color waitingColor = Color.magenta;
    public Color completedColor = Color.green;

    private DocumentData documentData;
    
    // Support pour les deux types d'inbox (legacy et enhanced)
    private InboxUI legacyInboxUI;
    private EnhancedInboxUI enhancedInboxUI;

    // Setup method pour legacy InboxUI
    public void Setup(DocumentData document, InboxUI inbox)
    {
        documentData = document;
        legacyInboxUI = inbox;
        enhancedInboxUI = null;

        Debug.Log($"Setting up DocumentItem (Legacy): {document.documentTitle}");
        UpdateDisplay();
        SetupButton();
    }

    // Setup method pour Enhanced InboxUI
    public void Setup(DocumentData document, EnhancedInboxUI inbox)
    {
        documentData = document;
        enhancedInboxUI = inbox;
        legacyInboxUI = null;

        Debug.Log($"Setting up DocumentItem (Enhanced): {document.documentTitle}");
        UpdateDisplay();
        SetupButton();
    }

    private void SetupButton()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectDocument);
        }
    }

    private void UpdateDisplay()
    {
        if (documentData == null) return;

        if (titleText != null)
        {
            titleText.text = documentData.documentTitle;
        }

        if (citizenText != null)
        {
            citizenText.text = $"From: {documentData.citizenName}";
        }

        if (statusText != null)
        {
            statusText.text = documentData.status.ToString();
        }

        if (statusIcon != null)
        {
            statusIcon.color = GetStatusColor(documentData.status);
        }
    }

    private Color GetStatusColor(DocumentStatus status)
    {
        switch (status)
        {
            case DocumentStatus.Pending:
                return pendingColor;
            case DocumentStatus.InProgress:
                return inProgressColor;
            case DocumentStatus.WaitingForAdditionalInfo:
                return waitingColor;
            case DocumentStatus.Completed:
                return completedColor;
            default:
                return Color.white;
        }
    }

    public void OnSelectDocument()
    {
        Debug.Log("Button clicked!");
        
        if (documentData == null)
        {
            Debug.LogWarning("No document data available!");
            return;
        }

        // Essaie d'abord avec Enhanced, puis avec Legacy
        if (enhancedInboxUI != null)
        {
            Debug.Log($"Using Enhanced InboxUI for document: {documentData.documentTitle}");
            enhancedInboxUI.SelectDocument(documentData);
        }
        else if (legacyInboxUI != null)
        {
            Debug.Log($"Using Legacy InboxUI for document: {documentData.documentTitle}");
            legacyInboxUI.SelectDocument(documentData);
        }
        else
        {
            Debug.LogError("No InboxUI reference available (neither Legacy nor Enhanced)!");
        }
    }

    // Méthode pour vérifier quel type d'inbox est utilisé
    public bool IsUsingEnhancedUI()
    {
        return enhancedInboxUI != null;
    }

    public bool IsUsingLegacyUI()
    {
        return legacyInboxUI != null;
    }
}
