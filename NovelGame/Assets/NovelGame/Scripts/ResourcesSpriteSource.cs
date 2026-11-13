using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ResourcesSpriteSource : ISpriteSource
{
    public async UniTask<Sprite> LoadAsync(string path, CancellationToken ct)
    {
        var sprite = await Resources.LoadAsync<Sprite>(path).WithCancellation(ct);
        if (sprite == null) throw new System.Exception($"{path} のアセットが存在しませんでした。");
        return sprite as Sprite;
    }
}
