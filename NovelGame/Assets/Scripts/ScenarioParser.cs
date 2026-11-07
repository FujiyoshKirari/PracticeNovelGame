using System.Collections.Generic;

public static class ScenarioParser
{
    public static List<string> ParseLines(string text)
    {
        var lines = text.Split('\n');
        var result = new List<string>();
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            if (trimmed.StartsWith('#')) continue;
            result.Add(trimmed);
        }
        return result;
    }
}
