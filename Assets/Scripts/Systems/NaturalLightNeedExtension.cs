using UnityEngine;
using System.Collections;

// Extension du système de besoins physiologiques avec la lumière naturelle
public class NaturalLightNeedExtension : MonoBehaviour
{
    [Header("Natural Light Configuration")]
    public bool enableNaturalLightNeed = true;
    public float lightDeprivationRate = 0.7f;
    public float seasonalAffectiveBonus = 1.5f; // Hiver = pire
    public int currentSeason = 0; // 0=Printemps, 1=Été, 2=Automne, 3=Hiver
    
    [Header("Office Environment")]
    public bool windowsAreBlocked = true;
    public bool fluorescentLightsOnly = true;
    public float artificialLightStressMultiplier = 1.3f;
    
    [Header("Bureaucratic Window Controls")]
    public bool windowPermitsRequired = true;
    public float windowOpeningTimeLimit = 300f; // 5 minutes max
    public int maxWindowOpeningsPerDay = 2;
    
    private PhysiologicalNeedsManager needsManager;
    private int windowOpeningsToday = 0;
    private bool isWindowCurrentlyOpen = false;
    private float windowOpenStartTime;
    
    // Nouveau besoin ajouté dynamiquement
    private NeedLevel naturalLightNeed;
    
    private void Start()
    {
        needsManager = FindObjectOfType<PhysiologicalNeedsManager>();
        if (needsManager == null)
        {
            Debug.LogError("PhysiologicalNeedsManager required for Natural Light extension!");
            return;
        }
        
        if (enableNaturalLightNeed)
        {
            AddNaturalLightNeed();
            StartCoroutine(SimulateSeasonalCycle());
            StartCoroutine(MonitorLightDeprivation());
        }
    }
    
    private void AddNaturalLightNeed()
    {
        // Ajoute dynamiquement le besoin de lumière naturelle
        naturalLightNeed = new NeedLevel
        {
            needType = (PhysiologicalNeed)5, // Nouveau type
            currentLevel = 100f,
            decreaseRate = lightDeprivationRate,
            criticalThreshold = 25f,
            emergencyThreshold = 10f,
            affectsProductivity = true,
            productivityPenalty = 0.3f,
            normalColor = Color.yellow,
            criticalColor = Color.magenta,
            emergencyColor = Color.red
        };
        
        // Ajoute aux besoins existants (hack temporaire)
        Debug.Log("Natural Light Need added to system!");
        
        // Subscribe aux événements
        needsManager.OnBureaucraticEvent += OnBureaucraticEvent;
    }
    
    private IEnumerator SimulateSeasonalCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(120f); // Change de saison toutes les 2 minutes
            
            currentSeason = (currentSeason + 1) % 4;
            string[] seasons = { "Printemps", "Été", "Automne", "Hiver" };
            
            UpdateSeasonalEffects();
            
