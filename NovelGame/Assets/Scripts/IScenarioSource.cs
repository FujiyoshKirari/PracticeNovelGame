using Cysharp.Threading.Tasks;
using System.Threading;

public interface IScenarioSource
{
    // 「キャンセル可能で、最終的にstringを返す非同期処理」という約束
    UniTask<string> LoadAsync(CancellationToken ct);
}
