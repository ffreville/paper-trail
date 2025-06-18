using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InboxUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform documentListParent;
    public GameObject documentItemPrefab;
    public TextMeshProUGUI inboxCountText;
    public Button refreshButton;

    [Header("Document Selection")]
    public DocumentUI documentUI;

    private DocumentManager documentManager;
    private List<GameObject> documentUIItems = new List<GameObject>();

    private void Start()
    {
        documentManager = FindObjectOfType<DocumentManager>();

        if (documentManager != null)
        {
            // Subscribe to document events
            documentManager.OnDocumentAdded += OnDocumentAdded;
            documentManager.OnDocumentProcessed += OnDocumentProcessed;
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshInbox);
        }

        RefreshInbox();
    }

    private void OnDestroy()
    {
        if (documentManager != null)
        {
            documentManager.OnDocumentAdded -= OnDocumentAdded;
            documentManager.OnDocumentProcessed -= OnDocumentProcessed;
        }
    }

    private void OnDocumentAdded(DocumentData document)
    {
        Debug.Log($"UI: New document added to inbox: {document.documentTitle}");
        RefreshInbox();
    }

    private void OnDocumentProcessed(DocumentData document)
    {
        Debug.Log($"UI: Document processed: {document.documentTitle}");
        RefreshInbox();
    }

    public void RefreshInbox()
    {
        if (documentManager == null) return;

        // Clear existing UI items
        ClearDocumentItems();

        // Create UI item for each document in inbox
        foreach (DocumentData doc in documentManager.inboxDocuments)
        {
            CreateDocumentUIItem(doc);
        }

        // Update inbox count
        UpdateInboxCount();
    }

    private void ClearDocumentItems()
    {
        foreach (GameObject item in documentUIItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        documentUIItems.Clear();
    }

    private void CreateDocumentUIItem(DocumentData document)
    {
        if (documentItemPrefab == null || documentListParent == null) return;

        GameObject item = Instantiate(documentItemPrefab, documentListParent);
        documentUIItems.Add(item);

        // Configure the document item
        DocumentItemUI itemUI = item.GetComponent<DocumentItemUI>();
        if (itemUI != null)
        {
            itemUI.Setup(document, this);
        }
    }

    private void UpdateInboxCount()
    {
        if (inboxCountText != null && documentManager != null)
        {
            inboxCountText.text = $"Inbox ({documentManager.GetInboxCount()})";
        }
    }

    public void SelectDocument(DocumentData document)
    {
        if (documentUI != null)
        {
            documentUI.DisplayDocument(document);
        }
    }
}
