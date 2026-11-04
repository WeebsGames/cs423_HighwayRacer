using System.Text.RegularExpressions;

public sealed class VoiceCommandMatch
{
    public int CommandIndex { get; internal set; } = -1;
    public int OriginalCommandIndex { get; internal set; } = -1;
    public string Keyword { get; internal set; }
    public bool IsCorrection { get; internal set; }
    public bool IsAmbiguous { get; internal set; }
}

// Parses speech-service alternatives as competing interpretations, not separate actions.
public static class VoiceCommandParser
{
    private static readonly Regex CorrectionMarker = new Regex(
        @"\b(?:i meant|i mean|make that|sorry|actually|no|instead(?: of)?|correction)\b",
        RegexOptions.IgnoreCase);

    public static VoiceCommandMatch Parse(string recognizedResult, params string[][] keywordGroups)
    {
        if (string.IsNullOrWhiteSpace(recognizedResult) || keywordGroups == null)
            return new VoiceCommandMatch();

        VoiceCommandMatch unresolvedCorrection = null;
        foreach (string transcript in recognizedResult.Split('~'))
        {
            VoiceCommandMatch result = ParseTranscript(transcript, keywordGroups);
            if (unresolvedCorrection != null)
            {
                // Try another corrected interpretation, never the rejected
                // command from an alternative that omitted the correction.
                if (result.IsCorrection && !result.IsAmbiguous && result.CommandIndex >= 0)
                    return result;
                continue;
            }

            if (result.IsCorrection && result.CommandIndex < 0)
                unresolvedCorrection = result;
            else if (result.IsCorrection || result.IsAmbiguous || result.CommandIndex >= 0)
                return result;
        }

        return unresolvedCorrection ?? new VoiceCommandMatch();
    }

    private static VoiceCommandMatch ParseTranscript(string transcript, string[][] keywordGroups)
    {
        string text = VoiceKeywordMatcher.Normalize(transcript);
        MatchCollection markers = CorrectionMarker.Matches(text);
        if (markers.Count == 0)
            return FindCommand(text, keywordGroups);

        Match marker = markers[markers.Count - 1];
        string before = text.Substring(0, marker.Index).Trim();
        string after = text.Substring(marker.Index + marker.Length).Trim();

        VoiceCommandMatch replacement;
        VoiceCommandMatch original;
        if (marker.Value == "instead of")
        {
            // "Shield instead of boost" names the desired command first.
            replacement = ParseTranscript(before, keywordGroups);
            original = FindCommand(after, keywordGroups);
        }
        else if (marker.Value == "instead" && after.Length == 0)
        {
            // "Shield instead" is a correction of the previous voice result.
            replacement = ParseTranscript(before, keywordGroups);
            original = new VoiceCommandMatch { CommandIndex = replacement.OriginalCommandIndex };
        }
        else
        {
            replacement = FindCommand(after, keywordGroups);
            original = ParseTranscript(before, keywordGroups);
        }

        replacement.IsCorrection = true;
        replacement.OriginalCommandIndex = original.CommandIndex >= 0
            ? original.CommandIndex : original.OriginalCommandIndex;
        return replacement;
    }

    private static VoiceCommandMatch FindCommand(string transcript, string[][] keywordGroups)
    {
        VoiceCommandMatch result = new VoiceCommandMatch();
        for (int i = 0; i < keywordGroups.Length; i++)
        {
            string keyword = VoiceKeywordMatcher.FindMatch(keywordGroups[i], new[] { transcript });
            if (keyword == null)
                continue;

            if (result.CommandIndex >= 0)
            {
                result.CommandIndex = -1;
                result.Keyword = null;
                result.IsAmbiguous = true;
                return result;
            }

            result.CommandIndex = i;
            result.Keyword = keyword;
        }

        return result;
    }
}
