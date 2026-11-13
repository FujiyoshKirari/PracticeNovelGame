using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStage : MonoBehaviour
{
    [SerializeField, Required] Image _left;
    [SerializeField, Required] Image _center;
    [SerializeField, Required] Image _right;

    [SerializeField] float _fadeDuration = 0.2f;

    private Dictionary<string, Image> _showCharacterDict = new Dictionary<string, Image>();

    private CancellationTokenSource _cts = new CancellationTokenSource();

    public async UniTask Show(string name, Sprite sprite, string position)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        Image image = null;
        switch (position)
        {
            case "left":
                image = _left;
                break;
            case "center":
                image = _center;
                break;
            case "right":
                image = _right;
                break;
        }
        image.sprite = sprite;
        _showCharacterDict.Add(name, image);

        try
        {
            await PlayFadeAnimation(image, 0f, 1f, _fadeDuration, _cts.Token);
        }
        finally
        {
            var color = image.color;
            color.a = 1f;
            image.color = color;
        }
    }

    public async UniTask Hide(string name)
    {
        if (_showCharacterDict.TryGetValue(name, out Image image))
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await PlayFadeAnimation(image, 1f, 0f, _fadeDuration, _cts.Token);
                _showCharacterDict.Remove(name);
            }
            finally
            {
                var color = image.color;
                color.a = 0f;
                image.color = color;
            }
        }
    }

    private async UniTask PlayFadeAnimation(Image target, float start, float end, float duration, CancellationToken ct)
    {
        await LMotion.Create(start, end, duration)
                     .BindToColorA(target)
                     .AddTo(this);
    }
}
