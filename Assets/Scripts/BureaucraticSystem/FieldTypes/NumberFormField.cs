[System.Serializable]
public class NumberFormField : FormField
{
    public float numberValue = 0f;
    public float minValue = float.MinValue;
    public float maxValue = float.MaxValue;
    
    public override object GetValue() => numberValue;
    public override void SetValue(object value) 
    {
        if (value is float f) numberValue = f;
        else if (float.TryParse(value?.ToString(), out float result)) numberValue = result;
    }
    public override bool IsValid() => !isRequired || (numberValue >= minValue && numberValue <= maxValue);
}
