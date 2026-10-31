using System;
using System.Text.RegularExpressions;

public static class VoiceKeywordMatcher
{
    public static string FindMatch(string[] keywords, string[] transcripts)
    {
        if (keywords == null || transcripts == null)
            return null;

        string[] normalizedTranscripts = new string[transcripts.Length];
        for (int i = 0; i < transcripts.Length; i++)
            normalizedTranscripts[i] = " " + Normalize(transcripts[i]) + " ";

        foreach (string keyword in keywords)
        {
            string normalizedKeyword = Normalize(keyword);
            if (normalizedKeyword.Length == 0)
                continue;

            string phrase = " " + normalizedKeyword + " ";
            foreach (string transcript in normalizedTranscripts)
            {
                if (transcript.IndexOf(phrase, StringComparison.Ordinal) >= 0)
                    return keyword;
            }
        }

        return null;
    }

    private static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string normalized = Regex.Replace(text.ToLowerInvariant(), @"[^\p{L}\p{Nd}]+", " ").Trim();
        return Regex.Replace(normalized, @"\b[123]\b", match =>
        {
            switch (match.Value)
            {
                case "1": return "one";
                case "2": return "two";
                default: return "three";
            }
        });
    }
}
