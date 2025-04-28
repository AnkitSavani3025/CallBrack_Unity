using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public static class ImageDownloader
{
    public static void Download(string url, System.Action<Sprite> onComplete)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.timeout = 20;
        var operation = request.SendWebRequest();
        
        operation.completed += (op) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                onComplete.Invoke(null);

            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);



             
                //byte[] pngBytes = texture.EncodeToPNG();

                //texture.LoadImage();

                        Sprite downloadedSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                onComplete.Invoke(downloadedSprite);
            }
        };
    }
}