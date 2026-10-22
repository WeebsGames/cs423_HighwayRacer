using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class MenuListener : MonoBehaviour, SpeechRecognizerPlugin.ISpeechRecognizerPlugin
{
    [Header("Optional Feedback")]
    public TMP_Text debugText;
    public TMP_Text voiceFeedback;

    [Header("Keywords")]
    public string[] playKeyword = {"play", "start", "race", "proceed"};

    private SpeechRecognizerPlugin plugin;
    private bool isLoadingScene;

    private void OnEnable()
    {
        isLoadingScene = false;
        if (plugin == null)
            plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(gameObject.name);

        BeginListening();
    }

    public void BeginListening()
    {
        // The Editor stub sends "start listening test", which would trigger a race.
        if (!Application.isEditor)
            plugin?.StartListening(true, "en-US");
    }

    public void OnResult(string recognizedResult)
    {
        if (isLoadingScene)
            return;

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

        string matchedKeyword = FindMatchingKeyword(playKeyword, transcripts);
        if (matchedKeyword != null)
        {
            Debug.Log("Keyword detected: " + matchedKeyword);
            if (voiceFeedback != null)
                voiceFeedback.color = Color.green;

            isLoadingScene = true;
            SceneManager.LoadScene("Track1");
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
        if (!Application.isEditor)
            plugin?.StopListening();
    }
}
