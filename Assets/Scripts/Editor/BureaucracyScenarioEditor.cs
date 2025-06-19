#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BureaucracyScenario))]
public class BureaucracyScenarioEditor : Editor
{
    private BureaucracyScenario scenario;
    private bool showCitizenDatabase = false;

    private void OnEnable()
    {
        scenario = (BureaucracyScenario)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        GUILayout.Label("🏛️ BUREAUCRACY SCENARIO EDITOR", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawScenarioInfo();
        DrawDocumentLists();
        DrawScenarioGoals();
        DrawNarrativeElements();
        
        showCitizenDatabase = EditorGUILayout.Foldout(showCitizenDatabase, "👥 French Citizens Database", true);
        if (showCitizenDatabase)
        {
            DrawCitizenDatabase();
        }

        DrawScenarioActions();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(scenario);
        }
    }

    private void DrawScenarioInfo()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Scenario Information", EditorStyles.boldLabel);
        
        scenario.scenarioName = EditorGUILayout.TextField("Scenario Name", scenario.scenarioName);
        scenario.difficultyLevel = EditorGUILayout.IntSlider("Difficulty Level", scenario.difficultyLevel, 1, 10);
        
        EditorGUILayout.LabelField("Description:");
        scenario.scenarioDescription = EditorGUILayout.TextArea(scenario.scenarioDescription, GUILayout.Height(60));
        
