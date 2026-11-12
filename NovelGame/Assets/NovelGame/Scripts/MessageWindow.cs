using Cysharp.Threading.Tasks;
using LitMotion;
using NaughtyAttributes;
using System.Threading;
using TMPro;
using UnityEngine;

public class MessageWindow : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text _nameText;
    [SerializeField, Required] private TMP_Text _messageText;
    [SerializeField] private float _duration = 0.1f;

    public async UniTask Show(string speaker, string body, CancellationToken ct)
    {
        _nameText.text = speaker;

        _messageText.text = body;
        _messageText.maxVisibleCharacters = 0;
        await LMotion.Create(0, body.Length, body.Length * _duration)
                     .Bind(count => _messageText.maxVisibleCharacters = count)
                     .AddTo(this)
                     .ToUniTask(ct);
    }
}
