using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public interface ISpriteSource
{
    UniTask<Sprite> LoadAsync(string path, CancellationToken ct);
}