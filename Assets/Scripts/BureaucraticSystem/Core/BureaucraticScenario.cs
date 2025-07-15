using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Bureaucratic Scenario", menuName = "Bureaucracy/Scenario")]
public class BureaucraticScenario : ScriptableObject
{
    [Header("Scenario Information")]
    public string scenarioTitle;
    public string scenarioDescription;
    public string citizenRequest; // La vraie demande du citoyen
    
    [Header("Narrative Elements")]
    public string initialBriefing;
    public string completionMessage;
    public string failureMessage;
    
    [Header("Emotional Impact")]
    [Range(1, 10)]
    public int emotionalWeight = 5;
    public string realWorldConsequence;
    
    [Header("Forms Chain")]
    public List<ScenarioStep> steps = new List<ScenarioStep>();
    
    [Header("Success Criteria")]
    public List<SuccessCriteria> successConditions = new List<SuccessCriteria>();
    
    public bool IsScenarioComplete()
    {
        return successConditions.TrueForAll(criteria => criteria.IsMet());
    }
    
    public float GetCompletionPercentage()
    {
        if (successConditions.Count == 0) return 0f;
        
        int metConditions = 0;
        foreach (var criteria in successConditions)
        {
            if (criteria.IsMet()) metConditions++;
        }
        return (float)metConditions / successConditions.Count;
    }
}

[System.Serializable]
public class ScenarioStep
{
    public string stepName;
    public BureaucraticForm requiredForm;
    public string instructions;
    public bool isOptional = false;
    public bool isCompleted = false;
    
    public List<ScenarioStep> nextSteps = new List<ScenarioStep>();
}

[System.Serializable]
public class SuccessCriteria
{
    public enum CriteriaType
    {
        FormCompleted,
        TimeLimit,
        NoErrors,
        SpecificFieldValue
    }
    
    public CriteriaType type;
    public BureaucraticForm targetForm;
    public string fieldName;
    public string requiredValue;
    public float timeLimit;
    
    public bool IsMet()
    {
        switch (type)
        {
            case CriteriaType.FormCompleted:
                return targetForm != null && targetForm.IsFormValid();
                
            case CriteriaType.SpecificFieldValue:
                if (targetForm == null) return false;
                var field = targetForm.fields.Find(f => f.fieldName == fieldName);
                return field != null && field.GetValue().ToString() == requiredValue;
                
            default:
                return true;
        }
    }
}
