using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DocumentUI : MonoBehaviour
{
    [Header("Document Display")]
    public TextMeshProUGUI documentTitleText;
    public TextMeshProUGUI citizenNameText;
    public TextMeshProUGUI documentTypeText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI detailsText;

    [Header("Form Fields")]
    public Transform formFieldsParent;
    public GameObject inputFieldPrefab;

    [Header("Actions")]
    public Button processButton;
    public Button rejectButton;
    public Button stampButton;
    public Button signButton;

    [Header("Visual Effects")]
    public GameObject stampEffect;
    public GameObject signatureEffect;
    public AudioSource stampSound;

    private DocumentData currentDocument;
    private DocumentManager documentManager;
    private List<TMP_InputField> formInputs = new List<TMP_InputField>();

    private void Start()
    {
        documentManager = FindObjectOfType<DocumentManager>();

        SetupButtons();
        ClearDisplay();
    }

    private void SetupButtons()
    {
        if (processButton != null)
        {
            processButton.onClick.AddListener(ProcessCurrentDocument);
        }

        if (rejectButton != null)
        {
            rejectButton.onClick.AddListener(RejectCurrentDocument);
        }

        if (stampButton != null)
        {
            stampButton.onClick.AddListener(ApplyStamp);
        }

        if (signButton != null)
        {
            signButton.onClick.AddListener(ApplySignature);
        }
    }

    public void DisplayDocument(DocumentData document)
    {
        currentDocument = document;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (currentDocument == null)
        {
            ClearDisplay();
            return;
        }

        // Update document info
        if (documentTitleText != null)
        {
            documentTitleText.text = currentDocument.documentTitle;
        }

        if (citizenNameText != null)
        {
            citizenNameText.text = $"Citizen: {currentDocument.citizenName}";
        }

        if (documentTypeText != null)
        {
            documentTypeText.text = $"Type: {currentDocument.documentType}";
        }

        if (statusText != null)
        {
            statusText.text = $"Status: {currentDocument.status}";
        }

        if (detailsText != null)
        {
            detailsText.text = currentDocument.requestDetails;
        }

        // Generate form fields
        GenerateFormFields();

        // Update action buttons
        UpdateActionButtons();
    }

    private void GenerateFormFields()
    {
        // Clear existing form inputs
        ClearFormFields();

        if (currentDocument == null || formFieldsParent == null || inputFieldPrefab == null)
            return;

        foreach (var field in currentDocument.formFields)
        {
            CreateFormField(field.Key, field.Value);
        }
    }

    private void ClearFormFields()
    {
        foreach (Transform child in formFieldsParent)
        {
            Destroy(child.gameObject);
        }
        formInputs.Clear();
    }

    private void CreateFormField(string fieldName, string fieldValue)
    {
        GameObject fieldObj = Instantiate(inputFieldPrefab, formFieldsParent);

        // Setup label
        TextMeshProUGUI label = fieldObj.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = fieldName + ":";
        }

        // Setup input field
        TMP_InputField input = fieldObj.GetComponentInChildren<TMP_InputField>();
        if (input != null)
        {
            input.text = fieldValue;
            formInputs.Add(input);

            // Store field name for later reference
            input.name = fieldName;
        }
    }

    private void UpdateActionButtons()
    {
        if (currentDocument == null)
        {
            SetButtonsActive(false);
            return;
        }

        SetButtonsActive(true);

        // Enable/disable stamp button
        if (stampButton != null)
        {
            stampButton.interactable = currentDocument.requiresStamp;
        }

        // Enable/disable signature button
        if (signButton != null)
        {
            signButton.interactable = currentDocument.requiresSignature;
        }
    }

    private void SetButtonsActive(bool active)
    {
        if (processButton != null) processButton.gameObject.SetActive(active);
        if (rejectButton != null) rejectButton.gameObject.SetActive(active);
        if (stampButton != null) stampButton.gameObject.SetActive(active);
        if (signButton != null) signButton.gameObject.SetActive(active);
    }

    private void ProcessCurrentDocument()
    {
        if (currentDocument == null || documentManager == null) return;

        // Save form field values
        SaveFormFields();

        // Process the document
        documentManager.ProcessDocument(currentDocument);

        // Clear display
        ClearDisplay();
    }

    private void RejectCurrentDocument()
    {
        if (currentDocument == null || documentManager == null) return;

        documentManager.RejectDocument(currentDocument, "Document rejected by officer");
        ClearDisplay();
    }

    private void ApplyStamp()
    {
        if (currentDocument == null) return;

        // Visual effect
        if (stampEffect != null)
        {
            GameObject effect = Instantiate(stampEffect, transform);
            Destroy(effect, 2f);
        }

        // Sound effect
        if (stampSound != null)
        {
            stampSound.Play();
        }

        Debug.Log($"STAMP applied to: {currentDocument.documentTitle}");

        // Add satisfying feedback
        GameManager.Instance.IncrementBureaucracyScore(5);
    }

    private void ApplySignature()
    {
        if (currentDocument == null) return;

        // Visual effect
        if (signatureEffect != null)
        {
            GameObject effect = Instantiate(signatureEffect, transform);
            Destroy(effect, 2f);
        }

        Debug.Log($"SIGNATURE applied to: {currentDocument.documentTitle}");
        GameManager.Instance.IncrementBureaucracyScore(5);
    }

    private void SaveFormFields()
    {
        if (currentDocument == null) return;

        foreach (TMP_InputField input in formInputs)
        {
            if (input != null && currentDocument.formFields.ContainsKey(input.name))
            {
                currentDocument.formFields[input.name] = input.text;
            }
        }
    }

    private void ClearDisplay()
    {
        currentDocument = null;

        if (documentTitleText != null) documentTitleText.text = "Select a document";
        if (citizenNameText != null) citizenNameText.text = "";
        if (documentTypeText != null) documentTypeText.text = "";
        if (statusText != null) statusText.text = "";
        if (detailsText != null) detailsText.text = "";

        ClearFormFields();
        SetButtonsActive(false);
    }
}