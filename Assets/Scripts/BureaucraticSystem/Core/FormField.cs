using UnityEngine;

[System.Serializable]
public abstract class FormField
{
    public string fieldName;
    public string label;
    public bool isRequired;
    public bool isReadOnly;
    
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract bool IsValid();
}
