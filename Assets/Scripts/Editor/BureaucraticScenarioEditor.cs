#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BureaucraticScenario))]
public class BureaucraticScenarioEditor : Editor
{
    private bool showSteps = true;
    private bool showSuccessConditions = true;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        BureaucraticScenario scenario = (BureaucraticScenario)target;
        
        // Header
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("SCÉNARIO BUREAUCRATIQUE", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Générateur de Situations Kafkaïennes", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        // Boutons d'action
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Tester le Scénario"))
        {
            TestScenario(scenario);
        }
        
        if (GUILayout.Button("Valider la Logique"))
        {
            ValidateScenario(scenario);
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Informations de debug
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Informations de Debug", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Nombre d'étapes: {scenario.steps.Count}");
        EditorGUILayout.LabelField($"Conditions de succès: {scenario.successConditions.Count}");
        EditorGUILayout.LabelField($"Poids émotionnel: {scenario.emotionalWeight}/10");
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void TestScenario(BureaucraticScenario scenario)
    {
        Debug.Log($"Test du scénario: {scenario.scenarioTitle}");
        // Ici vous pourriez lancer une simulation du scénario
        if (Application.isPlaying && BureaucracyGameManager.Instance != null)
        {
            BureaucracyGameManager.Instance.StartScenario(scenario);
        }
    }
    
    private void ValidateScenario(BureaucraticScenario scenario)
    {
        bool isValid = true;
        
        if (string.IsNullOrEmpty(scenario.scenarioTitle))
        {
            Debug.LogError("Le scénario doit avoir un titre !");
            isValid = false;
        }
        
        if (scenario.steps.Count == 0)
        {
            Debug.LogError("Le scénario doit avoir au moins une étape !");
            isValid = false;
        }
        
        foreach (var step in scenario.steps)
        {
            if (step.requiredForm == null)
            {
                Debug.LogError($"L'étape '{step.stepName}' n'a pas de formulaire associé !");
                isValid = false;
            }
        }
        
        if (isValid)
        {
            Debug.Log("✓ Scénario valide !");
        }
    }
}


#endif // UNITY_EDITOR
