#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BureaucraticForm))]
public class BureaucraticFormEditor : Editor
{
    private SerializedProperty formTitleProp;
    private SerializedProperty formCodeProp;
    private SerializedProperty departmentProp;
    private SerializedProperty descriptionProp;
    private SerializedProperty fieldsProp;
    private SerializedProperty cascadeRulesProp;
    
    private bool showFields = true;
    private bool showCascadeRules = true;
    private bool showValidationRules = true;
    
    private void OnEnable()
    {
        formTitleProp = serializedObject.FindProperty("formTitle");
        formCodeProp = serializedObject.FindProperty("formCode");
        departmentProp = serializedObject.FindProperty("department");
        descriptionProp = serializedObject.FindProperty("description");
        fieldsProp = serializedObject.FindProperty("fields");
        cascadeRulesProp = serializedObject.FindProperty("cascadeRules");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        BureaucraticForm form = (BureaucraticForm)target;
        
        // Header avec style bureaucratique
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("FORMULAIRE BUREAUCRATIQUE", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Système Administratif Intégré", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Informations principales
        EditorGUILayout.PropertyField(formTitleProp);
        EditorGUILayout.PropertyField(formCodeProp);
        EditorGUILayout.PropertyField(departmentProp);
        EditorGUILayout.PropertyField(descriptionProp);
        
        EditorGUILayout.Space();
        
        // Propriétés visuelles
        EditorGUILayout.LabelField("Propriétés Visuelles", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("formBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inkColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasWatermark"));
        
        EditorGUILayout.Space();
        
        // Propriétés bureaucratiques
        EditorGUILayout.LabelField("Propriétés Bureaucratiques", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("processingTimeMinutes"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("priorityLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiresStamp"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiresSignature"));
        
        EditorGUILayout.Space();
        
        // Champs du formulaire
        showFields = EditorGUILayout.Foldout(showFields, "Champs du Formulaire", true);
        if (showFields)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Boutons pour ajouter différents types de champs
            EditorGUILayout.LabelField("Ajouter un champ :", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Texte")) AddField<TextFormField>(form);
            if (GUILayout.Button("Nombre")) AddField<NumberFormField>(form);
            if (GUILayout.Button("Liste")) AddField<DropdownFormField>(form);
            if (GUILayout.Button("Case")) AddField<CheckboxFormField>(form);
            if (GUILayout.Button("Date")) AddField<DateFormField>(form);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            // Affichage des champs existants
            for (int i = 0; i < form.fields.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField($"Champ {i + 1}: {form.fields[i].GetType().Name}", EditorStyles.boldLabel);
                
                if (GUILayout.Button("↑", GUILayout.Width(20)) && i > 0)
                {
                    var temp = form.fields[i];
                    form.fields[i] = form.fields[i - 1];
                    form.fields[i - 1] = temp;
                }
                
                if (GUILayout.Button("↓", GUILayout.Width(20)) && i < form.fields.Count - 1)
                {
                    var temp = form.fields[i];
                    form.fields[i] = form.fields[i + 1];
                    form.fields[i + 1] = temp;
                }
                
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    form.fields.RemoveAt(i);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
                
                // Propriétés du champ
                DrawFieldProperties(form.fields[i]);
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space();
        
        // Règles de cascade
        showCascadeRules = EditorGUILayout.Foldout(showCascadeRules, "Règles de Cascade", true);
        if (showCascadeRules)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(cascadeRulesProp, true);
            EditorGUILayout.EndVertical();
        }
        
        // Règles de validation
        showValidationRules = EditorGUILayout.Foldout(showValidationRules, "Règles de Validation", true);
        if (showValidationRules)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("validationRules"), true);
            EditorGUILayout.EndVertical();
        }
        
        // Boutons d'action
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Prévisualiser le Formulaire"))
        {
            PreviewForm(form);
        }
        
        if (GUILayout.Button("Dupliquer le Formulaire"))
        {
            DuplicateForm(form);
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Informations de debug
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Informations de Debug", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Nombre de champs: {form.fields.Count}");
        EditorGUILayout.LabelField($"Formulaire valide: {form.IsFormValid()}");
        EditorGUILayout.LabelField($"Formulaires déclenchés: {form.GetTriggeredForms().Count}");
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void AddField<T>(BureaucraticForm form) where T : FormField, new()
    {
        T newField = new T();
        newField.fieldName = $"field_{form.fields.Count}";
        newField.label = $"Champ {form.fields.Count + 1}";
        form.fields.Add(newField);
        EditorUtility.SetDirty(form);
    }
    
    private void DrawFieldProperties(FormField field)
    {
        field.fieldName = EditorGUILayout.TextField("Nom du champ", field.fieldName);
        field.label = EditorGUILayout.TextField("Label", field.label);
        field.isRequired = EditorGUILayout.Toggle("Obligatoire", field.isRequired);
        field.isReadOnly = EditorGUILayout.Toggle("Lecture seule", field.isReadOnly);
        
        // Propriétés spécifiques selon le type
        switch (field)
        {
            case TextFormField textField:
                textField.maxLength = EditorGUILayout.IntField("Longueur max", textField.maxLength);
                textField.placeholder = EditorGUILayout.TextField("Placeholder", textField.placeholder);
                textField.textValue = EditorGUILayout.TextField("Valeur par défaut", textField.textValue);
                break;
                
            case NumberFormField numberField:
                numberField.minValue = EditorGUILayout.FloatField("Valeur min", numberField.minValue);
                numberField.maxValue = EditorGUILayout.FloatField("Valeur max", numberField.maxValue);
                numberField.numberValue = EditorGUILayout.FloatField("Valeur par défaut", numberField.numberValue);
                break;
                
            case DropdownFormField dropdownField:
                EditorGUILayout.LabelField("Options:");
                if (dropdownField.options == null) dropdownField.options = new string[0];
                
                for (int i = 0; i < dropdownField.options.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    dropdownField.options[i] = EditorGUILayout.TextField($"Option {i + 1}", dropdownField.options[i]);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        var newOptions = new string[dropdownField.options.Length - 1];
                        System.Array.Copy(dropdownField.options, 0, newOptions, 0, i);
                        System.Array.Copy(dropdownField.options, i + 1, newOptions, i, dropdownField.options.Length - i - 1);
                        dropdownField.options = newOptions;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                if (GUILayout.Button("Ajouter une option"))
                {
                    var newOptions = new string[dropdownField.options.Length + 1];
                    System.Array.Copy(dropdownField.options, newOptions, dropdownField.options.Length);
                    newOptions[dropdownField.options.Length] = "Nouvelle option";
                    dropdownField.options = newOptions;
                }
                break;
                
            case CheckboxFormField checkboxField:
                checkboxField.isChecked = EditorGUILayout.Toggle("Coché par défaut", checkboxField.isChecked);
                break;
                
            case DateFormField dateField:
                dateField.dateValue = EditorGUILayout.TextField("Date par défaut", dateField.dateValue);
                dateField.mustBeFuture = EditorGUILayout.Toggle("Doit être future", dateField.mustBeFuture);
                dateField.mustBePast = EditorGUILayout.Toggle("Doit être passée", dateField.mustBePast);
                break;
        }
    }
    
    private void PreviewForm(BureaucraticForm form)
    {
        Debug.Log($"Prévisualisation du formulaire: {form.formTitle}");
        // Ici vous pourriez ouvrir une fenêtre de prévisualisation
    }
    
    private void DuplicateForm(BureaucraticForm form)
    {
        string path = AssetDatabase.GetAssetPath(form);
        string newPath = path.Replace(".asset", "_Copy.asset");
        AssetDatabase.CopyAsset(path, newPath);
        AssetDatabase.Refresh();
    }
}
#endif // UNITY_EDITOR
