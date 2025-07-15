using UnityEngine;

[System.Serializable]
public class FormValidationRule
{
    public string ruleName;
    public string errorMessage;
    public ValidationCondition condition;
    
    public bool Validate(BureaucraticForm form)
    {
        return condition.IsValid(form);
    }
}

[System.Serializable]
public class ValidationCondition
{
    public enum ValidationType
    {
        FieldsMatch,
        FieldsConflict,
        DateRange,
        CustomLogic
    }
    
    public ValidationType type;
    public string field1Name;
    public string field2Name;
    public string customValidationCode;
    
    public bool IsValid(BureaucraticForm form)
    {
        switch (type)
        {
            case ValidationType.FieldsMatch:
                var field1 = form.fields.Find(f => f.fieldName == field1Name);
                var field2 = form.fields.Find(f => f.fieldName == field2Name);
                return field1 != null && field2 != null && 
                       field1.GetValue().ToString() == field2.GetValue().ToString();
                
            case ValidationType.FieldsConflict:
                var conflictField1 = form.fields.Find(f => f.fieldName == field1Name);
                var conflictField2 = form.fields.Find(f => f.fieldName == field2Name);
                return conflictField1 != null && conflictField2 != null && 
                       conflictField1.GetValue().ToString() != conflictField2.GetValue().ToString();
                
            default:
                return true;
        }
    }
}
