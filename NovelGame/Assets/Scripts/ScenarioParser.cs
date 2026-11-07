using System.Collections.Generic;

public static class ScenarioParser
{
    public static List<ScenarioCommand> ParseLines(string text)
    {
        var lines = text.Split('\n');
        var result = new List<ScenarioCommand>();

        var argsList = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var splitNameLine = lines[i].Split('\t');
            var firstString = splitNameLine[0].Trim();

            if (firstString.StartsWith('#')) continue;
            if (string.IsNullOrEmpty(firstString)) continue;

            var command = new ScenarioCommand();
            command.LineNumber = i + 1;
            command.Name = firstString;

            argsList.Clear();

            for(int j = 1; j < splitNameLine.Length; j++)
            {
                var trimmed = splitNameLine[j].Trim();
                argsList.Add(trimmed);
            }
            command.Args = argsList.ToArray();
            result.Add(command);
        }
        return result;
    }
}
