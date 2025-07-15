[System.Serializable]
public class DropdownFormField : FormField
{
    public string[] options;
    public int selectedIndex = 0;
    
    public override object GetValue() => selectedIndex >= 0 && selectedIndex < options.Length ? options[selectedIndex] : "";
    public override void SetValue(object value)
    {
        if (value is int i) selectedIndex = i;
        else if (value is string s) selectedIndex = System.Array.IndexOf(options, s);
    }
    public override bool IsValid() => !isRequired || selectedIndex >= 0;
}
