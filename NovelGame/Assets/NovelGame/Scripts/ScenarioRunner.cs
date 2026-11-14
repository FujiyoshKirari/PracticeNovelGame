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
    private BackgroundView _bgView;

    void Start() => RunAsync(destroyCancellationToken).Forget();

    [Inject]
    private void Construct(
        IScenarioSource scenarioSource,
        MessageWindow messageWindow,
        CharacterStage characterStage,
        ISpriteSource spriteSource,
        BackgroundView bgView)
    {
        _scenarioSource = scenarioSource;
        _characterStage = characterStage;
        _spriteSource = spriteSource;
        _messageWindow = messageWindow;
        _bgView = bgView;
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

            foreach (var command in scenarioCommands)
            {
                await ExecuteCommand(command, ct);
            }

            _messageWindow.Show("", "--- おわり ---");
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
            case "hide":
                await HideImage(command, ct);
                break;
            case "bg":
                await SwitchBackground(command, ct);
                break;
            default:
                Debug.LogWarning($"未対応のコマンド {command.Name}({command.LineNumber}行目)");
                break;
        }
    }

    private void ShowText(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command, 2))
        {
            var speaker = command.Args[0];
            var body = command.Args[1];

            _messageWindow.Show(speaker, body);
        }
    }

    private async UniTask ShowImage(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command, 2))
        {
            var name = command.Args[0];
            var position = command.Args[1];
            try
            {
                var sprite = await _spriteSource.LoadAsync("Characters/" + name, ct);
                await _characterStage.Show(name, sprite, position, ct);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    private async UniTask HideImage(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command, 1))
        {
            var name = command.Args[0];
            await _characterStage.Hide(name, ct);
        }
    }

    private async UniTask SwitchBackground(ScenarioCommand command, CancellationToken ct)
    {
        if (CheckCommandArgsLength(command, 1))
        {
            var name = command.Args[0];
            var sprite = await _spriteSource.LoadAsync("Backgrounds/" + name, ct);
            _bgView.SetImage(sprite);
        }
    }

    private bool CheckCommandArgsLength(ScenarioCommand command, int needCount)
    {
        if (command?.Args?.Length >= needCount)
        {
            return true;
        }
        else if (command == null)
        {
            throw new Exception("ScenarioCommandがnullで渡されました。");
        }
        else
        {
            throw new Exception($"{command.LineNumber}行目のテキストの引数の数が足りません");
        }
    }
}
