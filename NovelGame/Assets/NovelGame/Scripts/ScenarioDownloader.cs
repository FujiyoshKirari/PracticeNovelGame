using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class ScenarioDownloader : MonoBehaviour
{
    [SerializeField] string url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.txt";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadAsync(destroyCancellationToken).Forget();
    }

    private async UniTask LoadAsync(CancellationToken ct)
    {
        using var req = UnityWebRequest.Get(url);
        try
        {
            // リクエストを送る
            await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            // 失敗するとエラーに飛ぶため、ここに来た際には必ず成功している
            // テキストを表示する
            ParseLines(req.downloadHandler.text);
        }
        catch (UnityWebRequestException e)
        {
            // 失敗していればエラーを表示
            Debug.LogError(e.Error);
        }
    }

    private void ParseLines(string text)
    {
        var lines = text.Split('\n');
        var count = 1;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            if (trimmed.StartsWith('#')) continue;
            Debug.Log($"{count}: {trimmed}");
            count++;
        }
    }
}
