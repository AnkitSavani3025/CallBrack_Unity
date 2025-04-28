using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class CallBreak_IMGLoader : MonoBehaviour
{
    #region Variables
    [SerializeField] Image icon;
    [SerializeField] Image preloader;
    private CancellationTokenSource cancellationTokenSource;
    bool isSeatEmpty;

    string jpgURl = "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1678340520396.jpg";
    string pngURL = "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1678340534007.png";
    string temp = "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1678702843189.jpg";

    #endregion

    #region Unity callbacks
    private void Awake()
    {
        icon = GetComponent<Image>();

        // SetUserInformation(pngURL);
        // LoadIMG("https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1678700526574.jpg");
        // StartCoroutine(Load(jpgURl));
        // Load(jpgURl);
        // GetSprite(icon, "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1671447597960.jpg");
        // DownloadImg(icon, "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1668590057158.png");
        // LoadIMG("https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1671447597960.jpg", true);
    }
    #endregion

    #region Load Profile Image by url
    internal void LoadIMG(string url, bool offline = false)
    {
        Debug.Log(Time.time + " Image Loader ||  LoadIMG  TO URL => " + url);
        if (this.enabled)
        {
            preloader.gameObject.SetActive(true);
            isSeatEmpty = false;
            Sprite myoldSprite = ThreePlusGamesCallBreak.CallBreak_GS.Inst.downloadedSpriteList.Find(x => x.name == url);
            if (myoldSprite != null)
            {
                Debug.Log(" Image Found in Old Stored data Set Old Image ");
                icon.sprite = myoldSprite;
                preloader.gameObject.SetActive(false);
                return;
            }
            SetUserInformation(url);
            //ImageDownloader.Download(url, (sprite) =>
            //{
            //    Debug.Log(Time.time + " ImageDownloader || Download || Image Downloaded ");

            //    if (sprite != null && !isSeatEmpty)
            //    {
            //        icon.sprite = sprite;
            //        sprite.name = url;
            //        ThreePlusGamesCallBreak.CallBreak_GS.Inst.downloadedSpriteList.Add(sprite);
            //    }
            //    preloader.gameObject.SetActive(false);
            //});
        }
    }

    //async void DownloadImg(Image profilePic, string URL)
    //{
    //    Debug.Log(" <<<<<< Download Image >>>>>> " + URL);
    //    if (!string.IsNullOrEmpty(URL))
    //    {
    //        try
    //        {
    //            cancellationTokenSource = new CancellationTokenSource();

    //            Texture2D _texture = await GetRemoteTexture(URL, cancellationTokenSource.Token);
    //            Rect rect = new Rect(0, 0, _texture.width, _texture.height);
    //            Sprite downloadedSprite = Sprite.Create(_texture, rect, new Vector2(0.5f, 0.5f));
    //            profilePic.sprite = downloadedSprite;
    //            Debug.Log(" Profile Pic Downloaded Succed Set As Profile Pic ");
    //            downloadedSprite.name = URL;
    //            ThreePlusGamesCallBreak.CallBreak_GS.Inst.downloadedSpriteList.Add(downloadedSprite);
    //            preloader.gameObject.SetActive(false);
    //        }
    //        catch (TaskCanceledException)
    //        {
    //            Debug.Log("Task was canceled.");
    //        }
    //    }
    //    else
    //    {
    //        preloader.gameObject.SetActive(false);
    //        Debug.Log(" Profile Pic Download get faild  retry ");

    //    }
    //}





    //async Task<Texture2D> GetRemoteTexture(string url, CancellationToken cancellationToken)
    //{
    //    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    //    {
    //        var asyncOp = www.SendWebRequest();
    //        // await until it's done: 
    //        while (asyncOp.isDone == false)
    //            await Task.Delay(1000 / 30, cancellationToken);

    //        // read results:
    //        if (www.isNetworkError || www.isHttpError)
    //        {
    //            Debug.Log("Error Downloading Image > " + www.error + " URL " + www.url);
    //            return ThreePlusGamesCallBreak.CallBreak_UIManager.Inst.emptyProfileImage.texture;
    //        }
    //        else
    //        {
    //            return DownloadHandlerTexture.GetContent(www);
    //        }
    //    }

    //}



    //IEnumerator Load(string url)
    //{
    //    preloader.gameObject.SetActive(true);
    //    int count = 0;
    //    if (url != "")
    //    {
    //    MK:
    //        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
    //        //request.timeout = 4;
    //        yield return request.SendWebRequest();
    //        if (request.error != null)
    //        {
    //            count++;
    //            if (count > 1)
    //            {
    //                preloader.gameObject.SetActive(false);
    //                Debug.Log(" Profile Pic Download get faild  retry ");
    //                yield break;
    //            }
    //            else
    //                goto MK;
    //        }
    //        Texture2D myTexture = DownloadHandlerTexture.GetContent(request);
    //        Sprite downloadedSprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
    //        icon.sprite = downloadedSprite;
    //        Debug.Log(" Profile Pic Downloaded Succed Set As Profile Pic ");
    //        icon.gameObject.transform.localScale = Vector3.one;
    //        downloadedSprite.name = url;
    //        ThreePlusGamesCallBreak.CallBreak_GS.Inst.downloadedSpriteList.Add(downloadedSprite);
    //        preloader.gameObject.SetActive(false);

    //    }
    //    else
    //    {
    //        preloader.gameObject.SetActive(false);
    //    }
    //}
    //public static async void GetSprite(Image profileImage, string url)
    //{
    //    Debug.Log(" Image Loader ||  LoadIMG  TO URL => " + url);

    //    using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
    //    {
    //        request.SendWebRequest();
    //        await Task.Delay(1000 * 5);//30 hertz
    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log($"Error in downloading image : {request.responseCode} {request.GetHashCode()}");
    //            //return null;
    //        }
    //        else
    //        {
    //            Debug.LogError($"Error in downloading image ");

    //            Texture2D myTexture = DownloadHandlerTexture.GetContent(request);
    //            Rect rect = new Rect(0, 0, myTexture.width, myTexture.height);
    //            Sprite downloadedSprite = Sprite.Create(myTexture, rect, new Vector2(0.5f, 0.5f));
    //            profileImage.sprite = downloadedSprite;
    //            //return downloadedSprite;
    //        }
    //    }
    //}

    #endregion

    public void SetUserInformation(string _playerProfilePic)
    {
        Debug.Log(Time.time + " SetUserData || SetUserInformation || UserProfile Load Start ");
        try
        {
            StartCoroutine(GetTexture(icon, _playerProfilePic));
        }
        catch (System.Exception ex) { Debug.LogError("fgfds" + ex); }
    }
    UnityWebRequest www;
    int tryCounter = 0;
    IEnumerator GetTexture(Image _image, string _url)
    {
        if (_url == null)
        {
            Debug.LogError("SetUserData ||GetTexture||  URL NOT FOUND ");
            preloader.gameObject.SetActive(false);
        }
        else
        {
        tryAgain:
            www = UnityWebRequestTexture.GetTexture(_url);
            www.timeout = 20;
            yield return www.SendWebRequest();
            tryCounter++;
            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log(" UnityWebRequest Error " + www.error + " ||  TO URL || " + _url);
            else
            {
                Debug.Log(Time.time + " SetUserData || SetUserInformation || UserProfile Load Done");
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite mySprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                _image.sprite = mySprite;
                mySprite.name = _url;
                ThreePlusGamesCallBreak.CallBreak_GS.Inst.downloadedSpriteList.Add(mySprite);
            }

            preloader.gameObject.SetActive(false);

            if (_image.sprite.name != _url && tryCounter == 1 && !this.isSeatEmpty)
            {
                goto tryAgain;
            }
        }
    }








    #region Unload profile image
    internal void UnLoadIMG()
    {
        preloader.gameObject.SetActive(false);
        if (this.www != null)
            this.www.Abort();

        this.isSeatEmpty = true;
        icon.sprite = ThreePlusGamesCallBreak.CallBreak_UIManager.Inst.emptyProfileImage;
    }
    #endregion
}
