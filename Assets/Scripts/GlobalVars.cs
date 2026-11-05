using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalVars : MonoBehaviour
{
    public int groundSpeed;
    public int boostSpeed;
    public bool gameEnd = false;
    public bool boostActive = false;
    public bool shieldActive = false;
    public bool sirenActive = false;
    public GameObject gameOverScreen;

    private int originalSpeed;

    void Start()
    {
        originalSpeed = groundSpeed;
    }
    void Update()
    {
        if (gameEnd)
        {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
        }

        if (boostActive)
        {
            groundSpeed = boostSpeed;
        } else
        {
            groundSpeed = originalSpeed;
        }
    }
}
