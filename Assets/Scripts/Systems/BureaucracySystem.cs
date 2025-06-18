using UnityEngine;
using System.Collections.Generic;

public class BureaucracySystem : MonoBehaviour
{
    [Header("Bureaucracy Rules")]
    public int maxDocumentsBeforeAbandonment = 8;
    public float citizenPatienceTime = 120f; // 2 minutes in real time

    [Header("References")]
    public DocumentManager documentManager;

    private Dictionary<string, CitizenRequest> activeCitizenRequests = new Dictionary<string, CitizenRequest>();

    public void ProcessDocument(DocumentData document)
    {
        Debug.Log($"Processing bureaucracy for: {document.documentTitle}");

        // Apply bureaucracy rules based on document type
        switch (document.documentType)
        {
            case DocumentType.VacationRequest:
                ProcessVacationRequest(document);
                break;
            case DocumentType.MedicalCertificateRequest:
                ProcessMedicalCertificateRequest(document);
                break;
            case DocumentType.ApprovedDoctorsList:
                ProcessApprovedDoctorsList(document);
                break;
            case DocumentType.HRValidationForm:
                ProcessHRValidationForm(document);
                break;
        }
    }

    private void ProcessVacationRequest(DocumentData document)
    {
        // Check if vacation is more than 2 days
        string startDate = document.formFields.ContainsKey("Start Date") ? document.formFields["Start Date"] : "";
        string endDate = document.formFields.ContainsKey("End Date") ? document.formFields["End Date"] : "";

        // For prototype, assume it's always more than 2 days
        Debug.Log("Vacation request is more than 2 days - Medical certificate required!");

        // Generate medical certificate request
        DocumentData medicalCertReq = CreateMedicalCertificateRequest(document);
        documentManager.AddDocument(medicalCertReq);

        // Update original document status
        document.status = DocumentStatus.WaitingForAdditionalInfo;
        document.requiredDocuments.Add("Medical Certificate");

        GameManager.Instance.IncrementBureaucracyScore(25); // Bonus for creating cascade
    }

    private void ProcessMedicalCertificateRequest(DocumentData document)
    {
        Debug.Log("Medical certificate must be from approved doctor!");

        // Generate approved doctors list
        DocumentData doctorsList = CreateApprovedDoctorsList(document);
        documentManager.AddDocument(doctorsList);

        document.status = DocumentStatus.WaitingForAdditionalInfo;
        document.requiredDocuments.Add("Choose from approved doctors list");

        GameManager.Instance.IncrementBureaucracyScore(25);
    }

    private void ProcessApprovedDoctorsList(DocumentData document)
    {
        Debug.Log("Doctor selection requires HR validation!");

        // Generate HR validation form
        DocumentData hrValidation = CreateHRValidationForm(document);
        documentManager.AddDocument(hrValidation);

        document.status = DocumentStatus.WaitingForAdditionalInfo;
        document.requiredDocuments.Add("HR Validation Required");

        GameManager.Instance.IncrementBureaucracyScore(30);
    }

    private void ProcessHRValidationForm(DocumentData document)
    {
        Debug.Log("HR Validation complete - Request can now be finalized!");

        // This is the end of our prototype cascade
        document.status = DocumentStatus.Completed;

        GameManager.Instance.IncrementBureaucracyScore(50); // Big bonus for completion
        GameManager.Instance.citizensServed++;
    }

    private DocumentData CreateMedicalCertificateRequest(DocumentData originalDoc)
    {
        DocumentData medicalReq = new DocumentData();
        medicalReq.documentTitle = "Medical Certificate Request";
        medicalReq.documentType = DocumentType.MedicalCertificateRequest;
        medicalReq.citizenName = originalDoc.citizenName;
        medicalReq.requestDetails = "Medical certificate required for vacation > 2 days";
        medicalReq.bureaucracyLevel = 2;
        medicalReq.requiresStamp = true;

        medicalReq.formFields.Add("Patient Name", originalDoc.citizenName);
        medicalReq.formFields.Add("Reason for Certificate", "Vacation justification");
        medicalReq.formFields.Add("Duration", "");

        return medicalReq;
    }

    private DocumentData CreateApprovedDoctorsList(DocumentData originalDoc)
    {
        DocumentData doctorsList = new DocumentData();
        doctorsList.documentTitle = "Approved Doctors List";
        doctorsList.documentType = DocumentType.ApprovedDoctorsList;
        doctorsList.citizenName = originalDoc.citizenName;
        doctorsList.requestDetails = "List of company-approved medical practitioners";
        doctorsList.bureaucracyLevel = 3;
        doctorsList.requiresStamp = false;

        doctorsList.formFields.Add("Selected Doctor", "");
        doctorsList.formFields.Add("Doctor License Number", "");
        doctorsList.formFields.Add("Appointment Date", "");

        return doctorsList;
    }

    private DocumentData CreateHRValidationForm(DocumentData originalDoc)
    {
        DocumentData hrValidation = new DocumentData();
        hrValidation.documentTitle = "HR Validation Form";
        hrValidation.documentType = DocumentType.HRValidationForm;
        hrValidation.citizenName = originalDoc.citizenName;
        hrValidation.requestDetails = "HR validation for approved doctor selection";
        hrValidation.bureaucracyLevel = 4;
        hrValidation.requiresStamp = true;
        hrValidation.requiresSignature = true;

        hrValidation.formFields.Add("HR Officer", "");
        hrValidation.formFields.Add("Validation Date", "");
        hrValidation.formFields.Add("Comments", "");

        return hrValidation;
    }
}