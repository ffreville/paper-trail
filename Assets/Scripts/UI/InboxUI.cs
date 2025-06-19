using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InboxUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform documentListParent;
    public GameObject documentItemPrefab;
    public TextMeshProUGUI inboxCountText;
    public Button refreshButton;

    [Header("Dynamic Mode UI")]
    public GameObject dynamicModePanel;
    public Button generateRandomDocButton;
    public TMP_Dropdown scenarioDropdown;
    public Button loadScenarioButton;
    public TextMeshProUGUI scenarioStatusText;

    [Header("Document Selection")]
    public DocumentUI documentUI;

    private DocumentManager documentManager;
    private DynamicConfigurationManager configurationManager;
    private List<GameObject> documentUIItems = new List<GameObject>();
    private List<BureaucracyScenario> availableScenarios = new List<BureaucracyScenario>();

    private void Start()
    {
        // Find managers
        documentManager = FindObjectOfType<DocumentManager>();
        configurationManager = FindObjectOfType<DynamicConfigurationManager>();

        // Subscribe to events
        if (documentManager != null)
        {
            documentManager.OnDocumentAdded += OnDocumentAdded;
            documentManager.OnDocumentProcessed += OnDocumentProcessed;
        }

        // Setup UI
        SetupDynamicUI();
        
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshInbox);
        }

        RefreshInbox();
    }

    private void SetupDynamicUI()
    {
        if (dynamicModePanel != null)
        {
            dynamicModePanel.SetActive(true);
        }

        if (configurationManager != null)
        {
            // Setup dynamic mode controls
            if (generateRandomDocButton != null)
            {
                generateRandomDocButton.onClick.AddListener(GenerateRandomDocument);
            }

            if (loadScenarioButton != null)
            {
                loadScenarioButton.onClick.AddListener(LoadSelectedScenario);
            }

            // Load available scenarios
            LoadAvailableScenarios();
        }
    }

    private void LoadAvailableScenarios()
    {
        if (scenarioDropdown == null) return;

        // Find all BureaucracyScenario assets
        availableScenarios.Clear();
        scenarioDropdown.ClearOptions();

#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BureaucracyScenario");
        List<string> scenarioNames = new List<string>();

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            BureaucracyScenario scenario = UnityEditor.AssetDatabase.LoadAssetAtPath<BureaucracyScenario>(path);
            
            if (scenario != null)
            {
                availableScenarios.Add(scenario);
                scenarioNames.Add(scenario.scenarioName);
            }
        }

        scenarioDropdown.AddOptions(scenarioNames);
        
        if (availableScenarios.Count > 0)
        {
            UpdateScenarioStatus();
        }
#endif
    }

    private void UpdateScenarioStatus()
    {
        if (scenarioStatusText == null || configurationManager == null) return;

        var status = configurationManager.GetScenarioStatus();
        
        scenarioStatusText.text = $"Scénario: {status.scenarioName}\n" +
                                 $"Documents: {status.documentsProcessed}/{status.targetDocuments}\n" +
                                 $"Score: {status.currentScore}/{status.targetScore}\n" +
                                 $"Citoyens: {status.activeCitizensCount}\n" +
                                 $"Désespoir moyen: {status.averageCitizenDesperation:P0}";
    }

    private void GenerateRandomDocument()
    {
        if (configurationManager != null)
        {
            configurationManager.GenerateRandomDocument();
            Debug.Log("Generated random document via UI");
        }
    }

    private void LoadSelectedScenario()
    {
        if (scenarioDropdown == null || availableScenarios.Count == 0) return;

        int selectedIndex = scenarioDropdown.value;
        if (selectedIndex >= 0 && selectedIndex < availableScenarios.Count)
        {
            BureaucracyScenario selectedScenario = availableScenarios[selectedIndex];
            
            if (configurationManager != null)
            {
                configurationManager.LoadScenario(selectedScenario);
                Debug.Log($"Loaded scenario: {selectedScenario.scenarioName}");
            }

            // Also update the game manager
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.LoadNewScenario(selectedScenario);
            }

            UpdateScenarioStatus();
        }
    }

    private void Update()
    {
        // Update scenario status periodically
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            UpdateScenarioStatus();
        }
    }

    private void OnDestroy()
    {
        if (documentManager != null)
        {
            documentManager.OnDocumentAdded -= OnDocumentAdded;
            documentManager.OnDocumentProcessed -= OnDocumentProcessed;
        }
    }

    private void OnDocumentAdded(DocumentData document)
    {
        Debug.Log($"UI: New document added to inbox: {document.documentTitle}");
        RefreshInbox();
    }

    private void OnDocumentProcessed(DocumentData document)
    {
        Debug.Log($"UI: Document processed: {document.documentTitle}");
        RefreshInbox();
    }

    public void RefreshInbox()
    {
        if (documentManager == null) return;

        // Clear existing UI items
        ClearDocumentItems();

        // Create UI item for each document in inbox
        foreach (DocumentData doc in documentManager.inboxDocuments)
        {
            CreateDocumentUIItem(doc);
        }

        // Update inbox count
        UpdateInboxCount();
    }

    private void ClearDocumentItems()
    {
        foreach (GameObject item in documentUIItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        documentUIItems.Clear();
    }

    private void CreateDocumentUIItem(DocumentData document)
    {
        if (documentItemPrefab == null || documentListParent == null) return;

        GameObject item = Instantiate(documentItemPrefab, documentListParent);
        documentUIItems.Add(item);

        // Configure the document item
        DocumentItemUI itemUI = item.GetComponent<DocumentItemUI>();
        if (itemUI != null)
        {
            itemUI.Setup(document, this);
        }
    }

    private void UpdateInboxCount()
    {
        if (inboxCountText != null && documentManager != null)
        {
            inboxCountText.text = $"Inbox ({documentManager.GetInboxCount()})";
        }
    }

    public void SelectDocument(DocumentData document)
    {
        if (documentUI != null)
        {
            documentUI.DisplayDocument(document);
        }
    }
}
