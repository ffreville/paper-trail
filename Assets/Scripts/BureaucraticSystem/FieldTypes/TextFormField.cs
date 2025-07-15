
[System.Serializable]
public class TextFormField : FormField
{
    public string textValue = "";
    public int maxLength = 100;
    public string placeholder = "";
    
    public override object GetValue() => textValue;
    public override void SetValue(object value) => textValue = value?.ToString() ?? "";
    public override bool IsValid() => !isRequired || !string.IsNullOrEmpty(textValue);
}
