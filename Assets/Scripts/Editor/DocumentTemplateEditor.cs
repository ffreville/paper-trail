#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DocumentTemplate))]
public class DocumentTemplateEditor : Editor
{
    private DocumentTemplate documentTemplate;
    private bool showFormFields = true;
    private bool showTriggers = true;
    private bool showFrenchLocalization = true;
    private bool showPreview = false;

    private void OnEnable()
    {
        documentTemplate = (DocumentTemplate)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Header
        EditorGUILayout.Space();
        GUILayout.Label("📄 DOCUMENT TEMPLATE EDITOR", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Basic Info
        DrawBasicInfo();
        EditorGUILayout.Space();

        // Form Fields Section
        showFormFields = EditorGUILayout.Foldout(showFormFields, "📝 Form Fields Configuration", true);
        if (showFormFields)
        {
            DrawFormFieldsSection();
        }
        EditorGUILayout.Space();

        // Bureaucracy Settings
        DrawBureaucracySettings();
        EditorGUILayout.Space();

        // Triggers Section
        showTriggers = EditorGUILayout.Foldout(showTriggers, "⚡ Bureaucracy Triggers", true);
        if (showTriggers)
        {
            DrawTriggersSection();
        }
        EditorGUILayout.Space();

        // French Localization
        showFrenchLocalization = EditorGUILayout.Foldout(showFrenchLocalization, "🇫🇷 French Localization", true);
        if (showFrenchLocalization)
        {
            DrawFrenchLocalizationSection();
        }
        EditorGUILayout.Space();

        // Preview Section
        showPreview = EditorGUILayout.Foldout(showPreview, "👁️ Document Preview", true);
        if (showPreview)
        {
            DrawPreviewSection();
        }

        // Action Buttons
        EditorGUILayout.Space();
        DrawActionButtons();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(documentTemplate);
        }
    }

    private void DrawBasicInfo()
    {
        EditorGUILayout.LabelField("Basic Information", EditorStyles.boldLabel);
        
        documentTemplate.documentTitle = EditorGUILayout.TextField("Document Title", documentTemplate.documentTitle);
        documentTemplate.documentType = (DocumentType)EditorGUILayout.EnumPopup("Document Type", documentTemplate.documentType);
        
        EditorGUILayout.LabelField("Description:");
        documentTemplate.description = EditorGUILayout.TextArea(documentTemplate.description, GUILayout.Height(60));
        
        documentTemplate.documentIcon = (Sprite)EditorGUILayout.ObjectField("Document Icon", documentTemplate.documentIcon, typeof(Sprite), false);
    }

    private void DrawFormFieldsSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.LabelField($"Form Fields ({documentTemplate.formFields.Count})", EditorStyles.boldLabel);
        
        // Add new field button
        if (GUILayout.Button("+ Add New Field"))
        {
            documentTemplate.formFields.Add(new FormField
            {
                fieldName = "New Field",
                fieldType = FormFieldType.Text,
                isRequired = false
            });
        }

