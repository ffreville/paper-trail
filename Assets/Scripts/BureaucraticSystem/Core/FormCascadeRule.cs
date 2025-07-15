using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FormCascadeRule
{
    public string ruleName;
    public CascadeCondition condition;
    public List<BureaucraticForm> formsToTrigger;
    public int delayMinutes = 0;
    public string triggerMessage;
    
    public bool ShouldTrigger(BureaucraticForm form)
    {
        return condition.IsMet(form);
    }
}

[System.Serializable]
public class CascadeCondition
{
    public enum ConditionType
    {
        Always,
        FieldEquals,
        FieldGreaterThan,
        FieldLessThan,
        FormDepartment,
        Custom
    }
    
    public ConditionType type;
    public string fieldName;
    public string expectedValue;
    public float numericValue;
    
    public bool IsMet(BureaucraticForm form)
    {
        switch (type)
        {
            case ConditionType.Always:
                return true;
                
            case ConditionType.FieldEquals:
                var field = form.fields.Find(f => f.fieldName == fieldName);
                return field != null && field.GetValue().ToString() == expectedValue;
                
            case ConditionType.FieldGreaterThan:
                var numField = form.fields.Find(f => f.fieldName == fieldName);
                if (numField != null && float.TryParse(numField.GetValue().ToString(), out float val))
                    return val > numericValue;
                return false;
                
            case ConditionType.FormDepartment:
                return form.department == expectedValue;
                
            default:
                return false;
        }
    }
}
