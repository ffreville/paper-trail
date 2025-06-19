using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Document Template", menuName = "Paper Trail/Document Template")]
public class DocumentTemplate : ScriptableObject
{
    [Header("Document Info")]
    public string documentTitle;
    public DocumentType documentType;
    public string description;
    public Sprite documentIcon;
    
    [Header("Form Configuration")]
    public List<FormField> formFields = new List<FormField>();
    
    [Header("Bureaucracy Settings")]
    public bool requiresStamp;
    public bool requiresSignature;
    public int baseBureaucracyLevel = 1;
    public int processingTimeMinutes = 5;
    
    [Header("Triggers and Cascades")]
    public List<BureaucracyTrigger> triggers = new List<BureaucracyTrigger>();
    
    [Header("French Localization")]
    public string frenchTitle;
    public string frenchDescription;
    public List<string> frenchFieldNames = new List<string>();
    
    [Header("Rejection Scenarios")]
    public List<string> possibleRejections = new List<string>();
    public float rejectionProbability = 0.1f;
}
