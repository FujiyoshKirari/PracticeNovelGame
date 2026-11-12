using Cysharp.Threading.Tasks;
using MyDIContainer;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScenarioRunner : MonoBehaviour
{
    [SerializeField] private InputAction _action;
    private IScenarioSource _scenarioSource;

    private MessageWindow _messageWindow;

    void Start() => RunAsync(destroyCancellationToken).Forget();

    [Inject]
    private void Construct(IScenarioSource scenarioSource, MessageWindow messageWindow)
    {
        _scenarioSource = scenarioSource;
        _messageWindow = messageWindow;
    }

    private void Awake()
    {
        _action?.Enable();
    }

    private async UniTask RunAsync(CancellationToken ct)
    {
        try
        {
            var text = await _scenarioSource.LoadAsync(ct);
            var scenarioCommands = ScenarioParser.ParseLines(text, ',');

            foreach(var command in scenarioCommands)
            {
                var t = $"{command.LineNumber}行目 | [{command.Name}]";

                foreach(var arg in command.Args)
                {
                    t += $" [{arg}]";
                }

                Debug.Log(t);
            }

            foreach (var command in scenarioCommands)
            {
                await ExecuteCommand(command, ct);
            }
        }
        catch (UnityWebRequestException e)
        {
            Debug.LogError(e);
        }
    }

    private async UniTask ExecuteCommand(ScenarioCommand command, CancellationToken ct)
    {
        switch (command?.Name)
        {
            case null:
                throw new Exception("ScenarioCommandがnullでした");
            case "text":
                await ShowText(command, ct);
                await UniTask.WaitUntil(() => _action.triggered, cancellationToken: ct);
                break;

            default:
                Debug.LogWarning($"未対応のコマンド {command.Name}({command.LineNumber}行目)");
                break;
        }
    }

    private async UniTask ShowText(ScenarioCommand command, CancellationToken ct)
    {
        if (command?.Args?.Length >= 2)
        {
            var speaker = command.Args[0];
            var body = command.Args[1];

            await _messageWindow.Show(speaker, body, ct);
        }
        else if (command == null) 
        {
            throw new Exception("ScenarioCommandがnullで渡されました。");
        }
        else
        {
            throw new Exception($"{command.LineNumber}行目のテキストの要素数が足りません");
        }
    }
}
