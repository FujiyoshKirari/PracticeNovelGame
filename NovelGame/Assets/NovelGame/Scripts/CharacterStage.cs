using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStage : MonoBehaviour
{
    [SerializeField, Required] Image _left;
    [SerializeField, Required] Image _center;
    [SerializeField, Required] Image _right;

    [SerializeField] float _fadeDuration = 0.2f;

    private Dictionary<string, Image> _showCharacterDict = new Dictionary<string, Image>();

    public async UniTask Show(string name, Sprite sprite, string position, CancellationToken ct)
    {
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
            default:
                throw new System.Exception($"{position} は設定されてません");
        }

        var splitName = name.Split('_');

        if (_showCharacterDict.TryGetValue(splitName[0], out var img))
        {
            if (image != img)
            {
                await Hide(splitName[0], ct);
            }
        }

        image.sprite = sprite;
        _showCharacterDict[splitName[0]] = image;

        if (image == img)
        {
            return;
        }

        try
        {
            await PlayFadeAnimation(image, 0f, 1f, _fadeDuration, ct);
        }
        finally
        {
            var color = image.color;
            color.a = 1f;
            image.color = color;
        }
    }

    public async UniTask Hide(string name, CancellationToken ct)
    {
        var splitName = name.Split('_');
        if (_showCharacterDict.TryGetValue(splitName[0], out Image image))
        {
            try
            {
                await PlayFadeAnimation(image, 1f, 0f, _fadeDuration, ct);
                _showCharacterDict.Remove(splitName[0]);
            }
            finally
            {
                var color = image.color;
                color.a = 0f;
                image.color = color;
            }
        }
        else
        {
            Debug.LogWarning($"{name} は存在してませんでした。");
        }
    }

    private async UniTask PlayFadeAnimation(Image target, float start, float end, float duration, CancellationToken ct)
    {
        await LMotion.Create(start, end, duration)
                     .BindToColorA(target)
                     .AddTo(this)
                     .ToUniTask(ct);
    }
}