        EditorGUILayout.EndVertical();
    }

    private void DrawDocumentLists()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Document Configuration", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startingDocuments"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("availableDocuments"), true);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawScenarioGoals()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Scenario Goals", EditorStyles.boldLabel);
        
        scenario.targetDocumentsProcessed = EditorGUILayout.IntField("Target Documents", scenario.targetDocumentsProcessed);
        scenario.targetBureaucracyScore = EditorGUILayout.IntField("Target Score", scenario.targetBureaucracyScore);
        scenario.timeLimit = EditorGUILayout.FloatField("Time Limit (seconds)", scenario.timeLimit);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawNarrativeElements()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Narrative Elements", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("progressiveNarrativeTexts"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("absurdityEscalation"), true);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawCitizenDatabase()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.LabelField($"Citizens Database ({scenario.citizenDatabase.Count})", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎲 Generate Random Citizens"))
        {
            GenerateRandomCitizens();
        }
        if (GUILayout.Button("🗑️ Clear Database"))
        {
            if (EditorUtility.DisplayDialog("Clear Citizens Database", 
                "Are you sure you want to clear all citizens?", "Yes", "Cancel"))
            {
                scenario.citizenDatabase.Clear();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Show first few citizens as preview
        int displayCount = Mathf.Min(5, scenario.citizenDatabase.Count);
        for (int i = 0; i < displayCount; i++)
        {
            var citizen = scenario.citizenDatabase[i];
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{citizen.firstName} {citizen.lastName}", EditorStyles.boldLabel);
            if (GUILayout.Button("❌", GUILayout.Width(25)))
            {
                scenario.citizenDatabase.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField($"📧 {citizen.email}");
            EditorGUILayout.LabelField($"🏠 {citizen.address}");
            EditorGUILayout.LabelField($"💼 {citizen.profession}");
            EditorGUILayout.LabelField($"😤 Desperation: {citizen.desperationLevel:P0}");
            
            if (citizen.commonRequests.Count > 0)
            {
                EditorGUILayout.LabelField($"📋 Requests: {string.Join(", ", citizen.commonRequests)}");
            }
            
            EditorGUILayout.EndVertical();
        }
        
        if (scenario.citizenDatabase.Count > displayCount)
        {
            EditorGUILayout.LabelField($"... and {scenario.citizenDatabase.Count - displayCount} more citizens");
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawScenarioActions()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🚀 Test Scenario"))
        {
            TestScenario();
        }
        
        if (GUILayout.Button("📊 Generate Report"))
        {
            GenerateScenarioReport();
        }
        
        if (GUILayout.Button("💾 Export Scenario"))
        {
            ExportScenario();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void GenerateRandomCitizens()
    {
        FrenchDataGenerator generator = Resources.Load<FrenchDataGenerator>("FrenchDataGenerator");
        if (generator == null)
        {
            EditorUtility.DisplayDialog("Error", "FrenchDataGenerator not found in Resources folder!", "OK");
            return;
        }

        int count = EditorUtility.DisplayDialogComplex(
            "Generate Citizens",
            "How many citizens would you like to generate?",
            "10 Citizens", "50 Citizens", "100 Citizens"
        );

        int citizenCount = count == 0 ? 10 : count == 1 ? 50 : 100;
        
        for (int i = 0; i < citizenCount; i++)
        {
            scenario.citizenDatabase.Add(generator.GenerateRandomCitizen());
        }

        EditorUtility.SetDirty(scenario);
        Debug.Log($"Generated {citizenCount} random French citizens!");
    }

    private void TestScenario()
    {
        Debug.Log($"Testing Scenario: {scenario.scenarioName}");
        Debug.Log($"Difficulty: {scenario.difficultyLevel}/10");
        Debug.Log($"Starting Documents: {scenario.startingDocuments.Count}");
        Debug.Log($"Available Documents: {scenario.availableDocuments.Count}");
        Debug.Log($"Citizens: {scenario.citizenDatabase.Count}");
        Debug.Log($"Target: {scenario.targetDocumentsProcessed} docs, {scenario.targetBureaucracyScore} score");
    }

    private void GenerateScenarioReport()
    {
        string report = $"=== BUREAUCRACY SCENARIO REPORT ===\n\n";
        report += $"Scenario: {scenario.scenarioName}\n";
        report += $"Difficulty: {scenario.difficultyLevel}/10\n";
        report += $"Description: {scenario.scenarioDescription}\n\n";
        
        report += $"DOCUMENTS:\n";
        report += $"- Starting: {scenario.startingDocuments.Count}\n";
        report += $"- Available: {scenario.availableDocuments.Count}\n\n";
        
        report += $"GOALS:\n";
        report += $"- Target Documents: {scenario.targetDocumentsProcessed}\n";
        report += $"- Target Score: {scenario.targetBureaucracyScore}\n";
        report += $"- Time Limit: {scenario.timeLimit} seconds\n\n";
        
        report += $"CITIZENS DATABASE:\n";
        report += $"- Total Citizens: {scenario.citizenDatabase.Count}\n";
        
        if (scenario.citizenDatabase.Count > 0)
        {
            var avgDesperation = 0f;
            var totalRequests = 0;
            foreach (var citizen in scenario.citizenDatabase)
            {
                avgDesperation += citizen.desperationLevel;
                totalRequests += citizen.commonRequests.Count;
            }
            avgDesperation /= scenario.citizenDatabase.Count;
            
            report += $"- Average Desperation: {avgDesperation:P1}\n";
            report += $"- Total Requests: {totalRequests}\n";
        }
        
        report += $"\nNARRATIVE ELEMENTS:\n";
        report += $"- Progressive Texts: {scenario.progressiveNarrativeTexts.Count}\n";
        report += $"- Absurdity Escalation: {scenario.absurdityEscalation.Count}\n";
        
        Debug.Log(report);
        
        // Save to file
        string path = EditorUtility.SaveFilePanel("Save Scenario Report", "", $"{scenario.scenarioName}_Report.txt", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, report);
            Debug.Log($"Report saved to: {path}");
        }
    }

    private void ExportScenario()
    {
        string json = JsonUtility.ToJson(scenario, true);
        string path = EditorUtility.SaveFilePanel("Export Scenario", "", $"{scenario.scenarioName}.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Scenario exported to: {path}");
        }
    }
}

#endif
