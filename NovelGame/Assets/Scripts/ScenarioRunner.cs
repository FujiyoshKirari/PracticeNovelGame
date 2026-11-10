using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ScenarioRunner : MonoBehaviour
{
    [SerializeField] private string _url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.csv";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => RunAsync(destroyCancellationToken).Forget();

    private async UniTask RunAsync(CancellationToken ct)
    {
        try
        {
            IScenarioSource scenarioSource = new WebScenarioSource(_url);
            var text = await scenarioSource.LoadAsync(ct);
            var commands = ScenarioParser.ParseLines(text, ',');

            int count = 1;

            Debug.Log(commands?[0].Name == "bg");

            foreach(var line in commands)
            {
                var t = $"{line.LineNumber}行目 | [{line.Name}]";

                foreach(var arg in line.Args)
                {
                    t += $" [{arg}]";
                }

                Debug.Log(t);
                count++;
            }
        }
        catch (UnityWebRequestException e)
        {
            Debug.LogError(e);
        }
    }
}
