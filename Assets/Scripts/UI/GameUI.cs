using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI bureaucracyScoreText;
    public TextMeshProUGUI documentsProcessedText;
    public TextMeshProUGUI citizensServedText;

    [Header("Notifications")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public Button closeNotificationButton;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (closeNotificationButton != null)
        {
            closeNotificationButton.onClick.AddListener(CloseNotification);
        }

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        // Subscribe to document manager events for notifications
        DocumentManager docManager = FindObjectOfType<DocumentManager>();
        if (docManager != null)
        {
            docManager.OnDocumentAdded += OnDocumentAdded;
            docManager.OnDocumentProcessed += OnDocumentProcessed;
        }
    }

    private void Update()
    {
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (gameManager == null) return;

        if (bureaucracyScoreText != null)
        {
            bureaucracyScoreText.text = $"Bureaucracy Score: {gameManager.bureaucracyScore}";
        }

        if (documentsProcessedText != null)
        {
            documentsProcessedText.text = $"Documents Processed: {gameManager.documentsProcessed}";
        }

        if (citizensServedText != null)
        {
            citizensServedText.text = $"Citizens Served: {gameManager.citizensServed}";
        }
    }

    private void OnDocumentAdded(DocumentData document)
    {
        ShowNotification($"New document: {document.documentTitle}");
    }

    private void OnDocumentProcessed(DocumentData document)
    {
        ShowNotification($"Document processed: {document.documentTitle}");
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);

            // Auto-close after 3 seconds
            Invoke(nameof(CloseNotification), 3f);
        }
    }

    private void CloseNotification()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }
}
