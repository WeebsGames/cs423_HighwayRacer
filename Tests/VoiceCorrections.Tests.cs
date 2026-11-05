using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VoiceCorrectionsTests
{
    private static int assertions;
    private static readonly string[][] Keywords = {
        new[] { "boost", "one", "go fast", "nos" }, new[] { "shield", "two", "protection" },
        new[] { "siren", "three", "alarm" }, new[] { "pause", "stop", "freeze" }
    };

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        assertions++;
    }

    private static void Invoke(object instance, string method)
    {
        instance.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, null);
    }

    private static float Remaining(VoiceInput voice, string field)
    {
        return (float)typeof(VoiceInput).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(voice);
    }

    private static VoiceInput NewVoice()
    {
        Time.unscaledTime = 0f;
        Time.timeScale = 1f;
        UnityEngine.Object.FoundObject = null;
        VoiceInput voice = new VoiceInput {
            voiceFeedback = new TMP_Text(), boostText = new TMP_Text(),
            shieldText = new TMP_Text(), sirenText = new TMP_Text(),
            shieldImage = new UnityEngine.UI.Image(), sirenImage = new UnityEngine.UI.Image(),
            pause = new SimplePause { pause = new GameObject(), levelTimer = new LevelTimer(), others = new List<GameObject>() }
        };
        Invoke(voice, "Awake");
        Invoke(voice, "Start");
        return voice;
    }

    private static void Advance(VoiceInput voice, float seconds)
    {
        Time.unscaledTime += seconds;
        Time.deltaTime = seconds * Time.timeScale;
        Invoke(voice, "Update");
    }

    private static void ExpectParse(string phrase, int command, bool correction, int original = -1)
    {
        VoiceCommandMatch match = VoiceCommandParser.Parse(phrase, Keywords);
        Check(match.CommandIndex == command, "Wrong command: " + phrase);
        Check(match.IsCorrection == correction, "Wrong correction flag: " + phrase);
        Check(match.OriginalCommandIndex == original, "Wrong rejected command: " + phrase);
    }

    public static int Main()
    {
        try
        {
            ParserCases();
            CallbackCases();
            Console.WriteLine("PASS: " + assertions + " correction assertions (parser and actual VoiceInput/SimplePause/MenuListener callbacks).");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    private static void ParserCases()
    {
        ExpectParse("boost", 0, false);
        ExpectParse("boost, no, shield", 1, true, 0);
        ExpectParse("boost -- sorry, siren", 2, true, 0);
        ExpectParse("boost actually shield", 1, true, 0);
        ExpectParse("boost make that shield", 1, true, 0);
        ExpectParse("boost I mean shield", 1, true, 0);
        ExpectParse("boost no sorry I meant shield", 1, true, 0);
        ExpectParse("boost no shield actually siren", 2, true, 1);
        ExpectParse("shield instead of boost", 1, true, 0);
        ExpectParse("shield instead", 1, true);
        ExpectParse("boost no shield instead", 1, true, 0);
        ExpectParse("I meant shield", 1, true);
        ExpectParse("SORRY! I MEANT 3.", 2, true);
        ExpectParse("1, no, 2", 1, true, 0);
        ExpectParse("nos", 0, false);
        ExpectParse("noise", -1, false);
        ExpectParse("no", -1, true);
        ExpectParse("boost no", -1, true, 0);
        ExpectParse("boost no unknown~boost", -1, true, 0);
        ExpectParse("boost no she'll~boost no shield", 1, true, 0);
        ExpectParse("boost~shield", 0, false);
        ExpectParse("unknown~nothing~nothing else~shield", 1, false);
        ExpectParse("   ~actually shield~boost", 1, true);
        ExpectParse(null, -1, false);
        ExpectParse("  ", -1, false);
        Check(VoiceCommandParser.Parse("boost and shield", Keywords).IsAmbiguous, "Multiple actions must not fire together");
    }

    private static void CallbackCases()
    {
        VoiceInput voice = NewVoice();
        voice.OnResult("boost no shield");
        Check(!voice.boostActive && voice.shieldActive && !voice.sirenActive, "Inline correction executed the rejected action");
        Check(voice.boostRemaining == 5f, "Inline correction consumed boost");
        Check(voice.voiceFeedback.text == "Corrected: Boost -> Shield", "Missing correction feedback");

        voice = NewVoice();
        voice.OnResult("boost");
        Advance(voice, 1f);
        voice.OnResult("sorry I meant shield");
        Check(!voice.boostActive && voice.shieldActive, "Cross-result correction did not replace boost");
        Check(voice.boostRemaining == 4f, "Correction refunded used boost");
        Check(voice.boostText.text == "Boost: Inactive", "Rejected boost UI is stale");
        Check(voice.shieldText.text == "Shield: Active", "Replacement UI is stale");
        Check(voice.voiceFeedback.text == "Corrected: Boost -> Shield", "Cross-result feedback is incorrect");
        Advance(voice, 0.5f);
        voice.OnResult("no siren");
        Check(!voice.shieldActive && voice.sirenActive, "Repeated correction did not replace shield");
        Check(Remaining(voice, "shieldRemaining") == 5.5f, "Correction reset shield cooldown");
        voice.OnResult("no shield");
        Check(voice.sirenActive && !voice.shieldActive, "Unavailable replacement stopped the original effect");
        Check(voice.voiceFeedback.text.Contains("recharging"), "Unavailable replacement needs feedback");

        voice = NewVoice();
        voice.OnResult("boost");
        voice.OnResult("shield");
        Check(voice.boostActive && voice.shieldActive, "Normal next command was treated as a correction");
        voice.OnResult("no siren");
        Check(voice.boostActive && !voice.shieldActive && voice.sirenActive, "Correction undid more than the last action");

        voice = NewVoice();
        voice.OnResult("boost");
        voice.OnResult("I meant boost");
        Check(voice.boostActive, "Same-command correction toggled boost off");
        Check(voice.voiceFeedback.text.Contains("Confirmed"), "Same-command correction needs confirmation");

        voice = NewVoice();
        voice.OnResult("boost");
        voice.OnResult("boost");
        Check(!voice.boostActive, "Normal boost toggle regressed");
        voice.OnResult("actually shield");
        Check(voice.boostActive && voice.shieldActive, "Undo did not restore the mistaken boost deactivation");

        voice = NewVoice();
        voice.OnResult("boost~shield~siren");
        Check(voice.boostActive && !voice.shieldActive && !voice.sirenActive, "Speech alternatives executed as multiple commands");

        voice = NewVoice();
        voice.OnResult("boost no nonsense~boost");
        Check(!voice.boostActive, "Unknown replacement executed the rejected boost");
        voice.OnResult("shield");
        Check(voice.shieldActive && !voice.boostActive, "Replacement after clarification failed");

        voice = NewVoice();
        voice.OnResult("boost");
        Advance(voice, 1f);
        voice.OnResult("no");
        Check(voice.boostActive, "Incomplete correction changed the game");
        voice.OnResult("siren");
        Check(!voice.boostActive && voice.sirenActive, "Two-part correction failed");

        voice = NewVoice();
        voice.OnResult("shield");
        Advance(voice, 5.01f);
        voice.OnResult("I meant siren");
        Check(voice.shieldActive && !voice.sirenActive, "Expired correction changed an earlier action");
        Check(voice.voiceFeedback.text.Contains("expired"), "Expired correction needs feedback");
        voice.OnResult("siren");
        Check(voice.sirenActive && voice.shieldActive, "Normal command after an expired correction failed");

        voice = NewVoice();
        voice.OnResult("shield");
        Advance(voice, 5f);
        voice.OnResult("no siren");
        Check(!voice.shieldActive && voice.sirenActive, "Correction at window boundary failed");

        voice = NewVoice();
        voice.OnResult("shield");
        Advance(voice, 6f);
        voice.OnResult("boost no siren");
        Check(!voice.boostActive && voice.sirenActive, "Self-contained inline correction depended on expired history");

        voice = NewVoice();
        voice.OnResult("siren");
        voice.OnResult("boost no shield");
        Check(voice.sirenActive && voice.shieldActive && !voice.boostActive, "Inline correction undid an unrelated earlier action");

        voice = NewVoice();
        voice.OnResult("siren");
        voice.OnResult("boost no");
        voice.OnResult("sorry I meant shield");
        Check(voice.sirenActive && voice.shieldActive && !voice.boostActive, "Incomplete inline correction lost its original command");
        Check(voice.voiceFeedback.text == "Corrected: Boost -> Shield", "Incomplete inline correction lost its feedback context");

        voice = NewVoice();
        voice.OnResult("pause");
        Check(voice.pause.IsPaused && Time.timeScale == 0f, "Pause command failed");
        Advance(voice, 1f);
        voice.OnResult("no shield");
        Check(!voice.pause.IsPaused && Time.timeScale == 1f && voice.shieldActive, "Correction while paused did not resume the game");

        voice = NewVoice();
        voice.OnResult("pause");
        Advance(voice, 6f);
        voice.OnResult("no boost");
        Check(voice.pause.IsPaused && !voice.boostActive, "Pause froze the correction timeout");

        voice = NewVoice();
        voice.OnResult("boost");
        UnityEngine.Object.FoundObject = new ItemPhysics { Fallen = true };
        voice.OnResult("no pause");
        Check(voice.boostActive && !voice.pause.IsPaused, "Failed pause replacement did not restore boost");
        voice.OnResult("shield");
        Check(!voice.boostActive && voice.shieldActive, "Retry after unavailable correction lost its context");

        voice = NewVoice();
        voice.OnResult("pause");
        voice.pause.PauseToggle(); // Another input already resumed the game.
        voice.OnResult("no siren");
        Check(!voice.pause.IsPaused && voice.sirenActive, "Correction overwrote a later external state change");

        voice = NewVoice();
        voice.OnResult("boost and shield");
        Check(!voice.boostActive && !voice.shieldActive, "Ambiguous command changed the game");

        voice = NewVoice();
        voice.OnResult("actually shield");
        Check(voice.shieldActive, "Correction after an unrecognized original command failed");
        Invoke(voice, "OnDisable");
        voice.OnResult("no siren");
        Check(voice.shieldActive && voice.sirenActive, "Disabled listener retained stale undo history");

        voice = NewVoice();
        voice.voiceFeedback = null;
        voice.shieldText = null;
        voice.sirenText = null;
        voice.shieldImage = null;
        voice.sirenImage = null;
        voice.OnResult("shield no siren");
        Advance(voice, 1f);
        voice.OnResult("no boost");
        Check(voice.boostActive && !voice.sirenActive, "Optional UI references blocked correction handling");

        SceneManager.LoadedScene = null;
        MenuListener menu = new MenuListener();
        menu.OnResult("start no wait~start");
        Check(SceneManager.LoadedScene == null, "Menu launched a rejected start command");
        menu.OnResult("wait actually start");
        Check(SceneManager.LoadedScene == "Track1", "Corrected menu start failed");
    }
}
