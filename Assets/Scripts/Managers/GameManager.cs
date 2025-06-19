using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int bureaucracyScore = 0;
    public int documentsProcessed = 0;
    public int citizensServed = 0;
    public int citizensAbandoned = 0;

    [Header("Dynamic Configuration")]
    public DynamicConfigurationManager configurationManager;
    public BureaucracyScenario currentScenario;
    
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

        // Initialize configuration manager if not already done
        if (configurationManager == null)
        {
            configurationManager = FindObjectOfType<DynamicConfigurationManager>();
        }

        // Load scenario
        if (currentScenario != null && configurationManager != null)
        {
            configurationManager.LoadScenario(currentScenario);
            Debug.Log($"Loaded scenario: {currentScenario.scenarioName}");
        }
        else
        {
            Debug.LogWarning("No scenario configured! Please assign a BureaucracyScenario.");
        }

        // Subscribe to configuration manager events
        if (configurationManager != null)
        {
            configurationManager.OnDynamicDocumentGenerated += OnDynamicDocumentGenerated;
            configurationManager.OnTriggerActivated += OnTriggerActivated;
            configurationManager.OnNarrativeUpdate += OnNarrativeUpdate;
        }
    }

    private void OnDynamicDocumentGenerated(DocumentData document)
    {
        Debug.Log($"New dynamic document generated: {document.documentTitle}");
    }

    private void OnTriggerActivated(BureaucracyTrigger trigger)
    {
        Debug.Log($"Bureaucracy trigger activated: {trigger.triggerName}");
        IncrementBureaucracyScore(trigger.bureaucracyScoreBonus);
    }

    private void OnNarrativeUpdate(string narrativeText)
    {
        Debug.Log($"Narrative update: {narrativeText}");
        
        // Display narrative through UI systems
        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            gameUI.ShowNotification(narrativeText);
        }
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

    // Method to switch scenarios during runtime
    public void LoadNewScenario(BureaucracyScenario scenario)
    {
        currentScenario = scenario;
        if (configurationManager != null)
        {
            configurationManager.LoadScenario(scenario);
        }
    }

    // Get current scenario status
    public BureaucracyScenarioStatus GetScenarioStatus()
    {
        if (configurationManager != null)
        {
            return configurationManager.GetScenarioStatus();
        }
        
        // Fallback status
        return new BureaucracyScenarioStatus
        {
            scenarioName = "No Scenario",
            documentsProcessed = documentsProcessed,
            currentScore = bureaucracyScore,
            activeCitizensCount = 0,
            availableTemplatesCount = 0
        };
    }
}
