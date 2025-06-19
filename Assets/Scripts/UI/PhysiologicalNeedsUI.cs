using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PhysiologicalNeedsUI : MonoBehaviour
{
    [Header("Need Bars")]
    public Slider hungerBar;
    public Slider thirstBar;
    public Slider bladderBar;
    public Slider energyBar;
    public Slider stressBar;
    
    [Header("Need Icons")]
    public Image hungerIcon;
    public Image thirstIcon;
    public Image bladderIcon;
    public Image energyIcon;
    public Image stressIcon;
    
    [Header("Action Buttons")]
    public Button toiletButton;
    public Button lunchButton;
    public Button coffeeButton;
    public Button restButton;
    
    [Header("Emergency Notifications")]
    public GameObject emergencyPanel;
    public TextMeshProUGUI emergencyText;
    public Button emergencyCloseButton;
    public AudioSource urgentBeep;
    
    [Header("Time Display")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI workStatusText;
    
    [Header("Visual Effects")]
    public GameObject bladderShakeEffect;
    public GameObject hungerGrowlEffect;
    public GameObject stressFlashEffect;
    public ParticleSystem coffeeParticles;
    
    [Header("Audio")]
    public AudioSource stomachGrowl;
    public AudioSource toiletUrgency;
    public AudioSource stressSound;
    public AudioSource coffeeSlurp;
    
    private PhysiologicalNeedsManager needsManager;
    private bool isShowingEmergency = false;
    private Coroutine shakeCoroutine;
    
    private void Start()
    {
        needsManager = FindObjectOfType<PhysiologicalNeedsManager>();
        
        if (needsManager == null)
        {
            Debug.LogError("PhysiologicalNeedsManager not found!");
            return;
        }
        
        SetupUI();
        SubscribeToEvents();
        
        if (emergencyPanel != null)
        {
            emergencyPanel.SetActive(false);
        }
    }
    
    private void SetupUI()
    {
        // Setup action buttons
        if (toiletButton != null)
        {
            toiletButton.onClick.AddListener(OnToiletButtonClicked);
        }
        
        if (lunchButton != null)
        {
            lunchButton.onClick.AddListener(OnLunchButtonClicked);
        }
        
        if (coffeeButton != null)
        {
            coffeeButton.onClick.AddListener(OnCoffeeButtonClicked);
        }
        
        if (restButton != null)
        {
            restButton.onClick.AddListener(OnRestButtonClicked);
        }
        
        if (emergencyCloseButton != null)
        {
            emergencyCloseButton.onClick.AddListener(CloseEmergencyNotification);
        }
        
        // Configure need bars
        ConfigureNeedBars();
    }
    
    private void ConfigureNeedBars()
    {
        Slider[] bars = { hungerBar, thirstBar, bladderBar, energyBar, stressBar };
        
        foreach (var bar in bars)
        {
            if (bar != null)
            {
                bar.minValue = 0f;
                bar.maxValue = 100f;
                bar.value = 100f;
                bar.interactable = false; // Read-only
            }
        }
        
        // Stress bar starts at 0
        if (stressBar != null)
        {
            stressBar.value = 0f;
        }
    }
    
    private void SubscribeToEvents()
    {
        if (needsManager != null)
        {
            needsManager.OnNeedBecameCritical += OnNeedBecameCritical;
            needsManager.OnNeedBecameEmergency += OnNeedBecameEmergency;
            needsManager.OnBureaucraticEvent += OnBureaucraticEvent;
        }
    }
    
    private void Update()
    {
        if (needsManager == null) return;
        
        UpdateNeedBars();
        UpdateNeedIcons();
        UpdateActionButtons();
        UpdateTimeDisplay();
        CheckForVisualEffects();
    }
    
    private void UpdateNeedBars()
    {
        // Update hunger bar
        if (hungerBar != null)
        {
            float hungerLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Hunger);
            hungerBar.value = hungerLevel;
            UpdateBarColor(hungerBar, hungerLevel, PhysiologicalNeed.Hunger);
        }
        
        // Update thirst bar
        if (thirstBar != null)
        {
            float thirstLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Thirst);
            thirstBar.value = thirstLevel;
            UpdateBarColor(thirstBar, thirstLevel, PhysiologicalNeed.Thirst);
        }
        
        // Update bladder bar
        if (bladderBar != null)
        {
            float bladderLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Bladder);
            bladderBar.value = bladderLevel;
            UpdateBarColor(bladderBar, bladderLevel, PhysiologicalNeed.Bladder);
        }
        
        // Update energy bar
        if (energyBar != null)
        {
            float energyLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Energy);
            energyBar.value = energyLevel;
            UpdateBarColor(energyBar, energyLevel, PhysiologicalNeed.Energy);
        }
        
        // Update stress bar (inverse logic)
        if (stressBar != null)
        {
            float stressLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Stress);
            stressBar.value = stressLevel;
            UpdateBarColor(stressBar, 100f - stressLevel, PhysiologicalNeed.Stress); // Inverse for color
        }
    }
    
    private void UpdateBarColor(Slider bar, float level, PhysiologicalNeed needType)
    {
        var need = needsManager.GetNeed(needType);
        if (need == null || bar.fillRect == null) return;
        
        Image fillImage = bar.fillRect.GetComponent<Image>();
        if (fillImage == null) return;
        
        Color targetColor;
        
        if (needType == PhysiologicalNeed.Stress)
        {
            // Stress bar: plus c'est élevé, plus c'est rouge
            float stressLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Stress);
            if (stressLevel >= need.emergencyThreshold)
                targetColor = need.emergencyColor;
            else if (stressLevel >= need.criticalThreshold)
                targetColor = need.criticalColor;
            else
                targetColor = need.normalColor;
        }
        else
        {
            // Autres besoins: moins c'est élevé, plus c'est rouge
            if (level <= need.emergencyThreshold)
                targetColor = need.emergencyColor;
            else if (level <= need.criticalThreshold)
                targetColor = need.criticalColor;
            else
                targetColor = need.normalColor;
        }
        
        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * 2f);
    }
    
    private void UpdateNeedIcons()
    {
        // Flash icons when critical
        float pulseIntensity = Mathf.PingPong(Time.time * 3f, 1f);
        
        UpdateIconFlash(hungerIcon, PhysiologicalNeed.Hunger, pulseIntensity);
        UpdateIconFlash(thirstIcon, PhysiologicalNeed.Thirst, pulseIntensity);
        UpdateIconFlash(bladderIcon, PhysiologicalNeed.Bladder, pulseIntensity);
        UpdateIconFlash(energyIcon, PhysiologicalNeed.Energy, pulseIntensity);
        UpdateIconFlash(stressIcon, PhysiologicalNeed.Stress, pulseIntensity);
    }
    
    private void UpdateIconFlash(Image icon, PhysiologicalNeed needType, float pulseIntensity)
    {
        if (icon == null) return;
        
        bool isCritical = needsManager.IsNeedCritical(needType);
        bool isEmergency = false;
        
        var need = needsManager.GetNeed(needType);
        if (need != null)
        {
            if (needType == PhysiologicalNeed.Stress)
            {
                isEmergency = needsManager.GetNeedLevel(needType) >= need.emergencyThreshold;
            }
            else
            {
                isEmergency = needsManager.GetNeedLevel(needType) <= need.emergencyThreshold;
            }
        }
        
        if (isEmergency)
        {
            // Flash rouge rapidement
            icon.color = Color.Lerp(Color.white, Color.red, pulseIntensity);
        }
        else if (isCritical)
        {
            // Flash orange lentement
            icon.color = Color.Lerp(Color.white, Color.magenta, pulseIntensity * 0.5f);
        }
        else
        {
            // Normal
            icon.color = Color.white;
        }
    }
    
    private void UpdateActionButtons()
    {
        // Toilet button
        if (toiletButton != null)
        {
            bool bladderCritical = needsManager.IsNeedCritical(PhysiologicalNeed.Bladder);
            toiletButton.interactable = bladderCritical || needsManager.GetNeedLevel(PhysiologicalNeed.Bladder) < 50f;
            
            // Change button color based on urgency
            var buttonImage = toiletButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (needsManager.GetNeedLevel(PhysiologicalNeed.Bladder) <= 10f)
                {
                    buttonImage.color = Color.red;
                }
                else if (bladderCritical)
                {
                    buttonImage.color = Color.magenta;
                }
                else
                {
                    buttonImage.color = Color.white;
                }
            }
        }
        
        // Lunch button (only during lunch break)
        if (lunchButton != null)
        {
            float currentTime = needsManager.GetCurrentGameTime();
            bool isLunchTime = currentTime >= 12f && currentTime < 13f;
            bool isHungry = needsManager.IsNeedCritical(PhysiologicalNeed.Hunger);
            
            lunchButton.interactable = isLunchTime || isHungry;
            
            // Update button text
            var buttonText = lunchButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (isLunchTime)
                {
                    buttonText.text = "DÉJEUNER\n(Autorisé)";
                    buttonText.color = Color.green;
                }
                else if (isHungry)
                {
                    buttonText.text = "GRIGNOTER\n(Risqué)";
                    buttonText.color = Color.magenta;
                }
                else
                {
                    buttonText.text = "MANGER\n(Interdit)";
                    buttonText.color = Color.red;
                }
            }
        }
        
        // Coffee button
        if (coffeeButton != null)
        {
            bool needsCoffee = needsManager.IsNeedCritical(PhysiologicalNeed.Thirst) || 
                              needsManager.IsNeedCritical(PhysiologicalNeed.Energy);
            coffeeButton.interactable = needsCoffee;
        }
        
        // Rest button (only available during breaks or when energy is critical)
        if (restButton != null)
        {
            bool canRest = needsManager.IsNeedCritical(PhysiologicalNeed.Energy) || 
                          !needsManager.IsWorkingHours();
            restButton.interactable = canRest;
        }
    }
    
    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            float currentTime = needsManager.GetCurrentGameTime();
            int hours = Mathf.FloorToInt(currentTime);
            int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);
            
            timeText.text = $"{hours:D2}:{minutes:D2}";
        }
        
        if (workStatusText != null)
        {
            float currentTime = needsManager.GetCurrentGameTime();
            
            if (currentTime >= 12f && currentTime < 13f)
            {
                workStatusText.text = "PAUSE DÉJEUNER";
                workStatusText.color = Color.green;
            }
            else if (currentTime >= 8f && currentTime <= 17f)
            {
                workStatusText.text = "HEURES DE TRAVAIL";
                workStatusText.color = Color.white;
            }
            else
            {
                workStatusText.text = "HORS SERVICE";
                workStatusText.color = Color.gray;
            }
        }
    }
    
    private void CheckForVisualEffects()
    {
        // Bladder urgency shake
        float bladderLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Bladder);
        if (bladderLevel <= 15f && shakeCoroutine == null)
        {
            shakeCoroutine = StartCoroutine(ShakeScreen());
        }
        else if (bladderLevel > 15f && shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        
        // Stress flash effect
        float stressLevel = needsManager.GetNeedLevel(PhysiologicalNeed.Stress);
        if (stressFlashEffect != null)
        {
            stressFlashEffect.SetActive(stressLevel > 80f);
        }
    }
    
    private IEnumerator ShakeScreen()
    {
        Vector3 originalPosition = transform.position;
        
        while (true)
        {
            float shakeIntensity = Mathf.InverseLerp(15f, 0f, needsManager.GetNeedLevel(PhysiologicalNeed.Bladder));
            
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeIntensity * 5f,
                Random.Range(-1f, 1f) * shakeIntensity * 5f,
                0f
            );
            
            transform.position = originalPosition + shakeOffset;
            
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    // Button event handlers
    private void OnToiletButtonClicked()
    {
        if (toiletUrgency != null) toiletUrgency.Stop();
        
        needsManager.UseToilet();
        
        // Visual feedback
        if (bladderShakeEffect != null)
        {
            bladderShakeEffect.SetActive(false);
        }
        
        ShowActionFeedback("Soulagement ! 💧");
    }
    
    private void OnLunchButtonClicked()
    {
        float currentTime = needsManager.GetCurrentGameTime();
        bool isLunchTime = currentTime >= 12f && currentTime < 13f;
        
        if (isLunchTime)
        {
            needsManager.EatLunch();
            ShowActionFeedback("Sandwich réglementaire consommé ! 🥪");
            
            if (stomachGrowl != null) stomachGrowl.Stop();
        }
        else
        {
            // Eating outside lunch hours - risky!
            needsManager.SatisfyNeed(PhysiologicalNeed.Hunger, 30f); // Less effective
            needsManager.SatisfyNeed(PhysiologicalNeed.Stress, -10f); // Adds stress
            
            ShowActionFeedback("Grignotage non-autorisé détecté ! +STRESS 😰");
            
            // Chance of generating disciplinary document
            if (Random.Range(0f, 1f) < 0.3f)
            {
                var gameUI = FindObjectOfType<GameUI>();
                if (gameUI != null)
                {
                    gameUI.ShowNotification("Formulaire disciplinaire généré pour 'Consommation non-autorisée' !");
                }
            }
        }
    }
    
    private void OnCoffeeButtonClicked()
    {
        needsManager.DrinkCoffee();
        
        // Visual effects
        if (coffeeParticles != null)
        {
            coffeeParticles.Play();
        }
        
        if (coffeeSlurp != null)
        {
            coffeeSlurp.Play();
        }
        
        ShowActionFeedback("Caféine intégrée avec succès ! ☕");
    }
    
    private void OnRestButtonClicked()
    {
        float currentTime = needsManager.GetCurrentGameTime();
        
        if (!needsManager.IsWorkingHours())
        {
            // Proper rest outside work hours
            needsManager.SatisfyNeed(PhysiologicalNeed.Energy, 60f);
            needsManager.SatisfyNeed(PhysiologicalNeed.Stress, 30f);
            ShowActionFeedback("Repos réparateur ! 😴");
        }
        else
        {
            // Quick rest during work - less effective and risky
            needsManager.SatisfyNeed(PhysiologicalNeed.Energy, 20f);
            needsManager.SatisfyNeed(PhysiologicalNeed.Stress, -5f); // Adds stress from guilt
            ShowActionFeedback("Micro-sieste non-autorisée ! 😬");
            
            // Chance of being caught
            if (Random.Range(0f, 1f) < 0.4f)
            {
                needsManager.SatisfyNeed(PhysiologicalNeed.Stress, -15f); // More stress!
                ShowActionFeedback("VOUS AVEZ ÉTÉ VU ! Rapport disciplinaire en cours... 🚨");
            }
        }
    }
    
    private void ShowActionFeedback(string message)
    {
        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            // Use the existing notification system
            var methodInfo = gameUI.GetType().GetMethod("ShowNotification", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (methodInfo != null)
            {
                methodInfo.Invoke(gameUI, new object[] { message });
            }
        }
        else
        {
            Debug.Log($"Action Feedback: {message}");
        }
    }
    
    // Event handlers from PhysiologicalNeedsManager
    private void OnNeedBecameCritical(PhysiologicalNeed needType)
    {
        string message = GetCriticalMessage(needType);
        ShowEmergencyNotification(message, false);
        
        // Play appropriate sound
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                if (stomachGrowl != null) stomachGrowl.Play();
                break;
            case PhysiologicalNeed.Bladder:
                if (toiletUrgency != null) toiletUrgency.Play();
                break;
            case PhysiologicalNeed.Stress:
                if (stressSound != null) stressSound.Play();
                break;
        }
        
        // Visual effects
        TriggerVisualEffect(needType, false);
    }
    
    private void OnNeedBecameEmergency(PhysiologicalNeed needType)
    {
        string message = GetEmergencyMessage(needType);
        ShowEmergencyNotification(message, true);
        
        // More intense visual effects
        TriggerVisualEffect(needType, true);
        
        // Emergency sound
        if (urgentBeep != null)
        {
            urgentBeep.Play();
        }
    }
    
    private void OnBureaucraticEvent(string eventMessage)
    {
        ShowActionFeedback(eventMessage);
    }
    
    private string GetCriticalMessage(PhysiologicalNeed needType)
    {
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                return "🍽️ ATTENTION ! Votre estomac gargouille dangereusement !";
            case PhysiologicalNeed.Thirst:
                return "💧 ALERTE ! Votre bouche devient de plus en plus sèche !";
            case PhysiologicalNeed.Bladder:
                return "🚽 URGENT ! Votre vessie commence à protester !";
            case PhysiologicalNeed.Energy:
                return "😴 FATIGUE ! Vos paupières deviennent lourdes !";
            case PhysiologicalNeed.Stress:
                return "😰 STRESS ! Votre œil gauche commence à trembler !";
            default:
                return "⚠️ Besoin physiologique critique !";
        }
    }
    
    private string GetEmergencyMessage(PhysiologicalNeed needType)
    {
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                return "🍽️ URGENCE ALIMENTAIRE ! Vous allez vous évanouir !";
            case PhysiologicalNeed.Thirst:
                return "💧 DÉSHYDRATATION CRITIQUE ! Trouvez de l'eau MAINTENANT !";
            case PhysiologicalNeed.Bladder:
                return "🚽 CATASTROPHE IMMINENTE ! TOILETTES IMMÉDIATEMENT !";
            case PhysiologicalNeed.Energy:
                return "😴 ÉPUISEMENT TOTAL ! Vous ne tenez plus debout !";
            case PhysiologicalNeed.Stress:
                return "😱 BURN-OUT EN COURS ! ÉVACUATION IMMÉDIATE !";
            default:
                return "🚨 URGENCE PHYSIOLOGIQUE MAXIMALE !";
        }
    }
    
    private void ShowEmergencyNotification(string message, bool isEmergency)
    {
        if (emergencyPanel == null || emergencyText == null) return;
        
        if (isShowingEmergency && !isEmergency) return; // Don't override emergency with critical
        
        emergencyText.text = message;
        emergencyPanel.SetActive(true);
        isShowingEmergency = true;
        
        // Auto-close after delay (longer for emergency)
        float autoCloseDelay = isEmergency ? 8f : 5f;
        Invoke(nameof(CloseEmergencyNotification), autoCloseDelay);
        
        // Make emergency notifications more dramatic
        if (isEmergency && emergencyPanel != null)
        {
            StartCoroutine(PulseEmergencyPanel());
        }
    }
    
    private IEnumerator PulseEmergencyPanel()
    {
        Image panelImage = emergencyPanel.GetComponent<Image>();
        if (panelImage == null) yield break;
        
        Color originalColor = panelImage.color;
        
        for (int i = 0; i < 6; i++) // Pulse 3 times
        {
            panelImage.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            panelImage.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private void CloseEmergencyNotification()
    {
        if (emergencyPanel != null)
        {
            emergencyPanel.SetActive(false);
        }
        isShowingEmergency = false;
    }
    
    private void TriggerVisualEffect(PhysiologicalNeed needType, bool isEmergency)
    {
        switch (needType)
        {
            case PhysiologicalNeed.Hunger:
                if (hungerGrowlEffect != null)
                {
                    hungerGrowlEffect.SetActive(true);
                    Invoke(nameof(DisableHungerEffect), isEmergency ? 3f : 1.5f);
                }
                break;
                
            case PhysiologicalNeed.Bladder:
                if (bladderShakeEffect != null)
                {
                    bladderShakeEffect.SetActive(true);
                    // Will be disabled when toilet is used
                }
                break;
                
            case PhysiologicalNeed.Stress:
                if (stressFlashEffect != null)
                {
                    stressFlashEffect.SetActive(true);
                    // Will be disabled when stress decreases
                }
                break;
        }
    }
    
    private void DisableHungerEffect()
    {
        if (hungerGrowlEffect != null)
        {
            hungerGrowlEffect.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (needsManager != null)
        {
            needsManager.OnNeedBecameCritical -= OnNeedBecameCritical;
            needsManager.OnNeedBecameEmergency -= OnNeedBecameEmergency;
            needsManager.OnBureaucraticEvent -= OnBureaucraticEvent;
        }
    }
}
