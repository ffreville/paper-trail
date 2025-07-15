[System.Serializable]
public class CheckboxFormField : FormField
{
    public bool isChecked = false;
    
    public override object GetValue() => isChecked;
    public override void SetValue(object value) => isChecked = value is bool b && b;
    public override bool IsValid() => !isRequired || isChecked;
}
