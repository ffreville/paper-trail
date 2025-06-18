using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class DocumentData
{
    [Header("Document Information")]
    public string documentId;
    public string documentTitle;
    public DocumentType documentType;
    public DocumentStatus status;
    public DepartmentType currentDepartment;

    [Header("Content")]
    public string citizenName;
    public string requestDetails;
    public Dictionary<string, string> formFields;
    public List<string> requiredDocuments;

    [Header("Bureaucracy")]
    public bool requiresStamp;
    public bool requiresSignature;
    public int bureaucracyLevel; // How many steps this document will generate

    [Header("Metadata")]
    public DateTime creationDate;
    public DateTime lastModified;
    public string assignedEmployee;

    public DocumentData()
    {
        documentId = System.Guid.NewGuid().ToString();
        formFields = new Dictionary<string, string>();
        requiredDocuments = new List<string>();
        creationDate = DateTime.Now;
        lastModified = DateTime.Now;
        status = DocumentStatus.Pending;
    }
}
