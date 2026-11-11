using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject canvas;

    private GameObject manager;
    private GlobalVars globalVars;
    private SimplePause pauseMenu;
    private TMP_Text self;
    private float timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<TMP_Text>();
        manager = GameObject.FindWithTag("GameController");
        globalVars = manager.GetComponent<GlobalVars>(); 
        pauseMenu = canvas.GetComponent<SimplePause>();

    }

    // Update is called once per frame
    void Update()
    {
        if (globalVars.gameEnd ) return;
        timer += Time.deltaTime;
        string formattedTime = System.TimeSpan.FromSeconds(timer).ToString("mm':'ss");
        self.text = formattedTime;
    }
}
