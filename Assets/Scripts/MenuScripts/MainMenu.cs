using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform lvlCamPos;
    public Transform mainCamPos;
    public Transform controlsCamPos;
    public Transform garageCamPos;
    public GameObject mainButtons;
    public GameObject levelButtons;
    public GameObject garageButtons;
    public GameObject cam;
    public GameObject credits;
    public GameObject controls;
    public GameObject poster;

    Transform lastPos;
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene1");
        // lastPos = cam.GetComponent<CameraFollow>().carTransform;
        // cam.GetComponent<CameraFollow>().carTransform = lvlCamPos;
        // // Camera.main.transform.position = lvlCamPos.position;
        // // Camera.main.transform.rotation = lvlCamPos.rotation;
        // mainButtons.SetActive(false);
        // levelButtons.SetActive(true);

    }
    public void Back()
    {
        // lastPos = cam.GetComponent<CameraFollow>().carTransform;
        mainButtons.SetActive(true);
        levelButtons.SetActive(false);
        garageButtons.SetActive(false);
        poster.SetActive(true);
        cam.GetComponent<CameraFollow>().carTransform = lastPos;
        // Camera.main.transform.position = mainCamPos.position;
        // Camera.main.transform.rotation = mainCamPos.rotation;
    }

    public void Controls()
    {
        mainButtons.SetActive(false);
        levelButtons.SetActive(true);
        controls.SetActive(true);
        credits.SetActive(false);
        lastPos = cam.GetComponent<CameraFollow>().carTransform;
        cam.GetComponent<CameraFollow>().carTransform = controlsCamPos;
    }

    public void Credits()
    {
        mainButtons.SetActive(false);
        levelButtons.SetActive(true);
        controls.SetActive(false);
        credits.SetActive(true);
        lastPos = cam.GetComponent<CameraFollow>().carTransform;
        cam.GetComponent<CameraFollow>().carTransform = controlsCamPos;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Garage()
    {
        mainButtons.SetActive(false);
        garageButtons.SetActive(true);
        poster.SetActive(false);
        lastPos = cam.GetComponent<CameraFollow>().carTransform;
        cam.GetComponent<CameraFollow>().carTransform = garageCamPos;
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Intro");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
}