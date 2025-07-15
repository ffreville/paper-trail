#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BureaucraticFormWindow : EditorWindow
{
    private BureaucraticForm selectedForm;
    private Vector2 scrollPosition;
    
    [MenuItem("Bureaucracy/Form Designer")]
    public static void ShowWindow()
    {
        GetWindow<BureaucraticFormWindow>("Concepteur de Formulaires");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // Header
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        EditorGUILayout.LabelField("CONCEPTEUR DE FORMULAIRES BUREAUCRATIQUES", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Sélection du formulaire
        selectedForm = (BureaucraticForm)EditorGUILayout.ObjectField("Formulaire à éditer", selectedForm, typeof(BureaucraticForm), false);
        
        if (selectedForm == null)
        {
            EditorGUILayout.HelpBox("Sélectionnez un formulaire ou créez-en un nouveau.", MessageType.Info);
            
            if (GUILayout.Button("Créer un nouveau formulaire"))
            {
                CreateNewForm();
            }
            
            EditorGUILayout.EndVertical();
            return;
        }
        
        EditorGUILayout.Space();
        
        // Interface de design
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawFormDesigner();
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    private void CreateNewForm()
    {
        BureaucraticForm newForm = CreateInstance<BureaucraticForm>();
        newForm.formTitle = "Nouveau Formulaire";
        newForm.formCode = $"FORM-{System.DateTime.Now:yyyyMMdd}-{Random.Range(1000, 9999)}";
        newForm.department = "Administration Générale";
        
        string path = EditorUtility.SaveFilePanelInProject("Nouveau Formulaire", "NewForm", "asset", "Créer un nouveau formulaire");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newForm, path);
            AssetDatabase.SaveAssets();
            selectedForm = newForm;
        }
    }
    
    private void DrawFormDesigner()
    {
        // Informations générales
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Informations Générales", EditorStyles.boldLabel);
        
        selectedForm.formTitle = EditorGUILayout.TextField("Titre", selectedForm.formTitle);
        selectedForm.formCode = EditorGUILayout.TextField("Code", selectedForm.formCode);
        selectedForm.department = EditorGUILayout.TextField("Département", selectedForm.department);
        selectedForm.description = EditorGUILayout.TextArea(selectedForm.description, GUILayout.Height(60));
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Aperçu du formulaire
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Aperçu du Formulaire", EditorStyles.boldLabel);
        
        // Simuler l'apparence du formulaire
        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginVertical("Box");
        
        EditorGUILayout.LabelField(selectedForm.formTitle, EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Code: {selectedForm.formCode}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Département: {selectedForm.department}", EditorStyles.miniLabel);
        
        EditorGUILayout.Space();
        
        foreach (var field in selectedForm.fields)
        {
            DrawFieldPreview(field);
        }
        
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
        
        // Outils de gestion
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Sauvegarder"))
        {
            EditorUtility.SetDirty(selectedForm);
            AssetDatabase.SaveAssets();
        }
        
        if (GUILayout.Button("Réinitialiser"))
        {
            // Logique de réinitialisation
        }
        
        if (GUILayout.Button("Exporter"))
        {
            ExportFormToJSON();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawFieldPreview(FormField field)
    {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(field.label, GUILayout.Width(120));
        
        switch (field)
        {
            case TextFormField textField:
                EditorGUILayout.TextField(textField.textValue);
                break;
                
            case NumberFormField numberField:
                EditorGUILayout.FloatField(numberField.numberValue);
                break;
                
            case DropdownFormField dropdownField:
                if (dropdownField.options != null && dropdownField.options.Length > 0)
                {
                    EditorGUILayout.Popup(dropdownField.selectedIndex, dropdownField.options);
                }
                break;
                
            case CheckboxFormField checkboxField:
                EditorGUILayout.Toggle(checkboxField.isChecked);
                break;
                
            case DateFormField dateField:
                EditorGUILayout.TextField(dateField.dateValue);
                break;
        }
        
        if (field.isRequired)
        {
            EditorGUILayout.LabelField("*", GUILayout.Width(10));
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void ExportFormToJSON()
    {
        string json = JsonUtility.ToJson(selectedForm, true);
        string path = EditorUtility.SaveFilePanel("Exporter le formulaire", "", selectedForm.formTitle, "json");
        
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Formulaire exporté vers: {path}");
        }
    }
}

#endif // UNITY_EDITOR
