using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PhysiologicalNeedsManager : MonoBehaviour
{
    [Header("Need Configuration")]
    public NeedLevel[] needs = new NeedLevel[5];
    
    [Header("Bureaucratic Consequences")]
    public bool enableBureaucraticToilets = true;
    public bool enableLunchBreakDocuments = true;
    public bool enableCoffeeBreakPermits = true;
    
    [Header("Work Hours")]
    public float workDayStartHour = 8f;
    public float workDayEndHour = 17f;
    public float lunchBreakStart = 12f;
    public float lunchBreakDuration = 1f;
    
    [Header("Events")]
    public System.Action<PhysiologicalNeed> OnNeedBecameCritical;
    public System.Action<PhysiologicalNeed> OnNeedBecameEmergency;
    public System.Action<string> OnBureaucraticEvent;
    
    private float gameTimeHours = 8f; // Commence à 8h
    private bool isLunchBreak = false;
    private bool hasEatenToday = false;
    private bool hasUsedToiletToday = false;
    
    private void Start()
    {
        InitializeNeeds();
        StartCoroutine(UpdateNeedsOverTime());
        StartCoroutine(WorkDaySimulation());
    }
    
    private void InitializeNeeds()
    {
        // Configure default needs if not set
        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i] == null)
            {
                needs[i] = new NeedLevel();
            }
            
            needs[i].needType = (PhysiologicalNeed)i;
            
            switch (needs[i].needType)
            {
                case PhysiologicalNeed.Hunger:
                    needs[i].decreaseRate = 0.8f; // Diminue lentement
                    needs[i].criticalThreshold = 30f;
                    break;
                case PhysiologicalNeed.Thirst:
                    needs[i].decreaseRate = 1.2f; // Diminue plus vite
                    needs[i].criticalThreshold = 25f;
                    break;
                case PhysiologicalNeed.Bladder:
                    needs[i].decreaseRate = 1.5f; // Diminue rapidement !
                    needs[i].criticalThreshold = 20f;
                    needs[i].emergencyThreshold = 10f;
                    break;
                case PhysiologicalNeed.Energy:
                    needs[i].decreaseRate = 0.5f; // Fatigue graduelle
                    needs[i].criticalThreshold = 40f;
                    break;
                case PhysiologicalNeed.Stress:
                    needs[i].currentLevel = 0f; // Commence à 0
                    needs[i].decreaseRate = -0.3f; // Diminue avec le temps
                    needs[i].criticalThreshold = 70f;
                    needs[i].emergencyThreshold = 90f;
                    break;
            }
        }
    }
    
    private IEnumerator UpdateNeedsOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            foreach (var need in needs)
            {
                UpdateNeed(need);
            }
            
            CheckProductivityPenalties();
        }
    }
    
    private void UpdateNeed(NeedLevel need)
    {
        float oldLevel = need.currentLevel;
        
        // Modifie selon le contexte
        float modifier = GetContextModifier(need.needType);
        need.currentLevel -= need.decreaseRate * modifier;
        
        // Clamp values
        if (need.needType == PhysiologicalNeed.Stress)
        {
            need.currentLevel = Mathf.Clamp(need.currentLevel, 0f, 100f);
        }
        else
        {
            need.currentLevel = Mathf.Clamp(need.currentLevel, 0f, 100f);
        }
        
        // Check thresholds
        CheckThresholds(need, oldLevel);
    }
    
    private float GetContextModifier(PhysiologicalNeed needType)
    {
        float modifier = 1f;
        
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                if (isLunchBreak) modifier = 2f; // Plus faim pendant la pause déjeuner
                break;
                
            case PhysiologicalNeed.Thirst:
                // Plus soif quand on stresse
                if (GetNeedLevel(PhysiologicalNeed.Stress) > 50f) modifier = 1.5f;
                break;
                
            case PhysiologicalNeed.Bladder:
                // Urgence toilettes augmente avec le stress ET la soif
                if (GetNeedLevel(PhysiologicalNeed.Stress) > 60f) modifier = 1.8f;
                if (GetNeedLevel(PhysiologicalNeed.Thirst) < 30f) modifier += 0.5f;
                break;
                
            case PhysiologicalNeed.Energy:
                // Plus fatigue en fin de journée
                if (gameTimeHours > 15f) modifier = 1.5f;
                break;
                
            case PhysiologicalNeed.Stress:
                // Le stress augmente avec les besoins non satisfaits
                int criticalNeeds = 0;
                foreach (var need in needs)
                {
                    if (need.needType != PhysiologicalNeed.Stress && 
                        need.currentLevel < need.criticalThreshold)
                    {
                        criticalNeeds++;
                    }
                }
                modifier = 1f + (criticalNeeds * 0.5f);
                break;
        }
        
        return modifier;
    }
    
    private void CheckThresholds(NeedLevel need, float oldLevel)
    {
        // Critical threshold
        if (need.currentLevel <= need.criticalThreshold && oldLevel > need.criticalThreshold)
        {
            OnNeedBecameCritical?.Invoke(need.needType);
            TriggerCriticalEvent(need.needType);
        }
        
        // Emergency threshold
        if (need.currentLevel <= need.emergencyThreshold && oldLevel > need.emergencyThreshold)
        {
            OnNeedBecameEmergency?.Invoke(need.needType);
            TriggerEmergencyEvent(need.needType);
        }
    }
    
    private void TriggerCriticalEvent(PhysiologicalNeed needType)
    {
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                OnBureaucraticEvent?.Invoke("Votre estomac gargouille si fort que les citoyens l'entendent !");
                if (enableLunchBreakDocuments)
                {
                    CreateLunchBreakDocument();
                }
                break;
                
            case PhysiologicalNeed.Thirst:
                OnBureaucraticEvent?.Invoke("Votre bouche est si sèche que vous ne pouvez plus lécher les timbres !");
                CreateCoffeeBreakPermit();
                break;
                
            case PhysiologicalNeed.Bladder:
                OnBureaucraticEvent?.Invoke("URGENCE TOILETTES ! Mais d'abord... un formulaire !");
                if (enableBureaucraticToilets)
                {
                    CreateToiletPermitDocument();
                }
                break;
                
            case PhysiologicalNeed.Energy:
                OnBureaucraticEvent?.Invoke("Vous vous endormez sur vos tampons...");
                AddStressFromFatigue();
                break;
                
            case PhysiologicalNeed.Stress:
                OnBureaucraticEvent?.Invoke("Votre œil gauche se met à trembler de manière incontrôlable !");
                TriggerStressBreakdown();
                break;
        }
    }
    
    private void TriggerEmergencyEvent(PhysiologicalNeed needType)
    {
        switch (needType)
        {
            case PhysiologicalNeed.Bladder:
                OnBureaucraticEvent?.Invoke("CATASTROPHE ! Vous ne pouvez plus vous concentrer sur rien d'autre !");
                // Bloque complètement le gameplay jusqu'à ce qu'on aille aux toilettes
                DisableDocumentProcessing();
                break;
                
            case PhysiologicalNeed.Stress:
                OnBureaucraticEvent?.Invoke("BURN-OUT ! Vous jetez tous les formulaires en l'air !");
                TriggerCompleteBreakdown();
                break;
        }
    }
    
    private void CreateToiletPermitDocument()
    {
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            // Crée un document "Autorisation d'aller aux toilettes"
            var toiletTemplate = CreateToiletPermitTemplate();
            var citizen = GetCurrentEmployee(); // L'employé lui-même !
            
            var document = configManager.GenerateDocumentFromTemplate(toiletTemplate, citizen);
            OnBureaucraticEvent?.Invoke("Formulaire T-001 'Autorisation de Pause Physiologique' généré automatiquement !");
        }
    }
    
    private void CreateLunchBreakDocument()
    {
        OnBureaucraticEvent?.Invoke("Vous devez remplir un 'Formulaire de Justification de Pause Nutritionnelle' !");
        
        // Génère automatiquement un document de pause déjeuner
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            var lunchTemplate = CreateLunchPermitTemplate();
            var document = configManager.GenerateDocumentFromTemplate(lunchTemplate, GetCurrentEmployee());
        }
    }
    
    private void CreateCoffeeBreakPermit()
    {
        OnBureaucraticEvent?.Invoke("'Demande d'Autorisation de Consommation de Caféine' requise !");
        
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            var coffeeTemplate = CreateCoffeePermitTemplate();
            var document = configManager.GenerateDocumentFromTemplate(coffeeTemplate, GetCurrentEmployee());
        }
    }
    
    private DocumentTemplate CreateToiletPermitTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Autorisation de Pause Physiologique";
        template.frenchTitle = "Formulaire T-001 - Demande d'Utilisation des Sanitaires";
        template.documentType = DocumentType.CitizenComplaint; // Réutilise un type existant
        template.description = "Autorisation officielle pour quitter temporairement son poste";
        template.baseBureaucracyLevel = 2;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        // Champs absurdes
        template.formFields.Add(new FormField
        {
            fieldName = "Urgence Level",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Modérée", "Critique", "CATASTROPHIQUE" }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Durée Estimée",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "En minutes (max 5)"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Justification Médicale",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Expliquez en détail la nécessité physiologique"
        });
        
        // Trigger pour créer encore plus de bureaucratie !
        BureaucracyTrigger supervisorApproval = new BureaucracyTrigger
        {
            triggerName = "Supervisor Approval Required",
            condition = TriggerCondition.Always,
            probability = 1f,
            newDocumentTypes = new string[] { "HRValidationForm" },
            bureaucracyScoreBonus = 50,
            triggerMessage = "Votre demande de toilettes nécessite l'approbation de votre superviseur !",
            canTriggerRecursively = true
        };
        template.triggers.Add(supervisorApproval);
        
        return template;
    }
    
    private DocumentTemplate CreateLunchPermitTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Formulaire de Justification de Pause Nutritionnelle";
        template.frenchTitle = "Form-ALM-003 - Autorisation de Sustentation";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Demande officielle pour l'ingestion de nutrients";
        template.baseBureaucracyLevel = 1;
        template.requiresStamp = false;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Type de Repas",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Sandwich réglementaire", "Salade administrative", "Soupe bureaucratique" }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Lieu de Consommation",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Salle de pause autorisée", "Bureau (dérogation)", "Cafétéria municipale" }
        });
        
        return template;
    }
    
    private DocumentTemplate CreateCoffeePermitTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Demande d'Autorisation de Consommation de Caféine";
        template.frenchTitle = "CAFF-007 - Permit de Stimulation Légale";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Autorisation de consommer des substances stimulantes pendant les heures de travail";
        template.baseBureaucracyLevel = 3;
        template.requiresStamp = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Type de Caféine",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Café noir réglementaire", "Thé administratif", "Cappuccino (sur dérogation)" }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Niveau de Désespoir",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { "Modéré", "Élevé", "Je vais mourir sans café" }
        });
        
        // Trigger pour créer un certificat médical !
        BureaucracyTrigger medicalCert = new BureaucracyTrigger
        {
            triggerName = "Medical Certificate Required for Caffeine",
            condition = TriggerCondition.Always,
            probability = 0.7f,
            newDocumentTypes = new string[] { "MedicalCertificateRequest" },
            bureaucracyScoreBonus = 75,
            triggerMessage = "Un certificat médical est requis pour justifier votre dépendance à la caféine !",
            canTriggerRecursively = true
        };
        template.triggers.Add(medicalCert);
        
        return template;
    }
    
    private FrenchCitizenData GetCurrentEmployee()
    {
        // Retourne les données de l'employé (le joueur)
        return new FrenchCitizenData
        {
            firstName = "Employé",
            lastName = "Municipal",
            email = "employe.municipal@mairie.fr",
            address = "Bureau 42, Mairie de Kafkaville",
            profession = "Agent Administratif",
            desperationLevel = GetNeedLevel(PhysiologicalNeed.Stress) / 100f
        };
    }
    
    private void CheckProductivityPenalties()
    {
        float totalPenalty = 1f;
        
        foreach (var need in needs)
        {
            if (need.affectsProductivity && need.currentLevel < need.criticalThreshold)
            {
                totalPenalty *= need.productivityPenalty;
            }
        }
        
        // Applique la pénalité aux scores
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            // Modifie temporairement le multiplicateur de score
            // (tu pourrais ajouter cette fonctionnalité au GameManager)
        }
    }
    
    private IEnumerator WorkDaySimulation()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // 1 minute = 1 heure de jeu
            
            gameTimeHours += 1f;
            
            // Reset daily
            if (gameTimeHours >= 24f)
            {
                gameTimeHours = 8f; // Recommence à 8h
                hasEatenToday = false;
                hasUsedToiletToday = false;
            }
            
            // Pause déjeuner
            if (gameTimeHours >= lunchBreakStart && gameTimeHours < lunchBreakStart + lunchBreakDuration)
            {
                isLunchBreak = true;
                if (!hasEatenToday)
                {
                    OnBureaucraticEvent?.Invoke("C'est l'heure du déjeuner ! Mais d'abord... des formulaires !");
                }
            }
            else
            {
                isLunchBreak = false;
            }
            
            // Événements aléatoires
            if (Random.Range(0f, 1f) < 0.1f) // 10% de chance par heure
            {
                TriggerRandomPhysiologicalEvent();
            }
        }
    }
    
    private void TriggerRandomPhysiologicalEvent()
    {
        string[] randomEvents = {
            "Le chauffage est cassé... vous avez encore plus envie d'aller aux toilettes !",
            "Un collègue mange un croissant devant vous... votre faim augmente !",
            "L'odeur du café de la machine cassée vous rend fou !",
            "Un citoyen vous crie dessus... votre stress explose !",
            "Vous découvrez 47 nouveaux formulaires à traiter... fatigue mentale !"
        };
        
        string randomEvent = randomEvents[Random.Range(0, randomEvents.Length)];
        OnBureaucraticEvent?.Invoke(randomEvent);
        
        // Applique un effet aléatoire
        var randomNeed = needs[Random.Range(0, needs.Length)];
        if (randomNeed.needType == PhysiologicalNeed.Stress)
        {
            randomNeed.currentLevel += Random.Range(5f, 15f);
        }
        else
        {
            randomNeed.currentLevel -= Random.Range(5f, 15f);
        }
    }
    
    // Méthodes d'action pour satisfaire les besoins
    public void SatisfyNeed(PhysiologicalNeed needType, float amount)
    {
        var need = GetNeed(needType);
        if (need != null)
        {
            if (needType == PhysiologicalNeed.Stress)
            {
                need.currentLevel = Mathf.Max(0f, need.currentLevel - amount);
            }
            else
            {
                need.currentLevel = Mathf.Min(100f, need.currentLevel + amount);
            }
            
            Debug.Log($"Need {needType} satisfied by {amount}. New level: {need.currentLevel}");
        }
    }
    
    public void UseToilet()
    {
        if (!hasUsedToiletToday)
        {
            SatisfyNeed(PhysiologicalNeed.Bladder, 80f);
            SatisfyNeed(PhysiologicalNeed.Stress, 20f);
            hasUsedToiletToday = true;
            OnBureaucraticEvent?.Invoke("Ahhhh... soulagement ! Retour au travail !");
            EnableDocumentProcessing();
        }
    }
    
    public void EatLunch()
    {
        if (!hasEatenToday && isLunchBreak)
        {
            SatisfyNeed(PhysiologicalNeed.Hunger, 70f);
            SatisfyNeed(PhysiologicalNeed.Energy, 30f);
            hasEatenToday = true;
            OnBureaucraticEvent?.Invoke("Sandwich réglementaire consommé avec succès !");
        }
    }
    
    public void DrinkCoffee()
    {
        SatisfyNeed(PhysiologicalNeed.Thirst, 40f);
        SatisfyNeed(PhysiologicalNeed.Energy, 50f);
        SatisfyNeed(PhysiologicalNeed.Stress, 10f);
        OnBureaucraticEvent?.Invoke("Caféine légalement autorisée intégrée dans l'organisme !");
    }
    
    private void DisableDocumentProcessing()
    {
        // Bloque le traitement des documents
        var documentUI = FindObjectOfType<DocumentUI>();
        if (documentUI != null)
        {
            documentUI.processButton.interactable = false;
            documentUI.rejectButton.interactable = false;
        }
    }
    
    private void EnableDocumentProcessing()
    {
        // Réactive le traitement des documents
        var documentUI = FindObjectOfType<DocumentUI>();
        if (documentUI != null)
        {
            documentUI.processButton.interactable = true;
            documentUI.rejectButton.interactable = true;
        }
    }
    
    private void AddStressFromFatigue()
    {
        SatisfyNeed(PhysiologicalNeed.Stress, -15f); // Ajoute du stress
    }
    
    private void TriggerStressBreakdown()
    {
        OnBureaucraticEvent?.Invoke("Vous criez 'J'EN AI MARRE DE CES FORMULAIRES !' devant tout le monde...");
        // Génère automatiquement 5 nouveaux documents de punition
        StartCoroutine(GeneratePunishmentDocuments());
    }
    
    private void TriggerCompleteBreakdown()
    {
        OnBureaucraticEvent?.Invoke("GAME OVER : Burn-out bureaucratique complet !");
        // Pourrait déclencher un écran de game over ou un mini-jeu de récupération
    }
    
    private IEnumerator GeneratePunishmentDocuments()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(2f);
            OnBureaucraticEvent?.Invoke($"Document disciplinaire #{i+1} généré automatiquement...");
            // Génère des documents de sanction
        }
    }
    
    // Getters utiles
    public float GetNeedLevel(PhysiologicalNeed needType)
    {
        var need = GetNeed(needType);
        return need?.currentLevel ?? 0f;
    }
    
    public NeedLevel GetNeed(PhysiologicalNeed needType)
    {
        foreach (var need in needs)
        {
            if (need.needType == needType)
                return need;
        }
        return null;
    }
    
    public bool IsNeedCritical(PhysiologicalNeed needType)
    {
        var need = GetNeed(needType);
        return need != null && need.currentLevel <= need.criticalThreshold;
    }
    
    public float GetCurrentGameTime()
    {
        return gameTimeHours;
    }
    
    public bool IsWorkingHours()
    {
        return gameTimeHours >= workDayStartHour && gameTimeHours <= workDayEndHour;
    }
}
