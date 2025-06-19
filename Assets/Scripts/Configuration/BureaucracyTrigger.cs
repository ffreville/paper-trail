using UnityEngine;

[System.Serializable]
public class BureaucracyTrigger
{
    public string triggerName;
    public TriggerCondition condition;
    public string conditionValue;
    public float probability = 1f;
    
    [Header("Generated Documents")]
    public string[] newDocumentTypes;
    public int bureaucracyScoreBonus;
    public string triggerMessage;
    
    [Header("Cascade Settings")]
    public bool canTriggerRecursively;
    public int maxCascadeDepth = 5;
}
