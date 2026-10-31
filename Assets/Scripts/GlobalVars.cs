using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalVars : MonoBehaviour
{
    public int groundSpeed;
    public bool gameEnd = false;

    void Update()
    {
        if (gameEnd)
        {
            SceneManager.LoadScene("MainMenu2");
        }
    }
}
