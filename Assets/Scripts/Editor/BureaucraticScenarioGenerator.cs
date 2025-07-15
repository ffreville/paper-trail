#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BureaucraticScenarioGenerator : EditorWindow
{
    private string[] departments = {"Administration", "Ressources Humaines", "Comptabilité", "Juridique", "IT", "Sécurité"};
    private string[] citizenRequests = {"Demande de permis", "Réclamation", "Demande d'aide", "Inscription", "Renouvellement"};
    private string[] complications = {"Document manquant", "Signature incorrecte", "Département fermé", "Système en panne"};
    
    [MenuItem("Bureaucracy/Scenario Generator")]
    public static void ShowWindow()
    {
        GetWindow<BureaucraticScenarioGenerator>("Générateur de Scénarios");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.LabelField("GÉNÉRATEUR DE SCÉNARIOS KAFKAÏENS", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Générer un Scénario Aléatoire"))
        {
            GenerateRandomScenario();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Générer une Série de Formulaires"))
        {
            GenerateFormSeries();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Créer un Incident Bureaucratique"))
        {
            CreateBureaucraticIncident();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void GenerateRandomScenario()
    {
        BureaucraticScenario scenario = CreateInstance<BureaucraticScenario>();
        
        scenario.scenarioTitle = $"Scénario {System.DateTime.Now:yyyyMMdd-HHmmss}";
        scenario.citizenRequest = citizenRequests[Random.Range(0, citizenRequests.Length)];
        scenario.scenarioDescription = $"Le citoyen souhaite faire une {scenario.citizenRequest.ToLower()}";
        scenario.emotionalWeight = Random.Range(3, 9);
        
        // Générer des étapes aléatoires
        int stepCount = Random.Range(3, 8);
        for (int i = 0; i < stepCount; i++)
        {
            ScenarioStep step = new ScenarioStep();
            step.stepName = $"Étape {i + 1}";
            step.instructions = $"Traiter le formulaire du département {departments[Random.Range(0, departments.Length)]}";
            scenario.steps.Add(step);
        }
        
        string path = EditorUtility.SaveFilePanelInProject("Nouveau Scénario", "RandomScenario", "asset", "Créer un scénario aléatoire");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(scenario, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Scénario généré: {scenario.scenarioTitle}");
        }
    }
    
    private void GenerateFormSeries()
    {
        int formCount = Random.Range(5, 12);
        List<BureaucraticForm> forms = new List<BureaucraticForm>();
        
        for (int i = 0; i < formCount; i++)
        {
            BureaucraticForm form = CreateInstance<BureaucraticForm>();
            form.formTitle = $"Formulaire {i + 1:D2}";
            form.formCode = $"FORM-{System.DateTime.Now:yyyyMMdd}-{i:D3}";
            form.department = departments[Random.Range(0, departments.Length)];
            form.processingTimeMinutes = Random.Range(5, 30);
            form.priorityLevel = Random.Range(1, 6);
            
            // Ajouter des champs aléatoires
            int fieldCount = Random.Range(3, 8);
            for (int j = 0; j < fieldCount; j++)
            {
                FormField field = GenerateRandomField(j);
                form.fields.Add(field);
            }
            
            forms.Add(form);
        }
        
        // Créer les cascades entre formulaires
        for (int i = 0; i < forms.Count - 1; i++)
        {
            FormCascadeRule rule = new FormCascadeRule();
            rule.ruleName = $"Cascade {i + 1}";
            rule.condition = new CascadeCondition { type = CascadeCondition.ConditionType.Always };
            rule.formsToTrigger = new List<BureaucraticForm> { forms[i + 1] };
            rule.triggerMessage = $"Formulaire {i + 2} requis suite à validation";
            
            forms[i].cascadeRules.Add(rule);
        }
        
        // Sauvegarder tous les formulaires
        string folderPath = EditorUtility.SaveFolderPanel("Dossier pour la série", "Assets", "FormSeries");
        if (!string.IsNullOrEmpty(folderPath))
        {
            string relativePath = folderPath.Replace(Application.dataPath, "Assets");
            
            foreach (var form in forms)
            {
                string assetPath = $"{relativePath}/{form.formTitle}.asset";
                AssetDatabase.CreateAsset(form, assetPath);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Série de {formCount} formulaires créée dans {relativePath}");
        }
    }
    
    private FormField GenerateRandomField(int index)
    {
        int fieldType = Random.Range(0, 5);
        
        switch (fieldType)
        {
            case 0:
                return new TextFormField
                {
                    fieldName = $"text_field_{index}",
                    label = $"Champ texte {index + 1}",
                    maxLength = Random.Range(50, 200),
                    isRequired = Random.value > 0.5f
                };
                
            case 1:
                return new NumberFormField
                {
                    fieldName = $"number_field_{index}",
                    label = $"Champ numérique {index + 1}",
                    minValue = 0,
                    maxValue = 1000,
                    isRequired = Random.value > 0.3f
                };
                
            case 2:
                return new DropdownFormField
                {
                    fieldName = $"dropdown_field_{index}",
                    label = $"Liste déroulante {index + 1}",
                    options = new string[] { "Option A", "Option B", "Option C" },
                    isRequired = Random.value > 0.4f
                };
                
            case 3:
                return new CheckboxFormField
                {
                    fieldName = $"checkbox_field_{index}",
                    label = $"Case à cocher {index + 1}",
                    isRequired = Random.value > 0.7f
                };
                
            case 4:
                return new DateFormField
                {
                    fieldName = $"date_field_{index}",
                    label = $"Date {index + 1}",
                    mustBeFuture = Random.value > 0.5f,
                    isRequired = Random.value > 0.4f
                };
                
            default:
                return new TextFormField
                {
                    fieldName = $"default_field_{index}",
                    label = $"Champ par défaut {index + 1}",
                    isRequired = true
                };
        }
    }
    
    private void CreateBureaucraticIncident()
    {
        Debug.Log("Création d'un incident bureaucratique...");
        
        string incidentType = complications[Random.Range(0, complications.Length)];
        string affectedDepartment = departments[Random.Range(0, departments.Length)];
        int severity = Random.Range(1, 6);
        
        Debug.LogWarning($"INCIDENT BUREAUCRATIQUE: {incidentType} dans le département {affectedDepartment}. Sévérité: {severity}/5");
        
        // Ici vous pourriez créer automatiquement des formulaires de gestion d'incident
    }
}


#endif // UNITY_EDITOR
