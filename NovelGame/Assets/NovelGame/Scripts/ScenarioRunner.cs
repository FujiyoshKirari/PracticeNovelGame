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
    private CharacterStage _characterStage;
    private ISpriteSource _spriteSource;
    private MessageWindow _messageWindow;

    void Start() => RunAsync(destroyCancellationToken).Forget();

    [Inject]
    private void Construct(
        IScenarioSource scenarioSource,
        MessageWindow messageWindow,
        CharacterStage characterStage,
        ISpriteSource spriteSource)
    {
        _scenarioSource = scenarioSource;
        _characterStage = characterStage;
        _spriteSource = spriteSource;
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
                ShowText(command, ct);
                while (true)
                {
                    await UniTask.WaitUntil(() => _action.triggered, cancellationToken: ct);
                    if (_messageWindow.IsTyping)
                    {
                        _messageWindow.Complete();
                        await UniTask.Yield(ct);
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case "show":
                await ShowImage(command, ct);
                break;
            default:
                Debug.LogWarning($"未対応のコマンド {command.Name}({command.LineNumber}行目)");
                break;
        }
    }

    private void ShowText(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command))
        {
            var speaker = command.Args[0];
            var body = command.Args[1];

            _messageWindow.Show(speaker, body);
        }
    }

    private async UniTask ShowImage(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command))
        {
            var name = command.Args[0];
            var position = command.Args[1];
            var sprite = await _spriteSource.LoadAsync("Characters/" + name, ct);
            _characterStage.Show(name, sprite, position).Forget();
        }
    }

    private bool CheckCommandArgsLength(ScenarioCommand command)
    {
        if (command?.Args?.Length >= 2)
        {
            return true;
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
