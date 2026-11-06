using TMPro;
using UnityEngine;

public class VoiceInput : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    [Header("Debug Objects")]
    public TMP_Text boostText;
    public TMP_Text shieldText;
    public TMP_Text sirenText;
    public TMP_Text debugText;
    [Header("Display Objects")]
    public UnityEngine.UI.Image shieldImage;
    public UnityEngine.UI.Image sirenImage;
    public TMP_Text voiceFeedback;
    public UnityEngine.UI.Image voiceBar;
    [Header("Keywords")]
    public string[] boostKeyword = {"boost", "one", "speed", "faster", "go fast", "nitro", "Get my speed boost ready", "Prepare the boost for me",  "nos", "boosts"};
    public string[] shieldKeyword = {"shield", "two", "protect", "protection", "shields", "Get my shield ready", "Prepare the crash protection", "Prepare the power-up that protects my car", };
    public string[] sirenKeyword = {"siren", "three", "alarm", "emergency lights", "clear the path", "push cars away", "move other cars away", "clear my path", "out of the way", "silent"};
    public string[] pauseKeyword = {"pause", "stop", "quit", "menu", "leave", "leave race", "return", "exit", "freeze"};
    public SimplePause pause;
    [Header("Voice Corrections")]
    [Tooltip("Real-time seconds in which an explicit correction can replace the last voice action, including while paused.")]
    [Min(0f)] public float correctionWindowSeconds = 5f;
    [Tooltip("Multiplies the car's collision impulse on testBox-tagged boxes by 5. Boxes always use normal physics.")]
    public bool shieldActive = false;
    public bool sirenActive = false;

    [Header("Boost")]
    [Tooltip("Maximum boost duration in seconds. Starts fully charged.")]
    [SerializeField, Min(0.01f)] 
    private float maxBoostTime = 5f;
    [Tooltip("Seconds to refill an empty boost counter while boost is off. Partial charges can be used.")]
    [SerializeField, Min(0.01f)] 
    private float boostRefillTime = 10f;
    [Header("Shield")]
    public float maxShieldTime = 6f;
    public float shieldCD = 15f;
    [Header("Siren")]
    public float maxSirenTime = 5f;
    public float sirenCD = 15f;

    private SpeechRecognizerPlugin plugin;
    public bool boostActive { get; private set; } = false;
    public float boostRemaining { get; private set; }
    public float boostFillAmount => Mathf.Clamp01(boostRemaining / maxBoostTime);

    private float shieldRemaining;
    private float shieldFillAmount => shieldActive ? 0f : 1f - Mathf.Clamp01(shieldRemaining / maxShieldTime);
    private float sirenRemaining;
    private float sirenFillAmount => sirenActive ? 0f : 1f - Mathf.Clamp01(sirenRemaining / maxSirenTime);

    private enum Command { Boost, Shield, Siren, Pause }
    private struct VoiceAction
    {
        public Command Command;
        public bool WasActive;
        public bool IsActive;
    }

    private VoiceAction lastVoiceAction;
    private bool hasLastVoiceAction;
    private float lastVoiceActionTime;
    private bool awaitingCorrection;
    private float correctionRequestedTime;
    private int pendingOriginalCommandIndex = -1;
    private GameObject manager;
    private GlobalVars globalVars;

    private void Awake()
    {
        manager = GameObject.FindWithTag("GameController");
        globalVars = manager.GetComponent<GlobalVars>();

        boostRemaining = maxBoostTime;
        foreach (GameObject box in GameObject.FindGameObjectsWithTag("testBox"))
        {
            testBoxScript boxScript = box.GetComponent<testBoxScript>();
            if (boxScript == null)
            {
                boxScript = box.AddComponent<testBoxScript>();
            }
            boxScript.Initialize(this);
        }

        if (boostText == null)
        {
            Debug.LogWarning("No boost text assigned. Boost status text will not update.", this);
        }
        SetBoostActive(false);
    }

    private void SetBoostActive(bool active)
    {
        boostActive = active && boostRemaining > 0f;
        globalVars.boostActive = boostActive;
        if (boostText != null)
        {
            boostText.text = boostActive ? "Boost: Active" : "Boost: Inactive";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        shieldRemaining = maxShieldTime;
        sirenRemaining = maxSirenTime;
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(
            gameObject.name);
        BeginListening();
    }

    public void BeginListening()
    {
        plugin?.StartListening(true, "en-US");
    }

    public void OnResult(string recognizedResult)
    {
        if (voiceFeedback != null)
            voiceFeedback.color = Color.white;

        if (string.IsNullOrWhiteSpace(recognizedResult))
            return;

        if (debugText != null)
            debugText.text = "Recognizer returned: " + recognizedResult;

        // The plugin separates alternative transcripts with "~".
        string[] transcripts = recognizedResult.Split('~');
        if (voiceFeedback != null)
        {
            string[] recognizedPhrases = new string[3];
            int displayedPhraseCount = 0;
            for (int i = 0; i < transcripts.Length && displayedPhraseCount < recognizedPhrases.Length; i++)
            {
                string phrase = transcripts[i].Trim();
                if (!string.IsNullOrEmpty(phrase))
                {
                    recognizedPhrases[displayedPhraseCount++] = phrase;
                }
            }

            voiceFeedback.text = string.Join("\n", recognizedPhrases, 0, displayedPhraseCount);
        }

        VoiceCommandMatch match = VoiceCommandParser.Parse(
            recognizedResult, boostKeyword, shieldKeyword, sirenKeyword, pauseKeyword);
        float now = Time.unscaledTime;
        float window = Mathf.Max(0f, correctionWindowSeconds);
        bool recentAction = hasLastVoiceAction && now - lastVoiceActionTime <= window;
        awaitingCorrection = awaitingCorrection && now - correctionRequestedTime <= window;
        bool isCorrection = match.IsCorrection || awaitingCorrection;
        int originalCommandIndex = match.OriginalCommandIndex >= 0 ? match.OriginalCommandIndex
            : awaitingCorrection ? pendingOriginalCommandIndex : -1;
        bool inlineCorrection = originalCommandIndex >= 0;

        if (isCorrection && !inlineCorrection && hasLastVoiceAction && !recentAction)
        {
            awaitingCorrection = false;
            ShowFeedback("Correction window expired. Say your command again.", false);
            return;
        }

        if (match.IsAmbiguous || match.CommandIndex < 0)
        {
            if (isCorrection)
            {
                awaitingCorrection = true;
                correctionRequestedTime = now;
                pendingOriginalCommandIndex = originalCommandIndex;
                ShowFeedback("What did you mean? Say boost, shield, siren or pause.", false);
            }
            else
            {
                ShowFeedback("Say one command: boost, shield, siren or pause.", false);
            }
            return;
        }

        Command command = (Command)match.CommandIndex;
        bool undoPrevious = isCorrection && recentAction &&
            (!inlineCorrection || originalCommandIndex == (int)lastVoiceAction.Command);

        // Repeating the intended command in a correction must not toggle it twice.
        if (undoPrevious && command == lastVoiceAction.Command &&
            GetCommandState(command) == lastVoiceAction.IsActive)
        {
            awaitingCorrection = false;
            lastVoiceActionTime = now;
            ShowFeedback("Confirmed: " + command + ".", true);
            return;
        }

        if (isCorrection)
        {
            awaitingCorrection = true;
            correctionRequestedTime = now;
            pendingOriginalCommandIndex = originalCommandIndex;
        }

        if (!CanApplyCommand(command, isCorrection))
        {
            ShowFeedback(command + " is unavailable or recharging. Try another command.", false);
            return;
        }

        VoiceAction previousAction = lastVoiceAction;
        bool previousCurrentState = undoPrevious && GetCommandState(previousAction.Command);
        // Do not revive an effect that has already expired or overwrite a later
        // state change made by another input. Resources already used stay used.
        if (undoPrevious && previousCurrentState == previousAction.IsActive &&
            !SetCommandState(previousAction.Command, previousAction.WasActive))
        {
            ShowFeedback("The previous action cannot be reversed right now.", false);
            return;
        }

        bool wasActive = GetCommandState(command);
        bool desiredActive = command == Command.Boost || command == Command.Pause
            ? isCorrection || !wasActive
            : true;
        if (!SetCommandState(command, desiredActive))
        {
            // Pause can be refused by the game (for example, after losing cargo).
            // Restore the old effect if the replacement could not be applied.
            if (undoPrevious)
                SetCommandState(previousAction.Command, previousCurrentState);
            ShowFeedback(command + " cannot be applied right now.", false);
            return;
        }

        lastVoiceAction = new VoiceAction { Command = command, WasActive = wasActive, IsActive = GetCommandState(command) };
        hasLastVoiceAction = true;
        lastVoiceActionTime = now;
        awaitingCorrection = false;
        RefreshPowerUpUI();

        if (isCorrection)
        {
            string original = inlineCorrection ? ((Command)originalCommandIndex).ToString()
                : undoPrevious ? previousAction.Command.ToString() : null;
            ShowFeedback("Corrected: " + (original == null ? "" : original + " -> ") + command, true);
        }
        else
        {
            ShowFeedback(command + (lastVoiceAction.IsActive ? " activated" : " deactivated"), true);
        }
    }

    private bool CanApplyCommand(Command command, bool isCorrection)
    {
        switch (command)
        {
            case Command.Boost: return (!isCorrection && boostActive) || boostRemaining > 0f;
            case Command.Shield: return shieldActive || shieldRemaining >= maxShieldTime;
            case Command.Siren: return sirenActive || sirenRemaining >= maxSirenTime;
            default: return pause != null;
        }
    }

    private bool GetCommandState(Command command)
    {
        switch (command)
        {
            case Command.Boost: return boostActive;
            case Command.Shield: return shieldActive;
            case Command.Siren: return sirenActive;
            default: return pause != null && pause.IsPaused;
        }
    }

    private bool SetCommandState(Command command, bool active)
    {
        switch (command)
        {
            case Command.Boost:
                SetBoostActive(active);
                return boostActive == active;
            case Command.Shield:
                shieldActive = active;
                if (shieldText != null) shieldText.text = active ? "Shield: Active" : "Shield: Inactive";
                return true;
            case Command.Siren:
                sirenActive = active;
                if (sirenText != null) sirenText.text = active ? "Siren: Active" : "Siren: Inactive";
                return true;
            default:
                if (pause == null) return false;
                if (pause.IsPaused != active) pause.PauseToggle();
                return pause.IsPaused == active;
        }
    }

    private void ShowFeedback(string message, bool success)
    {
        Debug.Log(message);
        if (voiceFeedback == null) return;
        voiceFeedback.text = message;
        voiceFeedback.color = success ? Color.green : Color.white;
    }

    private void RefreshPowerUpUI()
    {
        if (shieldImage != null) shieldImage.fillAmount = shieldFillAmount;
        if (sirenImage != null) sirenImage.fillAmount = sirenFillAmount;
    }

    public void OnError(string recognizedError)
    {
        Debug.LogWarning("Speech recognition error: " + recognizedError);
    }

    private void OnDisable()
    {
        hasLastVoiceAction = false;
        awaitingCorrection = false;
        pendingOriginalCommandIndex = -1;
        if (!Application.isEditor)
            plugin?.StopListening();
    }
    // Update is called once per frame
    void Update()
    {
        if (boostActive)
        {
            boostRemaining = Mathf.Max(0f, boostRemaining - Time.deltaTime);
            if (boostRemaining <= 0f)
            {
                SetBoostActive(false);
            }
        }
        else
        {
            boostRemaining = Mathf.Min(maxBoostTime,
                boostRemaining + maxBoostTime / boostRefillTime * Time.deltaTime);
        }
        if (shieldActive)
        {
            shieldRemaining = Mathf.Max(0f, shieldRemaining - Time.deltaTime);
            if (shieldRemaining <= 0f)
            {
                shieldActive = false;
                if (shieldText != null) shieldText.text = "Shield: Inactive";
            }
        }
        else
        {
            shieldRemaining = Mathf.Min(maxShieldTime,
                shieldRemaining + maxShieldTime / shieldCD * Time.deltaTime);
        }
        if (sirenActive)
        {
            sirenRemaining = Mathf.Max(0f, sirenRemaining - Time.deltaTime);
            if (sirenRemaining <= 0f)
            {
                sirenActive = false;
                if (sirenText != null) sirenText.text = "Siren: Inactive";
            }
        }
        else
        {
            sirenRemaining = Mathf.Min(maxSirenTime,
                sirenRemaining + maxSirenTime / sirenCD * Time.deltaTime);
        }
        RefreshPowerUpUI();
    }
}
