using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrenchCitizenData
{
    public string firstName;
    public string lastName;
    public string email;
    public string address;
    public string phoneNumber;
    public string socialSecurityNumber;
    public string birthDate;
    public string birthPlace;
    public string profession;
    
    [Header("Request Patterns")]
    public List<string> commonRequests = new List<string>();
    public float desperationLevel = 0.5f;
    public int previousRequestsCount = 0;
}
