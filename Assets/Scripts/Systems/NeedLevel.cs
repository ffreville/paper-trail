using UnityEngine;
public class NeedLevel
{
    public PhysiologicalNeed needType;
    [Range(0f, 100f)]
    public float currentLevel = 100f;
    public float decreaseRate = 1f; // Points par seconde
    public float criticalThreshold = 20f;
    public float emergencyThreshold = 5f;

    [Header("Visual")]
    public Color normalColor = Color.green;
    public Color criticalColor = Color.yellow;
    public Color emergencyColor = Color.red;

    [Header("Effects")]
    public bool affectsProductivity = true;
    public float productivityPenalty = 0.5f; // Multiplicateur de score
}
