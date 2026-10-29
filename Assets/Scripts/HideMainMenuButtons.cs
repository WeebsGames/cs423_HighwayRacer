using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class HideMainMenuButtons
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu2") return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                button.interactable = false;
                CanvasGroup group = button.GetComponent<CanvasGroup>();
                if (group == null) group = button.gameObject.AddComponent<CanvasGroup>();
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }
    }
}