            needsManager.OnBureaucraticEvent?.Invoke(
                $"Changement de saison : {seasons[currentSeason]} ! " +
                "Mise à jour des réglementations lumineuses en cours..."
            );
        }
    }
    
    private void UpdateSeasonalEffects()
    {
        switch (currentSeason)
        {
            case 0: // Printemps
                lightDeprivationRate = 0.7f;
                needsManager.OnBureaucraticEvent?.Invoke(
                    "Printemps détecté ! Réglementation 'Anti-Distraction Printanière' activée !"
                );
                break;
                
            case 1: // Été
                lightDeprivationRate = 1.2f; // Paradoxalement pire en été
                needsManager.OnBureaucraticEvent?.Invoke(
                    "Été détecté ! Fenêtres fermées obligatoires pour 'préserver la climatisation' !"
                );
                break;
                
            case 2: // Automne
                lightDeprivationRate = 0.9f;
                needsManager.OnBureaucraticEvent?.Invoke(
                    "Automne ! Nouvelle directive : 'Les feuilles qui tombent sont une distraction' !"
                );
                break;
                
            case 3: // Hiver
                lightDeprivationRate = 1.8f; // TERRIBLE en hiver
                needsManager.OnBureaucraticEvent?.Invoke(
                    "Hiver ! Dépression saisonnière détectée ! Formulaires de luminothérapie disponibles !"
                );
                CreateSeasonalAffectiveDisorderDocument();
                break;
        }
        
        if (naturalLightNeed != null)
        {
            naturalLightNeed.decreaseRate = lightDeprivationRate;
        }
    }
    
    private IEnumerator MonitorLightDeprivation()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            if (naturalLightNeed == null) continue;
            
            // Diminue le besoin de lumière
            float modifier = GetLightDeprivationModifier();
            naturalLightNeed.currentLevel -= naturalLightNeed.decreaseRate * modifier;
            naturalLightNeed.currentLevel = Mathf.Clamp(naturalLightNeed.currentLevel, 0f, 100f);
            
            // Vérifie les seuils
            CheckLightDeprivationEffects();
            
            // Fermeture automatique des fenêtres après le temps limite
            if (isWindowCurrentlyOpen && Time.time - windowOpenStartTime > windowOpeningTimeLimit)
            {
                ForceCloseWindow();
            }
        }
    }
    
    private float GetLightDeprivationModifier()
    {
        float modifier = 1f;
        
        // Pire avec les néons
        if (fluorescentLightsOnly)
        {
            modifier *= artificialLightStressMultiplier;
        }
        
        // Saisonnier
        if (currentSeason == 3) // Hiver
        {
            modifier *= seasonalAffectiveBonus;
        }
        
        // Stress amplifie le besoin de lumière
        float stressLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Stress);
        if (stressLevel > 50f)
        {
            modifier *= 1.4f;
        }
        
        // Si fenêtre ouverte, récupération
        if (isWindowCurrentlyOpen)
        {
            modifier = -2f; // Récupération rapide
        }
        
        return modifier;
    }
    
    private void CheckLightDeprivationEffects()
    {
        if (naturalLightNeed.currentLevel <= naturalLightNeed.emergencyThreshold)
        {
            TriggerSevereDepression();
        }
        else if (naturalLightNeed.currentLevel <= naturalLightNeed.criticalThreshold)
        {
            TriggerMildDepression();
        }
    }
    
    private void TriggerMildDepression()
    {
        // Effets de la dépression légère
        needsManager.OnBureaucraticEvent?.Invoke(
            "Manque de lumière naturelle détecté ! Votre moral chute dangereusement..."
        );
        
        // Augmente le stress
        var stressNeed = needsManager.GetNeed(PhysiologicalNeed.Stress);
        if (stressNeed != null)
        {
            stressNeed.currentLevel += 5f;
        }
        
        // Réduit l'énergie
        var energyNeed = needsManager.GetNeed(PhysiologicalNeed.Energy);
        if (energyNeed != null)
        {
            energyNeed.currentLevel -= 3f;
        }
        
        // Génère des documents bureaucratiques
        if (Random.Range(0f, 1f) < 0.3f)
        {
            CreateWindowPermitDocument();
        }
    }
    
    private void TriggerSevereDepression()
    {
        needsManager.OnBureaucraticEvent?.Invoke(
            "DÉPRESSION SAISONNIÈRE SÉVÈRE ! Vous fixez le mur en pleurant doucement..."
        );
        
        // Effets dramatiques
        var stressNeed = needsManager.GetNeed(PhysiologicalNeed.Stress);
        if (stressNeed != null)
        {
            stressNeed.currentLevel += 15f;
        }
        
        // Productivité CATASTROPHIQUE
        DisableProductivityTemporarily();
        
        // Génère OBLIGATOIREMENT des documents
        CreateLuminotherapyRequest();
        CreatePsychologicalEvaluationForm();
    }
    
    private void CreateSeasonalAffectiveDisorderDocument()
    {
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            var sadTemplate = CreateSADTemplate();
            var employee = GetCurrentEmployee();
            
            configManager.GenerateDocumentFromTemplate(sadTemplate, employee);
            needsManager.OnBureaucraticEvent?.Invoke(
                "Formulaire SAD-404 'Trouble Affectif Saisonnier' généré automatiquement !"
            );
        }
    }
    
    private void CreateWindowPermitDocument()
    {
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager != null)
        {
            var windowTemplate = CreateWindowPermitTemplate();
            var employee = GetCurrentEmployee();
            
            configManager.GenerateDocumentFromTemplate(windowTemplate, employee);
            needsManager.OnBureaucraticEvent?.Invoke(
                "Demande de 'Permis d'Ouverture de Store' (FORM-WIN-001) requise !"
            );
        }
    }
    
    private void CreateLuminotherapyRequest()
    {
        needsManager.OnBureaucraticEvent?.Invoke(
            "Formulaire LUM-505 'Demande de Luminothérapie d'Urgence' en cours de génération..."
        );
        
        StartCoroutine(GenerateLuminotherapyDocuments());
    }
    
    private IEnumerator GenerateLuminotherapyDocuments()
    {
        var configManager = FindObjectOfType<DynamicConfigurationManager>();
        if (configManager == null) yield break;
        
        // Génère 3 documents en cascade
        yield return new WaitForSeconds(1f);
        
        var lumTemplate = CreateLuminotherapyTemplate();
        configManager.GenerateDocumentFromTemplate(lumTemplate, GetCurrentEmployee());
        needsManager.OnBureaucraticEvent?.Invoke("1/3 - Demande de luminothérapie générée...");
        
        yield return new WaitForSeconds(2f);
        
        var medTemplate = CreateMedicalLightEvaluationTemplate();
        configManager.GenerateDocumentFromTemplate(medTemplate, GetCurrentEmployee());
        needsManager.OnBureaucraticEvent?.Invoke("2/3 - Évaluation médicale lumineuse requise...");
        
        yield return new WaitForSeconds(2f);
        
        var equipTemplate = CreateLightEquipmentRequestTemplate();
        configManager.GenerateDocumentFromTemplate(equipTemplate, GetCurrentEmployee());
        needsManager.OnBureaucraticEvent?.Invoke("3/3 - Demande d'équipement lumineux finalisée !");
    }
    
    private void CreatePsychologicalEvaluationForm()
    {
        needsManager.OnBureaucraticEvent?.Invoke(
            "Votre état psychologique nécessite une évaluation ! Formulaire PSY-999 en préparation..."
        );
    }
    
    // Action publique pour ouvrir la fenêtre
    public void RequestWindowOpening()
    {
        if (!windowPermitsRequired)
        {
            OpenWindow();
            return;
        }
        
        if (windowOpeningsToday >= maxWindowOpeningsPerDay)
        {
            needsManager.OnBureaucraticEvent?.Invoke(
                "QUOTA DÉPASSÉ ! Maximum " + maxWindowOpeningsPerDay + " ouvertures de fenêtre par jour !"
            );
            
            // Génère un formulaire de dérogation
            CreateWindowQuotaExceptionForm();
            return;
        }
        
        // Vérifie si un permis valide existe
        if (HasValidWindowPermit())
        {
            OpenWindow();
        }
        else
        {
            needsManager.OnBureaucraticEvent?.Invoke(
                "Permis d'ouverture de fenêtre requis ! Veuillez remplir le formulaire FORM-WIN-001 !"
            );
            CreateWindowPermitDocument();
        }
    }
    
    private void OpenWindow()
    {
        if (isWindowCurrentlyOpen)
        {
            needsManager.OnBureaucraticEvent?.Invoke("La fenêtre est déjà ouverte !");
            return;
        }
        
        isWindowCurrentlyOpen = true;
        windowOpenStartTime = Time.time;
        windowOpeningsToday++;
        
        needsManager.OnBureaucraticEvent?.Invoke(
            $"Fenêtre ouverte ! Lumière naturelle autorisée pour {windowOpeningTimeLimit} secondes !"
        );
        
        // Commence la récupération de lumière naturelle
        StartCoroutine(WindowOpenRecovery());
    }
    
    private IEnumerator WindowOpenRecovery()
    {
        while (isWindowCurrentlyOpen && naturalLightNeed != null)
        {
            yield return new WaitForSeconds(1f);
            
            // Récupération rapide
            naturalLightNeed.currentLevel += 3f;
            naturalLightNeed.currentLevel = Mathf.Min(100f, naturalLightNeed.currentLevel);
            
            // Bonus de bonheur
            var stressNeed = needsManager.GetNeed(PhysiologicalNeed.Stress);
            if (stressNeed != null)
            {
                stressNeed.currentLevel = Mathf.Max(0f, stressNeed.currentLevel - 1f);
            }
        }
    }
    
    private void ForceCloseWindow()
    {
        isWindowCurrentlyOpen = false;
        
        needsManager.OnBureaucraticEvent?.Invoke(
            "TEMPS ÉCOULÉ ! Fermeture automatique de la fenêtre pour 'optimisation énergétique' !"
        );
        
        // Pénalité de stress pour la fermeture forcée
        var stressNeed = needsManager.GetNeed(PhysiologicalNeed.Stress);
        if (stressNeed != null)
        {
            stressNeed.currentLevel += 10f;
        }
        
        // Chance de générer un rapport d'incident
        if (Random.Range(0f, 1f) < 0.4f)
        {
            needsManager.OnBureaucraticEvent?.Invoke(
                "Rapport d'incident généré : 'Utilisation excessive de ressource lumineuse naturelle' !"
            );
        }
    }
    
    private bool HasValidWindowPermit()
    {
        // Simulation : vérifie s'il y a un document de permis traité aujourd'hui
        var docManager = FindObjectOfType<DocumentManager>();
        if (docManager == null) return false;
        
        // Logique simplifiée : assume qu'on a un permis si on a traité un document aujourd'hui
        return docManager.processedDocuments.Count > 0;
    }
    
    private void CreateWindowQuotaExceptionForm()
    {
        needsManager.OnBureaucraticEvent?.Invoke(
            "Génération du formulaire DERO-WIN-002 'Demande de Dérogation de Quota Lumineux' !"
        );
    }
    
    private void DisableProductivityTemporarily()
    {
        // Bloque temporairement la productivité
        var documentUI = FindObjectOfType<DocumentUI>();
        if (documentUI != null)
        {
            documentUI.processButton.interactable = false;
            documentUI.rejectButton.interactable = false;
            
            // Réactive après 30 secondes
            StartCoroutine(ReenableProductivityAfterDelay());
        }
    }
    
    private IEnumerator ReenableProductivityAfterDelay()
    {
        yield return new WaitForSeconds(30f);
        
        var documentUI = FindObjectOfType<DocumentUI>();
        if (documentUI != null)
        {
            documentUI.processButton.interactable = true;
            documentUI.rejectButton.interactable = true;
        }
        
        needsManager.OnBureaucraticEvent?.Invoke(
            "Productivité restaurée ! Vous pouvez maintenant retraiter des documents..."
        );
    }
    
    // Templates de documents pour la lumière naturelle
    private DocumentTemplate CreateWindowPermitTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Window Opening Permit FORM-WIN-001";
        template.frenchTitle = "Permis FORM-WIN-001 - Autorisation d'Ouverture de Store";
        template.documentType = DocumentType.CitizenComplaint;
        template.description = "Official authorization for natural light access via window opening";
        template.frenchDescription = "Autorisation officielle d'accès à la lumière naturelle via ouverture de fenêtre";
        template.baseBureaucracyLevel = 4;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Light Deprivation Level",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Légèrement photophobe", 
                "Modérément déprimé", 
                "Sévèrement carencé en vitamine D", 
                "Vampirisation en cours"
            }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Window Opening Duration",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "Durée en secondes (max 300)",
            maxLength = 3
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Justification for Natural Light",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Expliquez pourquoi la lumière artificielle réglementaire ne suffit pas à votre organisme défaillant"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Productivity Impact Assessment",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Démontrez comment 5 minutes de lumière naturelle amélioreront votre rendement bureaucratique"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Weather Conditions Declaration",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Ensoleillé (autorisation probable)", 
                "Nuageux (autorisation conditionnelle)", 
                "Pluvieux (autorisation improbable)", 
                "Orageux (INTERDIT - risque électrique)"
            }
        });
        
        // Trigger pour contrôle météorologique
        BureaucracyTrigger weatherCheck = new BureaucracyTrigger
        {
            triggerName = "Weather Verification Required",
            condition = TriggerCondition.Always,
            probability = 0.7f,
            newDocumentTypes = new string[] { "MedicalCertificateRequest" },
            bureaucracyScoreBonus = 100,
            triggerMessage = "Vérification météorologique obligatoire ! Contact du service Météo-France requis !",
            canTriggerRecursively = true
        };
        template.triggers.Add(weatherCheck);
        
        return template;
    }
    
    private DocumentTemplate CreateSADTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Seasonal Affective Disorder Form SAD-404";
        template.frenchTitle = "Formulaire SAD-404 - Trouble Affectif Saisonnier";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Mandatory seasonal depression evaluation and treatment authorization";
        template.frenchDescription = "Évaluation obligatoire de dépression saisonnière et autorisation de traitement";
        template.baseBureaucracyLevel = 5;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Seasonal Depression Symptoms",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Légère mélancolie hivernale", 
                "Déprime modérée automnale", 
                "Tristesse profonde printanière", 
                "Désespoir existentiel estival"
            }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Vitamin D Deficiency Level",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Carence légère", 
                "Carence modérée", 
                "Carence sévère", 
                "Rachitisme bureaucratique"
            }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Light Therapy Request",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Décrivez précisément votre besoin thérapeutique en lumière artificielle compensatoire"
        });
        
        return template;
    }
    
    private DocumentTemplate CreateLuminotherapyTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Light Therapy Request LUM-505";
        template.frenchTitle = "Demande LUM-505 - Luminothérapie d'Urgence";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Emergency light therapy equipment requisition";
        template.frenchDescription = "Réquisition d'équipement de luminothérapie d'urgence";
        template.baseBureaucracyLevel = 6;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Light Intensity Required",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "2,500 lux (minimum légal)", 
                "5,000 lux (thérapeutique)", 
                "10,000 lux (urgence médicale)", 
                "15,000 lux (DANGEREUX - autorisation spéciale)"
            }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Treatment Duration",
            fieldType = FormFieldType.Number,
            isRequired = true,
            placeholder = "Durée quotidienne en minutes"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Medical Supervision Required",
            fieldType = FormFieldType.Checkbox,
            isRequired = false
        });
        
        return template;
    }
    
    private DocumentTemplate CreateMedicalLightEvaluationTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Medical Light Evaluation MED-LUM-808";
        template.frenchTitle = "Évaluation MED-LUM-808 - Examen Photothérapeutique";
        template.documentType = DocumentType.MedicalCertificateRequest;
        template.description = "Comprehensive medical evaluation for light therapy eligibility";
        template.frenchDescription = "Évaluation médicale approfondie pour éligibilité à la photothérapie";
        template.baseBureaucracyLevel = 7;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Retinal Examination Results",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Résultats de l'examen rétinien complet (joindre certificat ophtalmologique)"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Skin Photosensitivity Test",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Peau résistante (autorisation complète)", 
                "Peau sensible (limitations requises)", 
                "Peau très sensible (contre-indication)", 
                "Albinisme (INTERDIT ABSOLU)"
            }
        });
        
        return template;
    }
    
    private DocumentTemplate CreateLightEquipmentRequestTemplate()
    {
        DocumentTemplate template = ScriptableObject.CreateInstance<DocumentTemplate>();
        template.documentTitle = "Light Equipment Request EQUIP-LUM-999";
        template.frenchTitle = "Demande EQUIP-LUM-999 - Matériel Luminothérapeutique";
        template.documentType = DocumentType.VacationRequest;
        template.description = "Official requisition for therapeutic lighting equipment";
        template.frenchDescription = "Réquisition officielle d'équipement d'éclairage thérapeutique";
        template.baseBureaucracyLevel = 8;
        template.requiresStamp = true;
        template.requiresSignature = true;
        
        template.formFields.Add(new FormField
        {
            fieldName = "Equipment Type",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Lampe de bureau standard (500 lux)", 
                "Panneau lumineux thérapeutique (5,000 lux)", 
                "Simulateur d'aube progressive", 
                "Casque de luminothérapie intégrale",
                "Combinaison photo-stimulante (EXPÉRIMENTAL)"
            }
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Budget Allocation Justification",
            fieldType = FormFieldType.TextArea,
            isRequired = true,
            placeholder = "Justifiez l'impact budgétaire de cet équipement sur la productivité municipale"
        });
        
        template.formFields.Add(new FormField
        {
            fieldName = "Installation Location",
            fieldType = FormFieldType.Dropdown,
            isRequired = true,
            dropdownOptions = new string[] { 
                "Bureau personnel (dérogation requise)", 
                "Salle commune (autorisation collective)", 
                "Cave administrative (ironique)", 
                "Toit de la mairie (installation spéciale)"
            }
        });
        
        return template;
    }
    
    private FrenchCitizenData GetCurrentEmployee()
    {
        return new FrenchCitizenData
        {
            firstName = "Employé",
            lastName = "Déprimé",
            email = "employe.deprime@mairie-sombre.fr",
            address = "Bureau sans fenêtre, Sous-sol administratif",
            profession = "Agent Administratif Photophobe",
            desperationLevel = naturalLightNeed != null ? (100f - naturalLightNeed.currentLevel) / 100f : 0.8f
        };
    }
    
    private void OnBureaucraticEvent(string eventMessage)
    {
        // Log pour debug
        Debug.Log($"Natural Light Event: {eventMessage}");
    }
    
    // Méthodes publiques pour l'UI
    public float GetNaturalLightLevel()
    {
        return naturalLightNeed?.currentLevel ?? 100f;
    }
    
    public bool IsWindowOpen()
    {
        return isWindowCurrentlyOpen;
    }
    
    public int GetRemainingWindowOpenings()
    {
        return Mathf.Max(0, maxWindowOpeningsPerDay - windowOpeningsToday);
    }
    
    public string GetCurrentSeason()
    {
        string[] seasons = { "Printemps", "Été", "Automne", "Hiver" };
        return seasons[currentSeason];
    }
    
    public bool CanOpenWindow()
    {
        return windowOpeningsToday < maxWindowOpeningsPerDay && !isWindowCurrentlyOpen;
    }
}
