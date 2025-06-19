using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Bureaucracy Scenario", menuName = "Paper Trail/Bureaucracy Scenario")]
public class BureaucracyScenario : ScriptableObject
{
    [Header("Scenario Info")]
    public string scenarioName;
    public string scenarioDescription;
    public int difficultyLevel = 1;
    
    [Header("Initial Documents")]
    public List<DocumentTemplate> startingDocuments = new List<DocumentTemplate>();
    
    [Header("Available Document Types")]
    public List<DocumentTemplate> availableDocuments = new List<DocumentTemplate>();
    
    [Header("French Citizens Database")]
    public List<FrenchCitizenData> citizenDatabase = new List<FrenchCitizenData>();
    
    [Header("Scenario Goals")]
    public int targetDocumentsProcessed = 10;
    public int targetBureaucracyScore = 1000;
    public float timeLimit = 300f; // 5 minutes
    
    [Header("Narrative Elements")]
    public List<string> progressiveNarrativeTexts = new List<string>();
    public List<string> absurdityEscalation = new List<string>();
}
