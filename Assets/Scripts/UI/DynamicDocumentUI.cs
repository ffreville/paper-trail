using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Enhanced Document UI to work with dynamic templates
public class DynamicDocumentUI : MonoBehaviour
{
    [Header("Dynamic Form Generation")]
    public Transform dynamicFormParent;
    public GameObject textFieldPrefab;
    public GameObject numberFieldPrefab;
    public GameObject dateFieldPrefab;
    public GameObject dropdownFieldPrefab;
    public GameObject checkboxFieldPrefab;
    public GameObject textAreaFieldPrefab;
    public GameObject signatureAreaPrefab;
    public GameObject stampAreaPrefab;
    
    [Header("French Localization")]
    public bool useFrenchLabels = true;
    
    private DocumentData currentDocument;
    private DocumentTemplate currentTemplate;
    private DynamicConfigurationManager configManager;
    private List<GameObject> dynamicFields = new List<GameObject>();
    
    private void Start()
    {
        configManager = FindObjectOfType<DynamicConfigurationManager>();
    }
    
    public void DisplayDocument(DocumentData document)
    {
        currentDocument = document;
        currentTemplate = configManager?.GetTemplate(document.documentType);
        
        ClearDynamicFields();
        GenerateDynamicForm();
    }
    
    private void ClearDynamicFields()
    {
        foreach (var field in dynamicFields)
        {
            if (field != null) Destroy(field);
        }
        dynamicFields.Clear();
    }
    
    private void GenerateDynamicForm()
    {
        if (currentTemplate == null || currentDocument == null) return;
        
        for (int i = 0; i < currentTemplate.formFields.Count; i++)
        {
            var fieldTemplate = currentTemplate.formFields[i];
            CreateDynamicField(fieldTemplate, i);
        }
    }
    
    private void CreateDynamicField(FormField fieldTemplate, int index)
    {
        GameObject fieldPrefab = GetFieldPrefab(fieldTemplate.fieldType);
        if (fieldPrefab == null) return;
        
        GameObject fieldInstance = Instantiate(fieldPrefab, dynamicFormParent);
        dynamicFields.Add(fieldInstance);
        
        // Configure field based on template
        ConfigureField(fieldInstance, fieldTemplate, index);
    }
    
    private GameObject GetFieldPrefab(FormFieldType fieldType)
    {
        switch (fieldType)
        {
            case FormFieldType.Text: return textFieldPrefab;
            case FormFieldType.Number: return numberFieldPrefab;
            case FormFieldType.Date: return dateFieldPrefab;
            case FormFieldType.Dropdown: return dropdownFieldPrefab;
            case FormFieldType.Checkbox: return checkboxFieldPrefab;
            case FormFieldType.TextArea: return textAreaFieldPrefab;
            case FormFieldType.Signature: return signatureAreaPrefab;
            case FormFieldType.Stamp: return stampAreaPrefab;
            default: return textFieldPrefab;
        }
    }
    
    private void ConfigureField(GameObject fieldInstance, FormField fieldTemplate, int index)
    {
        // Set label
        var label = fieldInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label != null)
        {
            string labelText = fieldTemplate.fieldName;
            
            // Use French localization if available
            if (useFrenchLabels && currentTemplate != null && 
                index < currentTemplate.frenchFieldNames.Count &&
                !string.IsNullOrEmpty(currentTemplate.frenchFieldNames[index]))
            {
                labelText = currentTemplate.frenchFieldNames[index];
            }
            
            // Add required indicator
            if (fieldTemplate.isRequired)
            {
                labelText += " *";
            }
            
            label.text = labelText;
        }
        
