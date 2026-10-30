using UnityEngine;

public class SpeechRecognizerPlugin_Editor : SpeechRecognizerPlugin
{
    public SpeechRecognizerPlugin_Editor(string gameObjectName) : base(gameObjectName) { }

    private bool isContinuous = false;
    
    protected override void SetUp()
    {
        Debug.LogWarning("<b>WARNING</b>: You are running this plugin on Editor mode. Real recognition only works running on mobile device.");
    }

    public override void StartListening()
    {
        GameObject receiver = GameObject.Find(gameObjectName);

        if (receiver == null)
        {
            Debug.LogWarning("Speech callback GameObject not found: " + gameObjectName);
            return;
        }

        string result = this.isContinuous
            ? "continuous listening test"
            : "start listening test";

        receiver.SendMessage("OnResult", result);
    }

    public override void StartListening(bool setContinuousListening = false, string newLanguage = "en-US", int newMaxResults = 10)
    {
        StartListening();
    }        
    
    public override void StopListening()
    {
        throw new System.NotImplementedException();
    }

    public override void SetContinuousListening(bool isContinuous)
    {
        this.isContinuous = isContinuous;
    }

    public override void SetLanguageForNextRecognition(string newLanguage)
    {
        Debug.Log("Language set: " + newLanguage);
        this.language = newLanguage;
    }

    public override void SetMaxResultsForNextRecognition(int newMaxResults)
    {
        Debug.Log("Max results set: " + newMaxResults);
        this.maxResults = newMaxResults;
    }
}