        // Display existing fields
        for (int i = 0; i < documentTemplate.formFields.Count; i++)
        {
            DrawFormField(i);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawFormField(int index)
    {
        FormField field = documentTemplate.formFields[index];
        
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        field.fieldName = EditorGUILayout.TextField("Field Name", field.fieldName);
        
        if (GUILayout.Button("❌", GUILayout.Width(30)))
        {
            documentTemplate.formFields.RemoveAt(index);
            return;
        }
        EditorGUILayout.EndHorizontal();

        field.fieldType = (FormFieldType)EditorGUILayout.EnumPopup("Field Type", field.fieldType);
        field.isRequired = EditorGUILayout.Toggle("Required", field.isRequired);
        field.placeholder = EditorGUILayout.TextField("Placeholder", field.placeholder);
        
        if (field.fieldType == FormFieldType.Text || field.fieldType == FormFieldType.TextArea)
        {
            field.maxLength = EditorGUILayout.IntField("Max Length", field.maxLength);
            field.validationRule = EditorGUILayout.TextField("Validation Rule", field.validationRule);
        }
        
        if (field.fieldType == FormFieldType.Dropdown)
        {
            EditorGUILayout.LabelField("Dropdown Options:");
            if (field.dropdownOptions == null) field.dropdownOptions = new string[0];
            
            for (int j = 0; j < field.dropdownOptions.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                field.dropdownOptions[j] = EditorGUILayout.TextField($"Option {j + 1}", field.dropdownOptions[j]);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    var list = new List<string>(field.dropdownOptions);
                    list.RemoveAt(j);
                    field.dropdownOptions = list.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("+ Add Option"))
            {
                var list = new List<string>(field.dropdownOptions);
                list.Add("New Option");
                field.dropdownOptions = list.ToArray();
            }
        }

        // Bureaucratic Complications
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🔥 Bureaucratic Complications", EditorStyles.boldLabel);
        field.triggersNewDocument = EditorGUILayout.Toggle("Triggers New Document", field.triggersNewDocument);
        
        if (field.triggersNewDocument)
        {
            field.triggerDocumentType = EditorGUILayout.TextField("Trigger Document Type", field.triggerDocumentType);
        }
        
        field.rejectionReason = EditorGUILayout.TextField("Rejection Reason", field.rejectionReason);
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawBureaucracySettings()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("🏛️ Bureaucracy Settings", EditorStyles.boldLabel);
        
        documentTemplate.requiresStamp = EditorGUILayout.Toggle("Requires Stamp", documentTemplate.requiresStamp);
        documentTemplate.requiresSignature = EditorGUILayout.Toggle("Requires Signature", documentTemplate.requiresSignature);
        documentTemplate.baseBureaucracyLevel = EditorGUILayout.IntSlider("Bureaucracy Level", documentTemplate.baseBureaucracyLevel, 1, 10);
        documentTemplate.processingTimeMinutes = EditorGUILayout.IntField("Processing Time (minutes)", documentTemplate.processingTimeMinutes);
        
        EditorGUILayout.Space();
        documentTemplate.rejectionProbability = EditorGUILayout.Slider("Rejection Probability", documentTemplate.rejectionProbability, 0f, 1f);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawTriggersSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.LabelField($"Bureaucracy Triggers ({documentTemplate.triggers.Count})", EditorStyles.boldLabel);
        
        if (GUILayout.Button("+ Add New Trigger"))
        {
            documentTemplate.triggers.Add(new BureaucracyTrigger
            {
                triggerName = "New Trigger",
                condition = TriggerCondition.Always,
                probability = 1f
            });
        }

        for (int i = 0; i < documentTemplate.triggers.Count; i++)
        {
            DrawTrigger(i);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawTrigger(int index)
    {
        BureaucracyTrigger trigger = documentTemplate.triggers[index];
        
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        trigger.triggerName = EditorGUILayout.TextField("Trigger Name", trigger.triggerName);
        if (GUILayout.Button("❌", GUILayout.Width(30)))
        {
            documentTemplate.triggers.RemoveAt(index);
            return;
        }
        EditorGUILayout.EndHorizontal();

        trigger.condition = (TriggerCondition)EditorGUILayout.EnumPopup("Condition", trigger.condition);
        trigger.conditionValue = EditorGUILayout.TextField("Condition Value", trigger.conditionValue);
        trigger.probability = EditorGUILayout.Slider("Probability", trigger.probability, 0f, 1f);
        
        EditorGUILayout.Space();
        trigger.bureaucracyScoreBonus = EditorGUILayout.IntField("Score Bonus", trigger.bureaucracyScoreBonus);
        trigger.triggerMessage = EditorGUILayout.TextField("Trigger Message", trigger.triggerMessage);
        
        EditorGUILayout.Space();
        trigger.canTriggerRecursively = EditorGUILayout.Toggle("Can Trigger Recursively", trigger.canTriggerRecursively);
        trigger.maxCascadeDepth = EditorGUILayout.IntField("Max Cascade Depth", trigger.maxCascadeDepth);
        
        // New Document Types
        EditorGUILayout.LabelField("Generated Documents:");
        if (trigger.newDocumentTypes == null) trigger.newDocumentTypes = new string[0];
        
        for (int j = 0; j < trigger.newDocumentTypes.Length; j++)
        {
            EditorGUILayout.BeginHorizontal();
            trigger.newDocumentTypes[j] = EditorGUILayout.TextField($"Document {j + 1}", trigger.newDocumentTypes[j]);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                var list = new List<string>(trigger.newDocumentTypes);
                list.RemoveAt(j);
                trigger.newDocumentTypes = list.ToArray();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        if (GUILayout.Button("+ Add Document Type"))
        {
            var list = new List<string>(trigger.newDocumentTypes);
            list.Add("New Document Type");
            trigger.newDocumentTypes = list.ToArray();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawFrenchLocalizationSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        documentTemplate.frenchTitle = EditorGUILayout.TextField("French Title", documentTemplate.frenchTitle);
        
        EditorGUILayout.LabelField("French Description:");
        documentTemplate.frenchDescription = EditorGUILayout.TextArea(documentTemplate.frenchDescription, GUILayout.Height(60));
        
        EditorGUILayout.LabelField("French Field Names:");
        
        // Ensure french field names list matches form fields count
        while (documentTemplate.frenchFieldNames.Count < documentTemplate.formFields.Count)
        {
            documentTemplate.frenchFieldNames.Add("");
        }
        while (documentTemplate.frenchFieldNames.Count > documentTemplate.formFields.Count)
        {
            documentTemplate.frenchFieldNames.RemoveAt(documentTemplate.frenchFieldNames.Count - 1);
        }
        
        for (int i = 0; i < documentTemplate.formFields.Count; i++)
        {
            if (i < documentTemplate.frenchFieldNames.Count)
            {
                documentTemplate.frenchFieldNames[i] = EditorGUILayout.TextField(
                    documentTemplate.formFields[i].fieldName + " (FR)", 
                    documentTemplate.frenchFieldNames[i]
                );
            }
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Document Preview", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField($"Title: {documentTemplate.documentTitle}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField($"Type: {documentTemplate.documentType}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField($"Fields: {documentTemplate.formFields.Count}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField($"Triggers: {documentTemplate.triggers.Count}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField($"Bureaucracy Level: {documentTemplate.baseBureaucracyLevel}", EditorStyles.wordWrappedLabel);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🎲 Generate Random French Data"))
        {
            GenerateRandomFrenchData();
        }
        
        if (GUILayout.Button("📋 Duplicate Template"))
        {
            DuplicateTemplate();
        }
        
        if (GUILayout.Button("🧪 Test Document"))
        {
            TestDocument();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void GenerateRandomFrenchData()
    {
        FrenchDataGenerator generator = Resources.Load<FrenchDataGenerator>("FrenchDataGenerator");
        if (generator != null)
        {
            var citizen = generator.GenerateRandomCitizen();
            
            // Auto-fill some French content
            if (string.IsNullOrEmpty(documentTemplate.frenchTitle))
            {
                documentTemplate.frenchTitle = "Formulaire de " + documentTemplate.documentTitle;
            }
            
            if (string.IsNullOrEmpty(documentTemplate.frenchDescription))
            {
                documentTemplate.frenchDescription = "Document officiel pour traitement administratif";
            }
            
            EditorUtility.SetDirty(documentTemplate);
        }
    }

    private void DuplicateTemplate()
    {
        string path = AssetDatabase.GetAssetPath(documentTemplate);
        string newPath = path.Replace(".asset", "_Copy.asset");
        AssetDatabase.CopyAsset(path, newPath);
        AssetDatabase.Refresh();
    }

    private void TestDocument()
    {
        Debug.Log($"Testing Document Template: {documentTemplate.documentTitle}");
        Debug.Log($"Fields: {documentTemplate.formFields.Count}");
        Debug.Log($"Triggers: {documentTemplate.triggers.Count}");
        Debug.Log($"Bureaucracy Level: {documentTemplate.baseBureaucracyLevel}");
    }
}

#endif
