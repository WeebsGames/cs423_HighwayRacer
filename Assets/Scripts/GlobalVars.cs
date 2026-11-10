using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalVars : MonoBehaviour
{
    public int groundSpeed;
    public int boostSpeed;
    public int score;
    public float spawnDelay = 5;
    public float timePassed = 0;
    public float difficultyTimer = 20;
    public bool gameEnd = false;
    public bool boostActive = false;
    public bool shieldActive = false;
    public bool sirenActive = false;
    public GameObject gameOverScreen;

    private int originalSpeed;
    private VoiceInput voiceInput;

    void Start()
    {
        originalSpeed = groundSpeed;
        voiceInput = GameObject.FindGameObjectWithTag("Listener").GetComponent<VoiceInput>();
    }
    void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= difficultyTimer)
        {
            timePassed = 0;
            spawnDelay -= spawnDelay/10;
        }
        if (gameEnd)
        {
            groundSpeed = 0;
            boostSpeed = 0;
            spawnDelay = 99999;
            gameOverScreen.SetActive(true);
        }

        if (boostActive)
        {
            groundSpeed = boostSpeed;
        } else
        {
            groundSpeed = originalSpeed;
        }
        boostActive = voiceInput.boostActive;
        shieldActive = voiceInput.shieldActive;
        sirenActive = voiceInput.sirenActive;

    }
}
