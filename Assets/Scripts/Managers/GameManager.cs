using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int bureaucracyScore = 0;
    public int documentsProcessed = 0;
    public int citizensServed = 0;
    public int citizensAbandoned = 0;

    [Header("References")]
    public DocumentManager documentManager;
    public BureaucracySystem bureaucracySystem;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Paper Trail - Bureaucracy Simulator Started!");

        // Create initial vacation request for prototype
        CreateInitialVacationRequest();
    }

    private void CreateInitialVacationRequest()
    {
        CitizenRequest citizenRequest = new CitizenRequest("Jean Dupont", "3 days vacation");
        DocumentData vacationDoc = new DocumentData();

        vacationDoc.documentTitle = "Vacation Request - 3 Days";
        vacationDoc.documentType = DocumentType.VacationRequest;
        vacationDoc.citizenName = citizenRequest.citizenName;
        vacationDoc.requestDetails = "Requesting 3 days of vacation";
        vacationDoc.bureaucracyLevel = 1;
        vacationDoc.requiresStamp = true;

        // Add form fields
        vacationDoc.formFields.Add("Start Date", "");
        vacationDoc.formFields.Add("End Date", "");
        vacationDoc.formFields.Add("Reason", "");
        vacationDoc.formFields.Add("Emergency Contact", "");

        documentManager.AddDocument(vacationDoc);
    }

    public void IncrementBureaucracyScore(int points)
    {
        bureaucracyScore += points;
        Debug.Log($"Bureaucracy Score: {bureaucracyScore}");
    }

    public void DocumentProcessed()
    {
        documentsProcessed++;
        IncrementBureaucracyScore(10); // Each processed document gives points
    }
}