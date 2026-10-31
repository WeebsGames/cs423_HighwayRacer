using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalVars : MonoBehaviour
{
    public int groundSpeed;
    public bool gameEnd = false;
    public GameObject gameOverScreen;
    void Update()
    {
        if (gameEnd)
        {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
        }
    }
}
