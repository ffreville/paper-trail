using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NaturalLightUI : MonoBehaviour
{
    [Header("Natural Light Display")]
    public Slider naturalLightBar;
    public Image naturalLightIcon;
    public TextMeshProUGUI lightLevelText;
    
    [Header("Window Controls")]
    public Button windowButton;
    public TextMeshProUGUI windowButtonText;
    public Image windowStatusIcon;
    public TextMeshProUGUI windowQuotaText;
    
    [Header("Seasonal Display")]
    public TextMeshProUGUI seasonText;
    public Image seasonIcon;
    public TextMeshProUGUI seasonEffectText;
    
    [Header("Depression Visual Effects")]
    public GameObject depressionOverlay;
    public Image screenDimmer;
    public GameObject sadnessParticles;
    public AudioSource depressiveMusic;
    
    [Header("Window Opening Effects")]
    public GameObject sunlightEffect;
    public ParticleSystem sunbeamParticles;
    public AudioSource birdsChirping;
    public AudioSource windowOpenSound;
    
    [Header("Bureaucracy Status")]
    public TextMeshProUGUI permitStatusText;
    public Image permitIcon;
    public TextMeshProUGUI lightRegulationText;
    
    private NaturalLightNeedExtension lightExtension;
    private PhysiologicalNeedsManager needsManager;
    private bool isShowingDepression = false;
    private Coroutine windowTimerCoroutine;
    
    private void Start()
    {
        lightExtension = FindObjectOfType<NaturalLightNeedExtension>();
        needsManager = FindObjectOfType<PhysiologicalNeedsManager>();
        
        if (lightExtension == null)
        {
            Debug.LogError("NaturalLightNeedExtension not found!");
            return;
        }
        
        SetupUI();
        StartCoroutine(UpdateLightDisplay());
    }
    
    private void SetupUI()
    {
        // Configure natural light bar
        if (naturalLightBar != null)
        {
            naturalLightBar.minValue = 0f;
            naturalLightBar.maxValue = 100f;
            naturalLightBar.value = 100f;
            naturalLightBar.interactable = false;
        }
        
        // Setup window button
        if (windowButton != null)
        {
            windowButton.onClick.AddListener(OnWindowButtonClicked);
        }
        
        // Hide effects initially
        if (depressionOverlay != null) depressionOverlay.SetActive(false);
        if (sunlightEffect != null) sunlightEffect.SetActive(false);
        if (sadnessParticles != null) sadnessParticles.SetActive(false);
    }
    
    private IEnumerator UpdateLightDisplay()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (lightExtension == null) continue;
            
            UpdateNaturalLightBar();
            UpdateWindowControls();
            UpdateSeasonalDisplay();
            UpdateDepressionEffects();
            UpdatePermitStatus();
        }
    }
    
    private void UpdateNaturalLightBar()
    {
        float lightLevel = lightExtension.GetNaturalLightLevel();
        
        if (naturalLightBar != null)
        {
            naturalLightBar.value = lightLevel;
            
            // Update bar color based on level
            Image fillImage = naturalLightBar.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                if (lightLevel <= 10f)
                    fillImage.color = Color.red;
                else if (lightLevel <= 25f)
                    fillImage.color = Color.magenta;
                else if (lightLevel <= 50f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.green;
            }
        }
        
        // Update text display
        if (lightLevelText != null)
        {
            string status = GetLightLevelStatus(lightLevel);
            lightLevelText.text = $"Lumière: {lightLevel:F0}%\n{status}";
            
            // Color text based on severity
            if (lightLevel <= 10f)
                lightLevelText.color = Color.red;
            else if (lightLevel <= 25f)
                lightLevelText.color = Color.magenta;
            else
                lightLevelText.color = Color.white;
        }
        
        // Update icon
        if (naturalLightIcon != null)
        {
            // Flash icon when critical
            if (lightLevel <= 25f)
            {
                float pulse = Mathf.PingPong(Time.time * 2f, 1f);
                naturalLightIcon.color = Color.Lerp(Color.white, Color.red, pulse);
            }
            else
            {
                naturalLightIcon.color = Color.white;
            }
        }
    }
    
    private string GetLightLevelStatus(float level)
    {
        if (level <= 5f) return "DÉPRESSION SÉVÈRE";
        if (level <= 10f) return "État critique";
        if (level <= 25f) return "Manque important";
        if (level <= 50f) return "Carence modérée";
        if (level <= 75f) return "Niveau acceptable";
        return "Bien éclairé";
    }
    
    private void UpdateWindowControls()
    {
        bool isOpen = lightExtension.IsWindowOpen();
        bool canOpen = lightExtension.CanOpenWindow();
        int remainingOpenings = lightExtension.GetRemainingWindowOpenings();
        
        // Update window button
        if (windowButton != null)
        {
            windowButton.interactable = canOpen || isOpen;
            
            // Update button color
            Image buttonImage = windowButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isOpen)
                    buttonImage.color = Color.green;
                else if (canOpen)
                    buttonImage.color = Color.yellow;
                else
                    buttonImage.color = Color.red;
            }
        }
        
        // Update button text
        if (windowButtonText != null)
        {
            if (isOpen)
            {
                windowButtonText.text = "FERMER\nFENÊTRE";
                windowButtonText.color = Color.white;
            }
            else if (canOpen)
            {
                windowButtonText.text = "OUVRIR\nFENÊTRE";
                windowButtonText.color = Color.black;
            }
            else
            {
                windowButtonText.text = "QUOTA\nÉPUISSÉ";
                windowButtonText.color = Color.red;
            }
        }
        
        // Update window status icon
        if (windowStatusIcon != null)
        {
            if (isOpen)
            {
                windowStatusIcon.color = Color.green;
                // Could change sprite to open window
            }
            else
            {
                windowStatusIcon.color = Color.gray;
                // Could change sprite to closed window
            }
        }
        
        // Update quota text
        if (windowQuotaText != null)
        {
            windowQuotaText.text = $"Ouvertures restantes: {remainingOpenings}/2";
            
            if (remainingOpenings == 0)
                windowQuotaText.color = Color.red;
            else if (remainingOpenings == 1)
                windowQuotaText.color = Color.magenta;
            else
                windowQuotaText.color = Color.white;
        }
    }
    
    private void UpdateSeasonalDisplay()
    {
        string currentSeason = lightExtension.GetCurrentSeason();
        
        if (seasonText != null)
        {
            seasonText.text = $"Saison: {currentSeason}";
        }
        
        if (seasonIcon != null)
        {
            // Change color based on season
            switch (currentSeason)
            {
                case "Printemps":
                    seasonIcon.color = Color.green;
                    break;
                case "Été":
                    seasonIcon.color = Color.yellow;
                    break;
                case "Automne":
                    seasonIcon.color = Color.magenta;
                    break;
                case "Hiver":
                    seasonIcon.color = Color.cyan;
                    break;
            }
        }
        
        if (seasonEffectText != null)
        {
            string effect = GetSeasonalEffect(currentSeason);
            seasonEffectText.text = effect;
        }
    }
    
    private string GetSeasonalEffect(string season)
    {
        switch (season)
        {
            case "Printemps":
                return "Réglementation Anti-Distraction active";
            case "Été":
                return "Fenêtres fermées (climatisation)";
            case "Automne":
                return "Feuilles = distraction interdite";
            case "Hiver":
                return "DÉPRESSION SAISONNIÈRE MAJEURE";
            default:
                return "Effet saisonnier inconnu";
        }
    }
    
    private void UpdateDepressionEffects()
    {
        float lightLevel = lightExtension.GetNaturalLightLevel();
        bool shouldShowDepression = lightLevel <= 25f;
        
        if (shouldShowDepression && !isShowingDepression)
        {
            StartDepressionEffects();
        }
        else if (!shouldShowDepression && isShowingDepression)
        {
            StopDepressionEffects();
        }
        
        // Update depression intensity
        if (isShowingDepression)
        {
            UpdateDepressionIntensity(lightLevel);
        }
    }
    
    private void StartDepressionEffects()
    {
        isShowingDepression = true;
        
        if (depressionOverlay != null)
        {
            depressionOverlay.SetActive(true);
        }
        
        if (sadnessParticles != null)
        {
            sadnessParticles.SetActive(true);
        }
        
        if (depressiveMusic != null && !depressiveMusic.isPlaying)
        {
            depressiveMusic.Play();
        }
        
        Debug.Log("Depression visual effects started");
    }
    
    private void StopDepressionEffects()
    {
        isShowingDepression = false;
        
        if (depressionOverlay != null)
        {
            depressionOverlay.SetActive(false);
        }
        
        if (sadnessParticles != null)
        {
            sadnessParticles.SetActive(false);
        }
        
        if (depressiveMusic != null && depressiveMusic.isPlaying)
        {
            depressiveMusic.Stop();
        }
        
        Debug.Log("Depression visual effects stopped");
    }
    
    private void UpdateDepressionIntensity(float lightLevel)
    {
        float intensity = Mathf.InverseLerp(25f, 0f, lightLevel); // 0 à 25% de lumière
        
        // Update screen dimmer
        if (screenDimmer != null)
        {
            Color dimColor = screenDimmer.color;
            dimColor.a = intensity * 0.5f; // Max 50% opacity
            screenDimmer.color = dimColor;
        }
        
        // Update music volume
        if (depressiveMusic != null)
        {
            depressiveMusic.volume = intensity * 0.3f; // Max 30% volume
        }
    }
    
    private void UpdatePermitStatus()
    {
        // Simule le statut du permis
        bool hasPermit = HasValidWindowPermit();
        
        if (permitStatusText != null)
        {
            if (hasPermit)
            {
                permitStatusText.text = "Permis: VALIDE";
                permitStatusText.color = Color.green;
            }
            else
            {
                permitStatusText.text = "Permis: REQUIS";
                permitStatusText.color = Color.red;
            }
        }
        
        if (permitIcon != null)
        {
            permitIcon.color = hasPermit ? Color.green : Color.red;
        }
        
        if (lightRegulationText != null)
        {
            string regulation = GetCurrentLightRegulation();
            lightRegulationText.text = regulation;
        }
    }
    
    private bool HasValidWindowPermit()
    {
        // Simulation simplifiée
        var docManager = FindObjectOfType<DocumentManager>();
        return docManager != null && docManager.processedDocuments.Count > 0;
    }
    
    private string GetCurrentLightRegulation()
    {
        string season = lightExtension.GetCurrentSeason();
        
        switch (season)
        {
            case "Printemps":
                return "Rég. 127-A: Lumière contrôlée";
            case "Été":
                return "Décret 89-B: Fenêtres interdites";
            case "Automne":
                return "Arrêté 45-C: Anti-distraction";
            case "Hiver":
                return "Loi 666-D: Dépression obligatoire";
            default:
                return "Réglementation inconnue";
        }
    }
    
    private void OnWindowButtonClicked()
    {
        if (lightExtension.IsWindowOpen())
        {
            // Force close window (not normally allowed)
            needsManager.OnBureaucraticEvent?.Invoke(
                "Fermeture manuelle de fenêtre détectée ! Rapport d'incident en cours..."
            );
        }
        else
        {
            // Request window opening
            lightExtension.RequestWindowOpening();
            
            if (lightExtension.IsWindowOpen())
            {
                StartWindowOpenEffects();
            }
        }
    }
    
    private void StartWindowOpenEffects()
    {
        // Visual effects for window opening
        if (sunlightEffect != null)
        {
            sunlightEffect.SetActive(true);
        }
        
        if (sunbeamParticles != null)
        {
            sunbeamParticles.Play();
        }
        
        if (birdsChirping != null)
        {
            birdsChirping.Play();
        }
        
        if (windowOpenSound != null)
        {
            windowOpenSound.Play();
        }
        
        // Stop depression effects temporarily
        if (isShowingDepression)
        {
            StopDepressionEffects();
        }
        
        // Start window timer countdown
        if (windowTimerCoroutine != null)
        {
            StopCoroutine(windowTimerCoroutine);
        }
        windowTimerCoroutine = StartCoroutine(WindowTimerCountdown());
    }
    
    private IEnumerator WindowTimerCountdown()
    {
        float timeRemaining = 300f; // 5 minutes
        
        while (timeRemaining > 0 && lightExtension.IsWindowOpen())
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            
            // Update countdown display
            if (windowButtonText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                windowButtonText.text = $"FERME DANS\n{minutes:D2}:{seconds:D2}";
            }
            
            // Warning at 30 seconds
            if (timeRemaining <= 30f && timeRemaining > 29f)
            {
                needsManager.OnBureaucraticEvent?.Invoke(
                    "ATTENTION ! Fermeture automatique de la fenêtre dans 30 secondes !"
                );
            }
        }
        
        // Window closed
        StopWindowOpenEffects();
    }
    
    private void StopWindowOpenEffects()
    {
        if (sunlightEffect != null)
        {
            sunlightEffect.SetActive(false);
        }
        
        if (sunbeamParticles != null)
        {
            sunbeamParticles.Stop();
        }
        
        if (birdsChirping != null)
        {
            birdsChirping.Stop();
        }
    }
    
    // Public method to force UI update
    public void RefreshDisplay()
    {
        StartCoroutine(UpdateLightDisplay());
    }
    
    // Method to simulate emergency lighting protocol
    public void TriggerEmergencyLighting()
    {
        StartCoroutine(EmergencyLightingSequence());
    }
    
    private IEnumerator EmergencyLightingSequence()
    {
        needsManager.OnBureaucraticEvent?.Invoke(
            "PROTOCOLE D'URGENCE LUMINEUX ACTIVÉ ! Clignotement des néons réglementaires !"
        );
        
        // Flash the UI red
        for (int i = 0; i < 10; i++)
        {
            if (naturalLightBar != null)
            {
                Image fillImage = naturalLightBar.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = Color.red;
                }
            }
            
            yield return new WaitForSeconds(0.2f);
            
            if (naturalLightBar != null)
            {
                Image fillImage = naturalLightBar.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = Color.white;
                }
            }
            
            yield return new WaitForSeconds(0.2f);
        }
        
        needsManager.OnBureaucraticEvent?.Invoke(
            "Protocole d'urgence terminé. Retour à l'éclairage déprimant standard."
        );
    }
}
