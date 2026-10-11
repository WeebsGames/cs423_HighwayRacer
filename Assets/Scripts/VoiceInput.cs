using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.AdaptivePerformance;

public class VoiceInput : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    
    public TMP_Text text;
    public TMP_Text debugText;
    public string keyword = "boost";

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
            text.text = transcript;

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        string pattern = @"\b" + Regex.Escape(keyword.Trim()) + @"\b";

        if (Regex.IsMatch(transcript, pattern, RegexOptions.IgnoreCase))
        {
            Debug.Log("Keyword detected: " + keyword);
            // Trigger your gameplay action here.
            active = !active;
            if (active)
            {
                text.text = "Boost: Active";
            } else
            {
                text.text = "Boost: Inactive";
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
