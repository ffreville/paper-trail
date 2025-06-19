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
    private InboxUI inboxUI;

    public void Setup(DocumentData document, InboxUI inbox)
    {
        documentData = document;
        inboxUI = inbox;

        Debug.Log($"Setting up DocumentItem: {document.documentTitle}");
        UpdateDisplay();

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
        Debug.Log("Document selected!");
        
        if (inboxUI != null && documentData != null)
        {
            inboxUI.SelectDocument(documentData);
        }
        else
        {
            Debug.LogError("Missing InboxUI or DocumentData reference!");
        }
    }
}
