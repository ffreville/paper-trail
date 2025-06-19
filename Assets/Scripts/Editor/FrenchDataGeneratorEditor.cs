#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(FrenchDataGenerator))]
public class FrenchDataGeneratorEditor : Editor
{
    private FrenchDataGenerator generator;
    private FrenchCitizenData previewCitizen;

    private void OnEnable()
    {
        generator = (FrenchDataGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        GUILayout.Label("🇫🇷 FRENCH DATA GENERATOR", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawDatabaseInfo();
        DrawGenerationSettings();
        DrawPreviewSection();
        DrawActionButtons();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(generator);
        }
    }

    private void DrawDatabaseInfo()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Database Statistics", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField($"First Names: {generator.frenchFirstNames.Count}");
        EditorGUILayout.LabelField($"Last Names: {generator.frenchLastNames.Count}");
        EditorGUILayout.LabelField($"Cities: {generator.frenchCities.Count}");
        EditorGUILayout.LabelField($"Streets: {generator.frenchStreets.Count}");
        EditorGUILayout.LabelField($"Professions: {generator.frenchProfessions.Count}");
        EditorGUILayout.LabelField($"Request Types: {generator.frenchRequestTypes.Count}");
        
        int totalCombinations = generator.frenchFirstNames.Count * generator.frenchLastNames.Count;
        EditorGUILayout.LabelField($"Possible Name Combinations: {totalCombinations:N0}");
        
        EditorGUILayout.EndVertical();
    }

    private void DrawGenerationSettings()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
        
