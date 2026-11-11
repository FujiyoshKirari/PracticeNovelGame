using Cysharp.Threading.Tasks;
using MyDIContainer;
using System.Threading;
using UnityEngine;

public class ScenarioRunner : MonoBehaviour
{
    private IScenarioSource _scenarioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => RunAsync(destroyCancellationToken).Forget();

    [Inject]
    private void Construct(IScenarioSource scenarioSource)
    {
        _scenarioSource = scenarioSource;
    }

    private async UniTask RunAsync(CancellationToken ct)
    {
        try
        {
            var text = await _scenarioSource.LoadAsync(ct);
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
