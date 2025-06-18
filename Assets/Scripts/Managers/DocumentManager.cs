using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DocumentManager : MonoBehaviour
{
    [Header("Document Storage")]
    public List<DocumentData> allDocuments = new List<DocumentData>();
    public List<DocumentData> inboxDocuments = new List<DocumentData>();
    public List<DocumentData> processedDocuments = new List<DocumentData>();

    [Header("Events")]
    public System.Action<DocumentData> OnDocumentAdded;
    public System.Action<DocumentData> OnDocumentProcessed;
    public System.Action<DocumentData> OnDocumentRejected;

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

        // Check if document triggers bureaucracy cascade
        BureaucracySystem bureaucracySystem = FindObjectOfType<BureaucracySystem>();
        if (bureaucracySystem != null)
        {
            bureaucracySystem.ProcessDocument(document);
        }

        // Move to processed
        inboxDocuments.Remove(document);
        processedDocuments.Add(document);

        GameManager.Instance.DocumentProcessed();
        OnDocumentProcessed?.Invoke(document);

        Debug.Log($"Document processed: {document.documentTitle}");
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
}