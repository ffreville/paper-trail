
[System.Serializable]
public class DateFormField : FormField
{
    public string dateValue = "";
    public bool mustBeFuture = false;
    public bool mustBePast = false;
    
    public override object GetValue() => dateValue;
    public override void SetValue(object value) => dateValue = value?.ToString() ?? "";
    public override bool IsValid()
    {
        if (!isRequired && string.IsNullOrEmpty(dateValue)) return true;
        if (isRequired && string.IsNullOrEmpty(dateValue)) return false;
        
        if (System.DateTime.TryParse(dateValue, out System.DateTime date))
        {
            if (mustBeFuture && date <= System.DateTime.Now) return false;
            if (mustBePast && date >= System.DateTime.Now) return false;
            return true;
        }
        return false;
    }
}
