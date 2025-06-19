using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BureaucracyScenarioStatus
{
    public string scenarioName;
    public int documentsProcessed;
    public int targetDocuments;
    public int currentScore;
    public int targetScore;
    public int activeCitizensCount;
    public int availableTemplatesCount;
    public float averageCitizenDesperation;
}
