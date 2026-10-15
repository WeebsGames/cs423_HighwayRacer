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
    public bool shieldActive = false;
    public bool sirenActive = false;

    private SpeechRecognizerPlugin plugin;
    public bool boostActive { get; private set; } = false;

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
        string transcript = recognizedResult.Split('~')[0];

        for (int i = 0; i < boostKeyword.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(boostKeyword[i]))
                return;

            string pattern = @"\b" + Regex.Escape(boostKeyword[i].Trim()) + @"\b";

            if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
            {
                Debug.Log("Keyword detected: " + boostKeyword[i]);
                // Trigger your gameplay action here.
                boostActive = !boostActive;
                if (boostActive)
                {
                    boostText.text = "Boost: Active";
                } else
                {
                    boostText.text = "Boost: Inactive";
                }
                break;
            }
        }

        for (int i = 0; i < shieldKeyword.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(shieldKeyword[i]))
                return;

            string pattern = @"\b" + Regex.Escape(shieldKeyword[i].Trim()) + @"\b";

            if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
            {
                Debug.Log("Keyword detected: " + shieldKeyword[i]);
                // Trigger your gameplay action here.
                shieldActive = !shieldActive;
                if (shieldActive)
                {
                    shieldText.text = "Shield: Active";
                } else
                {
                    shieldText.text = "Shield: Inactive";
                }
                break;
            }
        }

        for (int i = 0; i < sirenKeyword.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(sirenKeyword[i]))
                return;

            string pattern = @"\b" + Regex.Escape(sirenKeyword[i].Trim()) + @"\b";

            if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
            {
                Debug.Log("Keyword detected: " + sirenKeyword[i]);
                // Trigger your gameplay action here.
                sirenActive = !sirenActive;
                if (sirenActive)
                {
                    sirenText.text = "Siren: Active";
                } else
                {
                    sirenText.text = "Siren: Inactive";
                }
                break;
            }
        }

        for (int i = 0; i < pauseKeyword.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(pauseKeyword[i]))
                return;

            string pattern = @"\b" + Regex.Escape(pauseKeyword[i].Trim()) + @"\b";

            if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
            {
                Debug.Log("Keyword detected: " + pauseKeyword[i]);
                // Trigger your gameplay action here.
                    pause.PauseToggle();
                break;
            }
        }
        
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
        
    }
}