        generator.batchSize = EditorGUILayout.IntSlider("Batch Size", generator.batchSize, 1, 200);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Preview Generated Citizen", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🎲 Generate Preview Citizen"))
        {
            previewCitizen = generator.GenerateRandomCitizen();
        }
        
        if (previewCitizen != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField($"👤 {previewCitizen.firstName} {previewCitizen.lastName}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"📧 Email: {previewCitizen.email}");
            EditorGUILayout.LabelField($"📞 Phone: {previewCitizen.phoneNumber}");
            EditorGUILayout.LabelField($"🏠 Address: {previewCitizen.address}");
            EditorGUILayout.LabelField($"🆔 SSN: {previewCitizen.socialSecurityNumber}");
            EditorGUILayout.LabelField($"🎂 Birth: {previewCitizen.birthDate} in {previewCitizen.birthPlace}");
            EditorGUILayout.LabelField($"💼 Profession: {previewCitizen.profession}");
            EditorGUILayout.LabelField($"😤 Desperation: {previewCitizen.desperationLevel:P0}");
            EditorGUILayout.LabelField($"📋 Previous Requests: {previewCitizen.previousRequestsCount}");
            
            if (previewCitizen.commonRequests.Count > 0)
            {
                EditorGUILayout.LabelField("Common Requests:");
                foreach (string request in previewCitizen.commonRequests)
                {
                    EditorGUILayout.LabelField($"  • {request}");
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("📊 Generate Statistics"))
        {
            GenerateStatistics();
        }
        
        if (GUILayout.Button("💾 Export Database"))
        {
            ExportDatabase();
        }
        
        if (GUILayout.Button("📝 Add Custom Data"))
        {
            AddCustomDataWindow.ShowWindow(generator);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void GenerateStatistics()
    {
        var batch = generator.GenerateBatch();
        
        string stats = "=== FRENCH DATA GENERATION STATISTICS ===\n\n";
        stats += $"Generated {batch.Count} citizens\n\n";
        
        // Profession distribution
        var professionCount = new Dictionary<string, int>();
        foreach (var citizen in batch)
        {
            if (professionCount.ContainsKey(citizen.profession))
                professionCount[citizen.profession]++;
            else
                professionCount[citizen.profession] = 1;
        }
        
        stats += "PROFESSION DISTRIBUTION:\n";
        foreach (var kvp in professionCount.OrderByDescending(x => x.Value))
        {
            stats += $"  {kvp.Key}: {kvp.Value} ({kvp.Value * 100.0 / batch.Count:F1}%)\n";
        }
        
        // Average desperation
        float avgDesperation = batch.Average(c => c.desperationLevel);
        stats += $"\nAVERAGE DESPERATION: {avgDesperation:P1}\n";
        
        // Request types
        var allRequests = new List<string>();
        foreach (var citizen in batch)
        {
            allRequests.AddRange(citizen.commonRequests);
        }
        
        var requestCount = new Dictionary<string, int>();
        foreach (string request in allRequests)
        {
            if (requestCount.ContainsKey(request))
                requestCount[request]++;
            else
                requestCount[request] = 1;
        }
        
        stats += "\nMOST COMMON REQUESTS:\n";
        foreach (var kvp in requestCount.OrderByDescending(x => x.Value).Take(10))
        {
            stats += $"  {kvp.Key}: {kvp.Value} times\n";
        }
        
        Debug.Log(stats);
    }

    private void ExportDatabase()
    {
        var batch = generator.GenerateBatch();
        string json = JsonUtility.ToJson(new SerializableList<FrenchCitizenData>(batch), true);
        
        string path = EditorUtility.SaveFilePanel("Export French Citizens Database", "", "FrenchCitizens.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Database exported to: {path}");
        }
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;
    
    public SerializableList(List<T> items)
    {
        this.items = items;
    }
}

public class AddCustomDataWindow : EditorWindow
{
    private FrenchDataGenerator generator;
    private string newFirstName = "";
    private string newLastName = "";
    private string newCity = "";
    private string newStreet = "";
    private string newProfession = "";
    private string newRequestType = "";
    
    public static void ShowWindow(FrenchDataGenerator gen)
    {
        AddCustomDataWindow window = GetWindow<AddCustomDataWindow>("Add Custom French Data");
        window.generator = gen;
        window.Show();
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Add Custom French Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Names", EditorStyles.boldLabel);
        newFirstName = EditorGUILayout.TextField("First Name", newFirstName);
        if (GUILayout.Button("Add First Name") && !string.IsNullOrEmpty(newFirstName))
        {
            generator.frenchFirstNames.Add(newFirstName);
            newFirstName = "";
            EditorUtility.SetDirty(generator);
        }
        
        newLastName = EditorGUILayout.TextField("Last Name", newLastName);
        if (GUILayout.Button("Add Last Name") && !string.IsNullOrEmpty(newLastName))
        {
            generator.frenchLastNames.Add(newLastName);
            newLastName = "";
            EditorUtility.SetDirty(generator);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Locations", EditorStyles.boldLabel);
        newCity = EditorGUILayout.TextField("City", newCity);
        if (GUILayout.Button("Add City") && !string.IsNullOrEmpty(newCity))
        {
            generator.frenchCities.Add(newCity);
            newCity = "";
            EditorUtility.SetDirty(generator);
        }
        
        newStreet = EditorGUILayout.TextField("Street", newStreet);
        if (GUILayout.Button("Add Street") && !string.IsNullOrEmpty(newStreet))
        {
            generator.frenchStreets.Add(newStreet);
            newStreet = "";
            EditorUtility.SetDirty(generator);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Other Data", EditorStyles.boldLabel);
        newProfession = EditorGUILayout.TextField("Profession", newProfession);
        if (GUILayout.Button("Add Profession") && !string.IsNullOrEmpty(newProfession))
        {
            generator.frenchProfessions.Add(newProfession);
            newProfession = "";
            EditorUtility.SetDirty(generator);
        }
        
        newRequestType = EditorGUILayout.TextField("Request Type", newRequestType);
        if (GUILayout.Button("Add Request Type") && !string.IsNullOrEmpty(newRequestType))
        {
            generator.frenchRequestTypes.Add(newRequestType);
            newRequestType = "";
            EditorUtility.SetDirty(generator);
        }
        EditorGUILayout.EndVertical();
    }
}
#endif
