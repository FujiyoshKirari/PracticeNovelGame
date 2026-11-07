using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ScenarioRunner : MonoBehaviour
{
    [SerializeField] private string _url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.txt";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => RunAsync(destroyCancellationToken).Forget();

    private async UniTask RunAsync(CancellationToken ct)
    {
        try
        {
            IScenarioSource scenarioSource = new WebScenarioSource(_url);
            var text = await scenarioSource.LoadAsync(ct);
            var lines = ScenarioParser.ParseLines(text);

            int count = 1;
            foreach(var line in lines)
            {
                var t = $"{line.LineNumber}çsñ⁄ | [{line.Name}]";

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
