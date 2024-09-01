using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Help : MonoBehaviour
{
    public static Help Instance;

    public Font chinese;
    public Font english;

    Dictionary<string, AssetBundle> bundles;
    Dictionary<string, Color> colors;
    Dictionary<string, HouseEntityType> houseEntityTypes;
    List<string> works;

    public string streamingAssetsPath { get; private set; } = Application.streamingAssetsPath;
    public string customdatapath { get; private set; } = Application.streamingAssetsPath + "/Customdata";

    private void Awake()
    {
        Instance = this;
        works = new List<string>();
        bundles = new Dictionary<string, AssetBundle>(50);
        colors = new Dictionary<string, Color>(10);
        colors.Add("red", Color.red);
        colors.Add("grey", Color.grey);
        colors.Add("gray", Color.gray);
        colors.Add("yellow", Color.yellow);
        colors.Add("magenta", Color.magenta);
        colors.Add("cyan", Color.cyan);
        colors.Add("black", Color.black);
        colors.Add("white", Color.white);
        colors.Add("blue", Color.blue);
        colors.Add("green", Color.green);
        houseEntityTypes = new Dictionary<string, HouseEntityType>(15);
        houseEntityTypes.Add("wall", HouseEntityType.wall);
        houseEntityTypes.Add("floor", HouseEntityType.floor);
        houseEntityTypes.Add("door", HouseEntityType.door);
        houseEntityTypes.Add("stand", HouseEntityType.stand);
        houseEntityTypes.Add("sky", HouseEntityType.sky);
        houseEntityTypes.Add("arealight", HouseEntityType.arealight);
        houseEntityTypes.Add("anappliancesimal", HouseEntityType.appliances);
        houseEntityTypes.Add("flowLine", HouseEntityType.flowLine);
        houseEntityTypes.Add("animal", HouseEntityType.animal);
        houseEntityTypes.Add("weather", HouseEntityType.weather);
        houseEntityTypes.Add("eye", HouseEntityType.eye);
#if UNITY_WEBGL && !UNITY_EDITOR
        streamingAssetsPath = Shijie.domain + "/3dscene_local/StreamingAssets";
        customdatapath = Shijie.domain + "/3dscene_local/Customdata";
#endif
    }

    public string CombineAsherlinkdataInStreamingAssets(string root, string key)
    {
        string dk = key + "_" + root + ".asherlinkdata";
        var path = $"{streamingAssetsPath}/{root}/{dk}";
        return path;
    }

    public string CombineAsherlinkdataInCustomdata(string root, string key)
    {
        string dk = key + "_" + root + ".asherlinkdata";
        var path = $"{customdatapath}/{root}/{dk}";
        return path;
    }

    public string CombineStreamingAssetsPath(params string[] rpath)
    {
        var path = streamingAssetsPath;
        foreach (var item in rpath)
        {
            path += "/" + item;
        }
        return path;
    }

    public string CombineCustomdataPath(params string[] rpath)
    {
        var path = customdatapath;
        foreach (var item in rpath)
        {
            path += "/" + item;
        }
        return path;
    }

    public async Task<AssetBundle> LoadAsherlinkdataInSAssets(string root, string key)
    {
        string bundleID = key + "_" + root + ".asherlinkdata";
        if (bundles.ContainsKey(bundleID))
        {
            return bundles[bundleID];
        }
        else if (works.Contains(bundleID))
        {
            await new WaitUntilAsync(() => bundles.ContainsKey(bundleID));
            return bundles[bundleID];
        }
        var path = $"{streamingAssetsPath}/{root}/{bundleID}";
        System.Uri uri = new System.Uri(path);
        return await LoadAssetBundle(bundleID, uri);
    }

    public async Task<AssetBundle> LoadAsherlinkdataInSupplier(string root, string key, string supplier)
    {
        string bundleID = supplier + "_" + key + "_" + root + ".asherlinkdata";
        string dataname = key + "_" + root + ".asherlinkdata";
        if (bundles.ContainsKey(bundleID))
        {
            return bundles[bundleID];
        }
        else if (works.Contains(bundleID))
        {
            await new WaitUntilAsync(() => bundles.ContainsKey(bundleID));
            return bundles[bundleID];
        }
        var path = $"{customdatapath}/{supplier}/{root}/{dataname}";
        System.Uri uri = new System.Uri(path);
        return await LoadAssetBundle(bundleID, uri);
    }

    public async Task<AssetBundle> LoadAsherlinkdataInWWW(string bundleID, System.Uri uri)
    {
        if (bundles.ContainsKey(bundleID))
        {
            return bundles[bundleID];
        }
        else if (works.Contains(bundleID))
        {
            await new WaitUntilAsync(() => bundles.ContainsKey(bundleID));
            return bundles[bundleID];
        }
        return await LoadAssetBundle(bundleID, uri);
    }

    async Task<AssetBundle> LoadAssetBundle(string bundleID, System.Uri uri)
    {
        AssetBundle res = null;
        works.Add(bundleID);
        using (var request = UnityWebRequestAssetBundle.GetAssetBundle(uri))
        {
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                bundles.Add(bundleID, null);
                Debug.LogError("找不到ab:" + bundleID);
            }
            else
            {
                res = DownloadHandlerAssetBundle.GetContent(request);
                if (res)
                    bundles.Add(bundleID, res);
            }
        }
        works.Remove(bundleID);
        return res;
    }

    public async Task<Texture2D> LoadTextureLocal(bool streamingAssets, string path)
    {
        System.Uri uri;
        if (streamingAssets)
        {
            uri = new System.Uri(streamingAssetsPath + "/" + path);
        }
        else
        {
            uri = new System.Uri(customdatapath + "/" + path);
        }
        using (var request = UnityWebRequestTexture.GetTexture(uri))
        {
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("找不到地址:" + uri);
            }
            else
            {
                return DownloadHandlerTexture.GetContent(request);
            }
        }
        return null;
    }

    public async Task<Texture2D> LoadTextureWWW(string url)
    {
        System.Uri uri = new System.Uri(url);
        using (var request = UnityWebRequestTexture.GetTexture(uri))
        {
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("找不到地址:" + uri);
            }
            else
            {
                return DownloadHandlerTexture.GetContent(request);
            }
        }
        return null;
    }

    public async Task<string> LoadTextlocal(bool isstreamingAssets, string path, bool cache = true)
    {
        System.Uri uri;
        string res = null;
        if (isstreamingAssets)
        {
            uri = new System.Uri(streamingAssetsPath + "/" + path);
        }
        else
        {
            uri = new System.Uri(customdatapath + "/" + path);
        }
        using (var request = new UnityWebRequest(uri))
        {
            if (!cache)
            {
                request.SetRequestHeader("Cache-control", "no-cache");
                request.SetRequestHeader("Prama", "no-cache");
                request.SetRequestHeader("Expiree", "0");
            }
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            request.downloadHandler = dH;
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("找不到地址:" + path);
            }
            else
            {
                await new WaitUntilAsync(() => request.isDone);
                if (request.downloadHandler != null)
                {
                    res = request.downloadHandler.text;
                }
            }
        }
        return res;
    }

    public bool TryColor(string cstr, out Color va)
    {
        if (string.IsNullOrWhiteSpace(cstr))
        {

        }
        else if (cstr.StartsWith("#"))
        {
            return ColorUtility.TryParseHtmlString(cstr, out va);
        }
        else if (colors.ContainsKey(cstr))
        {
            va = colors[cstr];
            return true;
        }
        else
        {
            return RGBAStr2Color(cstr, out va);
        }
        va = Color.magenta;
        return false;

    }

    bool RGBAStr2Color(string str, out Color color)
    {
        var css = str.Split(',');
        byte va;
        if (css.Length == 4)
        {
            byte r, g, b, a;
            if (byte.TryParse(css[0], out va))
                r = va;
            else goto end;
            if (byte.TryParse(css[1], out va))
                g = va;
            else goto end;
            if (byte.TryParse(css[2], out va))
                b = va;
            else goto end;
            if (byte.TryParse(css[3], out va))
                a = va;
            else goto end;
            color = new Color32(r, g, b, a);
            return true;
        }
    end:
        color = Color.magenta;
        return false;
    }

    public HouseEntityType string2HouseEntityType(string str)
    {
        if (houseEntityTypes.ContainsKey(str))
        {
            return houseEntityTypes[str];
        }
        return HouseEntityType.none;
    }
}
