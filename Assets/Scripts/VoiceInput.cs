using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.AdaptivePerformance;

public class VoiceInput : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    
    public TMP_Text boostText;
    public TMP_Text shieldText;
    public TMP_Text sirenText;
    public TMP_Text debugText;
    public string[] boostKeyword = {"boost", "one", "Get my speed boost ready", "Prepare the boost for me", "speed"};
    public string[] shieldKeyword = {"shield", "two", "Get my shield ready", "Prepare the crash protection", "Prepare the power-up that protects my car", "Protect"};
    public string[] sirenKeyword = {"siren", "two", "clear the path", "push cars away", "move other cars away", "clear my path"};
    public string[] pauseKeyword = {"pause", "stop", "quit", "menu", "leave", "leave race", "return", "exit"};
    public SimplePause pause;
    [Tooltip("Allows testBox-tagged boxes to move under physics. When off, they remain solid and immovable.")]
    public bool shieldActive = false;
    public bool sirenActive = false;

    [Header("Boost")]
    [Tooltip("Maximum boost duration in seconds. Starts fully charged.")]
    [SerializeField, Min(0.01f)] private float maxBoostTime = 5f;
    [Tooltip("Seconds to refill an empty boost counter while boost is off. Partial charges can be used.")]
    [SerializeField, Min(0.01f)] private float boostRefillTime = 10f;

    private SpeechRecognizerPlugin plugin;
    public bool boostActive { get; private set; } = false;
    public float boostRemaining { get; private set; }
    public float boostFillAmount => Mathf.Clamp01(boostRemaining / maxBoostTime);

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
            shieldActive = !shieldActive;
            if (shieldActive)
            {
                shieldText.text = "Shield: Active";
            } else
            {
                shieldText.text = "Shield: Inactive";
            }
        }

        matchedKeyword = FindMatchingKeyword(sirenKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            // Trigger your gameplay action here.
            sirenActive = !sirenActive;
            if (sirenActive)
            {
                sirenText.text = "Siren: Active";
            } else
            {
                sirenText.text = "Siren: Inactive";
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
    }
}
