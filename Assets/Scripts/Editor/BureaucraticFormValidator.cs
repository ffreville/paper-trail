#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BureaucraticFormValidator : EditorWindow
{
    private BureaucraticForm[] allForms;
    private Vector2 scrollPosition;
    
    [MenuItem("Bureaucracy/Form Validator")]
    public static void ShowWindow()
    {
        GetWindow<BureaucraticFormValidator>("Validateur de Formulaires");
    }
    
    private void OnEnable()
    {
        RefreshFormList();
    }
    
    private void RefreshFormList()
    {
        string[] guids = AssetDatabase.FindAssets("t:BureaucraticForm");
        allForms = new BureaucraticForm[guids.Length];
        
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            allForms[i] = AssetDatabase.LoadAssetAtPath<BureaucraticForm>(path);
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.LabelField("VALIDATEUR DE FORMULAIRES", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Actualiser la liste"))
        {
            RefreshFormList();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Valider tous les formulaires"))
        {
            ValidateAllForms();
        }
        
        EditorGUILayout.Space();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (var form in allForms)
        {
            if (form != null)
            {
                DrawFormValidationStatus(form);
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawFormValidationStatus(BureaucraticForm form)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        // Icône de statut
        bool isValid = ValidateForm(form);
        GUI.color = isValid ? Color.green : Color.red;
        EditorGUILayout.LabelField(isValid ? "✓" : "✗", GUILayout.Width(20));
        GUI.color = Color.white;
        
        // Nom du formulaire
        EditorGUILayout.LabelField(form.formTitle, EditorStyles.boldLabel);
        
        // Code du formulaire
        EditorGUILayout.LabelField(form.formCode, EditorStyles.miniLabel, GUILayout.Width(100));
        
        // Bouton de sélection
        if (GUILayout.Button("Sélectionner", GUILayout.Width(80)))
        {
            Selection.activeObject = form;
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private bool ValidateForm(BureaucraticForm form)
    {
        if (string.IsNullOrEmpty(form.formTitle)) return false;
        if (string.IsNullOrEmpty(form.formCode)) return false;
        if (form.fields.Count == 0) return false;
        
        foreach (var field in form.fields)
        {
            if (string.IsNullOrEmpty(field.fieldName)) return false;
            if (string.IsNullOrEmpty(field.label)) return false;
        }
        
        return true;
    }
    
    private void ValidateAllForms()
    {
        int validForms = 0;
        int invalidForms = 0;
        
        foreach (var form in allForms)
        {
            if (form != null)
            {
                if (ValidateForm(form))
                {
                    validForms++;
                }
                else
                {
                    invalidForms++;
                    Debug.LogWarning($"Formulaire invalide: {form.name}");
                }
            }
        }
        
        Debug.Log($"Validation terminée: {validForms} formulaires valides, {invalidForms} formulaires invalides");
    }
}

#endif // UNITY_EDITOR
