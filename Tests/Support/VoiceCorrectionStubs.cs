// Minimal engine doubles for running the actual voice callbacks outside Unity.
// These files live outside Assets and are never included in the game build.
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Object
    {
        public static object FoundObject;
        public static T FindAnyObjectByType<T>() where T : class { return FoundObject as T; }
    }
    public class MonoBehaviour : Object { public GameObject gameObject = new GameObject(); }
    public class GameObject : Object
    {
        public string name = "Listener";
        public bool activeSelf;
        public static GameObject Player = new GameObject();
        public void SetActive(bool value) { activeSelf = value; }
        public static GameObject[] FindGameObjectsWithTag(string tag) { return new GameObject[0]; }
        public static GameObject FindWithTag(string tag) { return Player; }
        public T GetComponent<T>() where T : class { return null; }
        public T AddComponent<T>() where T : new() { return new T(); }
        public T[] GetComponentsInChildren<T>() { return new T[0]; }
    }
    public struct Color
    {
        public static Color white = new Color();
        public static Color green = new Color();
    }
    public static class Time { public static float unscaledTime, deltaTime, timeScale = 1f; }
    public static class Application { public static bool isEditor = true; }
    public static class Mathf
    {
        public static float Max(float a, float b) { return Math.Max(a, b); }
        public static float Min(float a, float b) { return Math.Min(a, b); }
        public static float Clamp01(float a) { return Math.Max(0f, Math.Min(1f, a)); }
    }
    public static class Debug
    {
        public static void Log(object message) { }
        public static void LogWarning(object message, object context = null) { }
    }
    public class HeaderAttribute : Attribute { public HeaderAttribute(string value) { } }
    public class TooltipAttribute : Attribute { public TooltipAttribute(string value) { } }
    public class MinAttribute : Attribute { public MinAttribute(float value) { } }
    public class SerializeField : Attribute { }
    public class AudioSource { public string name; public bool mute; public void Pause() { } public void UnPause() { } }
    public enum KeyCode { Escape, Delete, R }
    public static class Input { public static bool GetKeyDown(KeyCode key) { return false; } }
}
namespace UnityEngine.UI { public class Image { public float fillAmount; } }
namespace TMPro { public class TMP_Text { public string text; public UnityEngine.Color color; } }
namespace UnityEngine.SceneManagement
{
    public struct Scene { public string name; }
    public static class SceneManager
    {
        public static string LoadedScene;
        public static void LoadScene(string name) { LoadedScene = name; }
        public static Scene GetActiveScene() { return new Scene { name = "Track1" }; }
    }
}
public class testBoxScript { public void Initialize(VoiceInput input) { } }
public class ItemPhysics { public bool Fallen; public bool GetItemsFallen() { return Fallen; } }
public class LevelTimer { public void StopTimer() { } public void StartTimer() { } }
public class SpeechRecognizerPlugin
{
    public interface ISpeechRecognizerPlugin { void OnResult(string result); void OnError(string error); }
    public static SpeechRecognizerPlugin GetPlatformPluginVersion(string name) { return new SpeechRecognizerPlugin(); }
    public void StartListening(bool continuous, string language) { }
    public void StopListening() { }
}
