using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class WebSpriteSource : ISpriteSource
{
    private readonly string _baseUrl;
    private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

    public WebSpriteSource(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async UniTask<Sprite> LoadAsync(string path, CancellationToken ct)
    {
        if (_cache.TryGetValue(path, out var sprite))
        {
            return sprite;
        }
        else
        {
            var url = _baseUrl + path + ".png";
            using var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest().ToUniTask(cancellationToken: ct);
            Debug.Log($"download: {url}");
            var texture = DownloadHandlerTexture.GetContent(request);
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sp = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            _cache[path] = sp;
            return sp;
        }
    }
}
