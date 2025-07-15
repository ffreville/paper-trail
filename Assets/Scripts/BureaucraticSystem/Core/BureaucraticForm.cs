using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Bureaucratic Form", menuName = "Bureaucracy/Form")]
public class BureaucraticForm : ScriptableObject
{
    [Header("Form Information")]
    public string formTitle;
    public string formCode; // Ex: "FORM-001-A"
    public string department;
    public string description;
    
    [Header("Visual Properties")]
    public Texture2D formBackground;
    public Color inkColor = Color.black;
    public bool hasWatermark = true;
    
    [Header("Bureaucratic Properties")]
    public int processingTimeMinutes = 5;
    public int priorityLevel = 1; // 1 = low, 5 = urgent
    public bool requiresStamp = true;
    public bool requiresSignature = true;
    
    [Header("Form Fields")]
    [SerializeReference] public List<FormField> fields = new List<FormField>();
    
    [Header("Cascade Rules")]
    public List<FormCascadeRule> cascadeRules = new List<FormCascadeRule>();
    
    [Header("Validation Rules")]
    public List<FormValidationRule> validationRules = new List<FormValidationRule>();
    
    public bool IsFormValid()
    {
        // Vérifier tous les champs
        foreach (var field in fields)
        {
            if (!field.IsValid()) return false;
        }
        
        // Vérifier les règles de validation personnalisées
        foreach (var rule in validationRules)
        {
            if (!rule.Validate(this)) return false;
        }
        
        return true;
    }
    
    public List<BureaucraticForm> GetTriggeredForms()
    {
        List<BureaucraticForm> triggeredForms = new List<BureaucraticForm>();
        
        foreach (var rule in cascadeRules)
        {
            if (rule.ShouldTrigger(this))
            {
                triggeredForms.AddRange(rule.formsToTrigger);
            }
        }
        
        return triggeredForms;
    }
}
