using TMPro;
using UnityEngine;

public class MessageWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _messageText;

    public void Show(string speaker, string body)
    {
        _nameText.text = speaker;
        _messageText.text = body;
    }
}
