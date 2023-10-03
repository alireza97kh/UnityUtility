using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DobeilRemoteAsset : MonoSingleton<DobeilRemoteAsset>
{
    public string BaseUrl { get; set; }
    /// <summary>
    /// Download a TextAsset. if already downloaded it loaded from writable path on storage
    /// </summary>
    /// <param name="url">url address</param>
    /// <param name="loadedEvent">Event called ween downloading content is finished or loaded from storage.</param>
    /// <param name="forceDownload">Ignore cache path and force to download text from url</param>
    public void DownloadText(string url, Action<TextAsset, string, bool> loadedEvent, bool forceDownload = false)
    {
        StartCoroutine(DownloadTextCoroutine(url, loadedEvent, forceDownload));
    }

    IEnumerator<YieldInstruction> DownloadTextCoroutine(string url, Action<TextAsset, string, bool> loadedEvent, bool forceDownload)
    {
        url = CheckEditUrl(url);

        if (!MakeCacheUrlPath(url, out string cachedFilePath))
            yield break;

        if (!forceDownload && File.Exists(cachedFilePath))
        {
            var data = File.ReadAllText(cachedFilePath);
            var texAsset = new TextAsset(data);
            loadedEvent?.Invoke(texAsset, "", true);
            yield break;
        }

        var webReq = new UnityWebRequest(url);
        webReq.timeout = 15;
        webReq.downloadHandler = new DownloadHandlerBuffer();
        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError)
        {
            DobeilLogger.LogError($"Error on downloading text buffer - <color=red>{webReq.error} from url:{url}</color>");
            loadedEvent?.Invoke(null, webReq.error, false);
        }
        else
        {
            var handler = webReq.downloadHandler;
            var texAsset = new TextAsset(handler.text);
            loadedEvent?.Invoke(texAsset, "", false);

            File.WriteAllText(cachedFilePath, handler.text);
        }
    }



    /// <summary>
    /// Download a file.
    /// </summary>
    /// <param name="url">url address</param>
    /// <param name="loadedEvent">Event called ween downloading content is finished or loaded from storage.</param>
    public void DownloadFile(string url, Action<string, string, bool> loadedEvent)
    {
        StartCoroutine(DownloadFileCoroutine(url, loadedEvent));
    }

    IEnumerator<YieldInstruction> DownloadFileCoroutine(string url, Action<string, string, bool> loadedEvent)
    {
        url = CheckEditUrl(url);

        if (!MakeCacheUrlPath(url, out string cachedFilePath))
            yield break;

        if (File.Exists(cachedFilePath))
        {
            loadedEvent?.Invoke(cachedFilePath, "", true);
            yield break;
        }

        var webReq = new UnityWebRequest(url);
        webReq.downloadHandler = new DownloadHandlerFile(cachedFilePath);
        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError)
        {
            DobeilLogger.LogError($"Error on downloading file - <color=red>{webReq.error} from url:{url}</color>");
            loadedEvent?.Invoke(null, webReq.error, false);
        }
        else
        {
            loadedEvent?.Invoke(cachedFilePath, "", false);
            File.WriteAllBytes(cachedFilePath, webReq.downloadHandler.data);
        }
    }


    /// <summary>
    /// Download a Texture2D. if already downloaded it loaded from writable path on storage
    /// </summary>
    /// <param name="url">url address</param>
    /// <param name="loadedEvent">Event called ween downloading content is finished or loaded from storage.</param>
    public void DownloadTexture(string url, Action<Texture2D, string, bool> loadedEvent)
    {
        StartCoroutine(DownloadTextureCoroutine(url, loadedEvent));
    }

    IEnumerator<YieldInstruction> DownloadTextureCoroutine(string url, Action<Texture2D, string, bool> loadedEvent)
    {
        url = CheckEditUrl(url);

        if (!MakeCacheUrlPath(url, out string cachedFilePath))
            yield break;

        if (File.Exists(cachedFilePath))
        {
            var data = File.ReadAllBytes(cachedFilePath);
            var tex2D = new Texture2D(1, 1);
            tex2D.LoadImage(data);
            loadedEvent?.Invoke(tex2D, "", true);
            yield break;
        }

        using (var webReq = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webReq.SendWebRequest();

            if (webReq.isNetworkError || webReq.isHttpError)
            {
                // DobeilLogger.LogError($"Error on downloading texture - <color=red>{webReq.error} from url:{url}</color>");
                loadedEvent?.Invoke(null, webReq.error, false);
            }
            else
            {
                Texture2D tex2D = DownloadHandlerTexture.GetContent(webReq);

                loadedEvent?.Invoke(tex2D, "", false);
                File.WriteAllBytes(cachedFilePath, webReq.downloadHandler.data);
            }
        }
    }


    /// <summary>
    /// Download an audio clip. if already downloaded it loaded from writable path on storage
    /// </summary>
    /// <param name="url">url address</param>
    /// <param name="type">AudioTypes</param>
    /// <param name="loadedEvent">Event called ween downloading content is finished or loaded from storage.</param>
    public void DownloadAudio(string url, AudioType type, Action<AudioClip, string, bool> loadedEvent)
    {
        StartCoroutine(DownloadAudioCoroutine(url, type, loadedEvent));
    }

    IEnumerator<YieldInstruction> DownloadAudioCoroutine(string url, AudioType type, Action<AudioClip, string, bool> loadedEvent)
    {
        url = CheckEditUrl(url);

        if (!MakeCacheUrlPath(url, out string cachedFilePath))
            yield break;

        bool cached = false;
        if (File.Exists(cachedFilePath))
        {
            cached = true;
        }

        var webReq = new UnityWebRequest(url);
        var handler = new DownloadHandlerAudioClip(url, type);
        webReq.downloadHandler = handler;
        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError)
        {
            DobeilLogger.LogError($"Error on downloading audio - <color=red>{webReq.error} from url:{url}</color>");
            loadedEvent?.Invoke(null, webReq.error, false);
        }
        else
        {

            var clip = DownloadHandlerAudioClip.GetContent(webReq);
            loadedEvent?.Invoke(clip, "", cached);

            if (!cached)
                File.WriteAllBytes(cachedFilePath, handler.data);
        }
    }


    /// <summary>
    /// Download an asset bundle and cache it. if already downloaded it loaded from writable path on storage
    /// </summary>
    /// <param name="bundleUrl"></param>
    /// <param name="loadedEvent"></param>
    public void DownloadAssetBundle(string bundleUrl, Action<AssetBundle, string, bool> loadedEvent)
    {
        StartCoroutine(DownloadAssetBundleCoroutine(bundleUrl, loadedEvent));
    }

    IEnumerator<YieldInstruction> DownloadAssetBundleCoroutine(string url, Action<AssetBundle, string, bool> loadedEvent)
    {
        url = CheckEditUrl(url);

        string bundleName = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
        string rootBundleUrl = url.Remove(url.LastIndexOf("/", StringComparison.Ordinal));
        string rootBundleName = rootBundleUrl.Substring(rootBundleUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);
        string rootBundleUrlName = $"{rootBundleUrl}/{rootBundleName}";

        var loadedBundlesList = AssetBundle.GetAllLoadedAssetBundles().ToList();

        AssetBundle bundle = loadedBundlesList.Find(b => b.name == bundleName);

        if (bundle != null)
        {
            loadedEvent?.Invoke(bundle, "", true);
            yield break;
        }

        AssetBundle rootBundle = loadedBundlesList.Find(b => b.name == rootBundleName);

        if (rootBundle == null)
        {
            using (var webReq = UnityWebRequestAssetBundle.GetAssetBundle(rootBundleUrlName))
            {
                yield return webReq.SendWebRequest();

                if (webReq.isNetworkError || webReq.isHttpError)
                {
                    DobeilLogger.LogError($"Error on downloading root asset bundle - <color=red>{webReq.error} from url:{url}</color>");
                    loadedEvent?.Invoke(null, webReq.error, false);
                }
                else
                {
                    rootBundle = DownloadHandlerAssetBundle.GetContent(webReq);
                    rootBundle.name = rootBundleName;
                }
            }
        }

        if (rootBundle == null)
        {
            DobeilLogger.LogError($"Root bundle is null for bundle name {bundleName}");
            yield break;
        }

        var bundleManifest = rootBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hash128 = bundleManifest.GetAssetBundleHash(bundleName);

        using (var webReq = UnityWebRequestAssetBundle.GetAssetBundle(url, hash128))
        {
            yield return webReq.SendWebRequest();

            if (webReq.isNetworkError || webReq.isHttpError)
            {
                DobeilLogger.LogError($"Error on downloading asset bundle - <color=red>{webReq.error} from url:{url}</color>");
                loadedEvent?.Invoke(null, webReq.error, false);
            }
            else
            {
                bundle = DownloadHandlerAssetBundle.GetContent(webReq);
                loadedEvent?.Invoke(bundle, "", false);
            }
        }

        rootBundle.Unload(false);
    }



    bool MakeCacheUrlPath(string url, out string cachedFilePath)
    {
        cachedFilePath = string.Empty;

        int soloSlashIndex = GetFirstSoloSlashIndex(url);
        if (soloSlashIndex == -1)
        {
            DobeilLogger.Log($"RemoteAssets - Invalid url. {url}");
            return false;
        }

        string urlPath = url;
        TryToCorrectCachePath(url, ref urlPath);
        string fileCdnPath = urlPath.Substring(soloSlashIndex);
        string dirCdnPath = fileCdnPath.Remove(fileCdnPath.LastIndexOf('/'));

        string dirPath = $"{Application.persistentDataPath}/{dirCdnPath}";
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        cachedFilePath = $"{Application.persistentDataPath}/{fileCdnPath}";

        //TODO
        if (cachedFilePath.Contains("profilepic") && cachedFilePath.EndsWith("/"))
            cachedFilePath += "file.jpg";

        return true;
    }

    int GetFirstSoloSlashIndex(string url)
    {
        const char slashChar = '/';
        int indexOfChar = -1;
        for (int i = 0; i < url.Length; i++)
        {
            if (i == 0 || i == url.Length - 1)
                continue;

            var prevChar = url[i - 1];
            var nextChar = url[i + 1];
            var curChar = url[i];

            if (curChar == slashChar && prevChar != slashChar && nextChar != slashChar)
            {
                indexOfChar = i + 1;
                break;
            }
        }
        return indexOfChar;
    }


    /// <summary>
    /// If url has query param like FaceBook profile image, then create a folder named url hash before file name in cachePath. 
    /// </summary>
    /// <param name="url"> download url </param>
    /// <param name="cachePath"> url path on storage </param>
    void TryToCorrectCachePath(string url, ref string cachePath)
    {
        if (!url.Contains('?'))
            return;

        try
        {
            var strsArr = url.Split('?');
            string mainUrl = strsArr[0];
            string queryParam = strsArr[1];

            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(url));
                var sb = new StringBuilder();
                foreach (var h in hashBytes)
                    sb.Append(h.ToString("X"));

                string hexadecimalHash = sb.ToString(); //folder name
                cachePath = mainUrl.Insert(mainUrl.LastIndexOf('/'), $"/{hexadecimalHash}");
            }
        }
        catch (Exception ex)
        {
            print($"<color=red> {ex.Message} </color>");
            throw;
        }
    }

    string CheckEditUrl(string url)
    {
        string editedUrl = url;
        if (!editedUrl.ToLower().StartsWith("http"))
            editedUrl = $"{BaseUrl}{editedUrl}";

        return editedUrl;
    }
}