        // Configure input component
        var inputField = fieldInstance.GetComponentInChildren<TMPro.TMP_InputField>();
        if (inputField != null)
        {
            inputField.placeholder.GetComponent<TMPro.TextMeshProUGUI>().text = fieldTemplate.placeholder;
            
            if (fieldTemplate.maxLength > 0)
            {
                inputField.characterLimit = fieldTemplate.maxLength;
            }
            
            // Set initial value from document
            if (currentDocument.formFields.ContainsKey(fieldTemplate.fieldName))
            {
                inputField.text = currentDocument.formFields[fieldTemplate.fieldName];
            }
            
            // Add validation
            if (!string.IsNullOrEmpty(fieldTemplate.validationRule))
            {
                AddValidation(inputField, fieldTemplate.validationRule);
            }
        }
        
        // Configure dropdown
        var dropdown = fieldInstance.GetComponentInChildren<TMPro.TMP_Dropdown>();
        if (dropdown != null && fieldTemplate.dropdownOptions != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(fieldTemplate.dropdownOptions));
            
            // Set selected value
            if (currentDocument.formFields.ContainsKey(fieldTemplate.fieldName))
            {
                string currentValue = currentDocument.formFields[fieldTemplate.fieldName];
                int index2 = System.Array.IndexOf(fieldTemplate.dropdownOptions, currentValue);
                if (index2 >= 0)
                {
                    dropdown.value = index2;
                }
            }
        }
        
        // Configure checkbox
        var toggle = fieldInstance.GetComponentInChildren<UnityEngine.UI.Toggle>();
        if (toggle != null)
        {
            if (currentDocument.formFields.ContainsKey(fieldTemplate.fieldName))
            {
                bool.TryParse(currentDocument.formFields[fieldTemplate.fieldName], out bool value);
                toggle.isOn = value;
            }
        }
    }
    
    private void AddValidation(TMPro.TMP_InputField inputField, string validationRule)
    {
        inputField.onValueChanged.AddListener((value) =>
        {
            ValidateField(inputField, validationRule, value);
        });
    }
    
    private void ValidateField(TMPro.TMP_InputField inputField, string validationRule, string value)
    {
        bool isValid = true;
        
        // Simple validation rules
        switch (validationRule.ToLower())
        {
            case "email":
                isValid = value.Contains("@") && value.Contains(".");
                break;
            case "phone":
                isValid = value.Length >= 10 && value.All(c => char.IsDigit(c) || c == '.' || c == '-' || c == ' ');
                break;
            case "numeric":
                isValid = value.All(char.IsDigit);
                break;
            case "date":
                isValid = System.DateTime.TryParse(value, out _);
                break;
        }
        
        // Visual feedback
        var image = inputField.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = isValid ? Color.white : new Color(1f, 0.8f, 0.8f); // Light red for invalid
        }
    }
    
    public void SaveFormData()
    {
        if (currentDocument == null || currentTemplate == null) return;
        
        for (int i = 0; i < dynamicFields.Count && i < currentTemplate.formFields.Count; i++)
        {
            var fieldTemplate = currentTemplate.formFields[i];
            var fieldInstance = dynamicFields[i];
            
            string value = GetFieldValue(fieldInstance, fieldTemplate.fieldType);
            currentDocument.formFields[fieldTemplate.fieldName] = value;
        }
        
        currentDocument.lastModified = System.DateTime.Now;
    }
    
    private string GetFieldValue(GameObject fieldInstance, FormFieldType fieldType)
    {
        switch (fieldType)
        {
            case FormFieldType.Text:
            case FormFieldType.Number:
            case FormFieldType.Date:
            case FormFieldType.TextArea:
                var inputField = fieldInstance.GetComponentInChildren<TMPro.TMP_InputField>();
                return inputField != null ? inputField.text : "";
                
            case FormFieldType.Dropdown:
                var dropdown = fieldInstance.GetComponentInChildren<TMPro.TMP_Dropdown>();
                return dropdown != null ? dropdown.options[dropdown.value].text : "";
                
            case FormFieldType.Checkbox:
                var toggle = fieldInstance.GetComponentInChildren<UnityEngine.UI.Toggle>();
                return toggle != null ? toggle.isOn.ToString() : "false";
                
            default:
                return "";
        }
    }
}
