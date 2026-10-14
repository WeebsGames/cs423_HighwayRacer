using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.AdaptivePerformance;

public class VoiceInput : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    
    public TMP_Text text;
    public TMP_Text debugText;
    public string[] boostKeyword = {"boost", "one", "Get my speed boost ready", "Prepare the boost for me", "speed"};
    public string[] shieldKeyword = {"shield", "two", "Get my shield ready", "Prepare the crash protection", "Prepare the power-up that protects my car", "Protect"};
    public string[] sirenKeyword = {"siren", "two", "clear the path", "push cars away", "move other cars away", "clear my path"};

    private SpeechRecognizerPlugin plugin;
    private bool active = false;

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

        if (text != null)
            // text.text = transcript;

        for (int i = 0; i < boostKeyword.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(boostKeyword[i]))
                return;

            string pattern = @"\b" + Regex.Escape(boostKeyword[i].Trim()) + @"\b";

            if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
            {
                Debug.Log("Keyword detected: " + boostKeyword[i]);
                // Trigger your gameplay action here.
                active = !active;
                if (active)
                {
                    text.text = "Boost: Active";
                } else
                {
                    text.text = "Boost: Inactive";
                }
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
