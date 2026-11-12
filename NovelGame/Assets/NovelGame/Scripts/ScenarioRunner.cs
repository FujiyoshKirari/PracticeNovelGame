using Cysharp.Threading.Tasks;
using MyDIContainer;
using R3;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScenarioRunner : MonoBehaviour
{
    private IScenarioSource _scenarioSource;
    private List<ScenarioCommand> _scenarioCommands;
    [SerializeField] private InputAction _action;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => RunAsync(destroyCancellationToken).Forget();

    [Inject]
    private void Construct(IScenarioSource scenarioSource)
    {
        _scenarioSource = scenarioSource;
    }

    private void Awake()
    {
        _action?.Enable();
        Observable.EveryUpdate()
            .Where(_ => _action?.triggered == true)
            .Subscribe(_ => OnButtonPressDown())
            .AddTo(this);
    }

    private async UniTask RunAsync(CancellationToken ct)
    {
        try
        {
            var text = await _scenarioSource.LoadAsync(ct);
            _scenarioCommands = ScenarioParser.ParseLines(text, ',');

            int count = 1;

            Debug.Log(_scenarioCommands?[0].Name == "bg");

            foreach(var line in _scenarioCommands)
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

    private void OnButtonPressDown()
    {
        Debug.Log("Pressed");
    }
}
