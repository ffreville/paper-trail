using UnityEngine;
using System;

[Serializable]
public class CitizenRequest
{
    [Header("Citizen Information")]
    public string citizenId;
    public string citizenName;
    public string citizenEmail;

    [Header("Request Details")]
    public string requestType;
    public string requestDescription;
    public int urgencyLevel; // 1-5, 5 being most urgent
    public DateTime requestDate;

    [Header("Status")]
    public bool isAbandoned;
    public string abandonReason;
    public int documentsGenerated; // Track how many docs this request created

    public CitizenRequest(string name, string request)
    {
        citizenId = System.Guid.NewGuid().ToString();
        citizenName = name;
        requestDescription = request;
        requestDate = DateTime.Now;
        isAbandoned = false;
        documentsGenerated = 1; // Start with 1 (the initial request)
    }
}