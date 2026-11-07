using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Networking;

public class WebScenarioSource : IScenarioSource
{
    private readonly string _url;

    public WebScenarioSource(string url)
    {
        _url = url;
    }

    public async UniTask<string> LoadAsync(CancellationToken ct)
    {
        using var request = UnityWebRequest.Get(_url);
        // リクエストを送る
        await request.SendWebRequest().ToUniTask(cancellationToken: ct);
        // 失敗時は await で例外が投げられるので、ここに来るのは成功時だけ
        return request.downloadHandler.text;
    }
}
