using UnityEngine;

[System.Serializable]
public class FormField
{
    public string fieldName;
    public FormFieldType fieldType;
    public bool isRequired;
    public string placeholder;
    public string[] dropdownOptions;
    public string validationRule;
    public int maxLength;
    
    [Header("Bureaucratic Complications")]
    public bool triggersNewDocument;
    public string triggerDocumentType;
    public string rejectionReason;
}
