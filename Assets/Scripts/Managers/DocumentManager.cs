using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DocumentManager : MonoBehaviour
{
    [Header("Document Storage")]
    public List<DocumentData> allDocuments = new List<DocumentData>();
    public List<DocumentData> inboxDocuments = new List<DocumentData>();
    public List<DocumentData> processedDocuments = new List<DocumentData>();

    [Header("Dynamic Configuration")]
    public DynamicConfigurationManager configurationManager;

    [Header("Events")]
    public System.Action<DocumentData> OnDocumentAdded;
    public System.Action<DocumentData> OnDocumentProcessed;
    public System.Action<DocumentData> OnDocumentRejected;

    private void Start()
    {
        // Find configuration manager if not assigned
        if (configurationManager == null)
        {
            configurationManager = FindObjectOfType<DynamicConfigurationManager>();
        }
    }

    public void AddDocument(DocumentData document)
    {
        allDocuments.Add(document);
        inboxDocuments.Add(document);

        Debug.Log($"New document added: {document.documentTitle}");
        OnDocumentAdded?.Invoke(document);
    }

    public void ProcessDocument(DocumentData document)
    {
        if (!inboxDocuments.Contains(document))
        {
            Debug.LogWarning("Trying to process document not in inbox!");
            return;
        }

        document.status = DocumentStatus.InProgress;
        document.lastModified = System.DateTime.Now;

        // Use dynamic configuration for bureaucracy cascades
        if (configurationManager != null)
        {
            var template = configurationManager.GetTemplate(document.documentType);
            if (template != null)
            {
                ProcessDynamicDocument(document, template);
            }
        }

        // Move to processed
        inboxDocuments.Remove(document);
        processedDocuments.Add(document);

        // Notify game manager
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.DocumentProcessed();
        }

        OnDocumentProcessed?.Invoke(document);
        Debug.Log($"Document processed: {document.documentTitle}");
    }

    private void ProcessDynamicDocument(DocumentData document, DocumentTemplate template)
    {
        Debug.Log($"Processing document with dynamic template: {template.documentTitle}");
        
        // Apply bureaucracy level
        document.bureaucracyLevel = template.baseBureaucracyLevel;
        
        // Check requirements
        if (template.requiresStamp)
        {
            Debug.Log("Document requires stamp!");
        }
        
        if (template.requiresSignature)
        {
            Debug.Log("Document requires signature!");
        }
        
        // The DynamicConfigurationManager handles trigger processing automatically
    }

    public void RejectDocument(DocumentData document, string reason)
    {
        document.status = DocumentStatus.Rejected;
        document.lastModified = System.DateTime.Now;

        inboxDocuments.Remove(document);
        processedDocuments.Add(document);

        OnDocumentRejected?.Invoke(document);
        Debug.Log($"Document rejected: {document.documentTitle} - Reason: {reason}");
    }

    // Dynamic document creation using templates
    public DocumentData CreateDocumentFromTemplate(DocumentTemplate template, FrenchCitizenData citizen)
    {
        if (configurationManager != null)
        {
            return configurationManager.GenerateDocumentFromTemplate(template, citizen);
        }
        
        Debug.LogWarning("No configuration manager available for dynamic document creation");
        return null;
    }

    public List<DocumentData> GetDocumentsByType(DocumentType type)
    {
        return allDocuments.Where(doc => doc.documentType == type).ToList();
    }

    public List<DocumentData> GetDocumentsByStatus(DocumentStatus status)
    {
        return allDocuments.Where(doc => doc.status == status).ToList();
    }

    public DocumentData GetDocumentById(string id)
    {
        return allDocuments.FirstOrDefault(doc => doc.documentId == id);
    }

    public int GetInboxCount()
    {
        return inboxDocuments.Count;
    }

    // Method to force generation of specific document type
    public void RequestDocument(DocumentType docType, string citizenName = null)
    {
        if (configurationManager != null)
        {
            configurationManager.ForceGenerateDocument(docType, citizenName);
        }
        else
        {
            Debug.LogWarning("Cannot request document - no configuration manager available");
        }
    }
}
