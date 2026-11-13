using Cysharp.Threading.Tasks;
using LitMotion;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class MessageWindow : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text _nameText;
    [SerializeField, Required] private TMP_Text _messageText;
    [SerializeField] private float _duration = 0.1f;
    private MotionHandle _handle;

    public bool IsTyping => _handle.IsActive();

    public void Show(string speaker, string body)
    {
        _nameText.text = speaker;

        _messageText.text = body;
        _messageText.maxVisibleCharacters = 0;
        _handle = LMotion.Create(0, body.Length, body.Length * _duration)
                         .Bind(count => _messageText.maxVisibleCharacters = count)
                         .AddTo(this);
    }

    public void Complete()
    {
        if (_handle.IsActive() == false) return;
        _handle.Complete();
    }
}
