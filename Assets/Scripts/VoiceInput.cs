using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.AdaptivePerformance;

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
    public string[] boostKeyword = {"boost", "one", "Get my speed boost ready", "Prepare the boost for me", "speed", "faster", "go fast", "nitro", "nos"};
    public string[] shieldKeyword = {"shield", "two", "Get my shield ready", "Prepare the crash protection", "Prepare the power-up that protects my car", "Protect", "protection"};
    public string[] sirenKeyword = {"siren", "three", "clear the path", "push cars away", "move other cars away", "clear my path", "out of the way", "alarm", "emergency lights"};
    public string[] pauseKeyword = {"pause", "stop", "quit", "menu", "leave", "leave race", "return", "exit", "freeze"};
    public SimplePause pause;
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

    private void Awake()
    {
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
        if (boostText != null)
        {
            boostText.text = boostActive ? "Boost: Active" : "Boost: Inactive";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
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
        if (string.IsNullOrWhiteSpace(recognizedResult))
            return;

        debugText.text = "Recognizer returned: " + recognizedResult;

        // The plugin separates alternative transcripts with "~".
        string[] transcripts = recognizedResult.Split('~');
        string[] recognizedWords = Regex.Split(transcripts[0].Trim(), @"\s+");
        int displayedWordCount = Mathf.Min(3, recognizedWords.Length);
        if (voiceFeedback != null)
        {
            voiceFeedback.text = string.Join(" ", recognizedWords, 0, displayedWordCount);
        }

        string matchedKeyword = FindMatchingKeyword(boostKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            SetBoostActive(!boostActive);
        }

        matchedKeyword = FindMatchingKeyword(shieldKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            // Trigger your gameplay action here.
            if(shieldRemaining == maxShieldTime)
            {
                shieldActive = true;
                shieldText.text = "Shield: Active";      
            }
        }

        matchedKeyword = FindMatchingKeyword(sirenKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            // Trigger your gameplay action here.
            if(sirenRemaining == maxSirenTime)
            {
                sirenActive = true;
                sirenText.text = "Siren: Active";
            }
        }

        matchedKeyword = FindMatchingKeyword(pauseKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            // Trigger your gameplay action here.
            pause.PauseToggle();
        }
    }

    private static string FindMatchingKeyword(string[] keywords, string[] transcripts)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(keywords[i]))
                continue;

            string pattern = @"\b" + Regex.Escape(keywords[i].Trim()) + @"\b";
            for (int transcriptIndex = 0;
                transcriptIndex < transcripts.Length && transcriptIndex < 3;
                transcriptIndex++)
            {
                if (Regex.IsMatch(transcripts[transcriptIndex], pattern, RegexOptions.IgnoreCase))
                    return keywords[i];
            }
        }

        return null;
    }

    public void OnError(string recognizedError)
    {
        Debug.LogWarning("Speech recognition error: " + recognizedError);
    }

    private void OnDisable()
    {
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
                shieldText.text = "Shield: Inactive";
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
                sirenText.text = "Siren: Inactive";
            }
        }
        else
        {
            sirenRemaining = Mathf.Min(maxSirenTime,
                sirenRemaining + maxSirenTime / sirenCD * Time.deltaTime);
        }
        shieldImage.fillAmount = shieldFillAmount;
        sirenImage.fillAmount = sirenFillAmount;
    }
}
