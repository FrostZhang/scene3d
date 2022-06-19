using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddModelPanel : MonoBehaviour
{
    public Button bj;
    public Button add;
    public InputField inputField;

    public RawImage preiew;
    public CanvasGroup cgroup;
    public Text err;
    string verifyUrl;
    void Start()
    {
        bj.onClick.AddListener(() =>
        {
            var rc = bj.transform.GetChild(0) as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rc, Input.mousePosition))
            {
                return;
            }
            Destroy(gameObject);
        });
        inputField.onEndEdit.AddListener((x) =>
        {
            err.text = string.Empty;
            Ans(x);
        });
        add.onClick.AddListener(() =>
        {

        });
    }

    public async void Ans(string url)
    {
        verifyUrl = null;
        if (url.StartsWith("http") && url.EndsWith(".asherlinkdata"))
        {
            url = url.Replace("\\", "/");
            var filename = url.Substring(url.LastIndexOf("/") + 1);
            var filenameWE = filename.Remove(filename.IndexOf("."));
            if (filenameWE.Length < 3)
            {
                err.text = "File naming is not standardized.the format should be like bed_stand,fan_appliances";
                return;
            }
            var nandt = filenameWE.Split('_');
            if (nandt.Length != 2)
            {
                return;
            }
            cgroup.interactable = false;
            err.text = "Loading ...";
            var bundle = await Help.Instance.LoadAsherlinkdataInWWW(url, new System.Uri(url));
            if (!gameObject)
                return;
            if (bundle != null)
            {
                var imagepath = url.Remove(url.LastIndexOf("/") + 1) + nandt[0] + ".png";
                var bs = bundle.LoadAllAssets();
                if (bs != null && bs.Length > 0)
                {
                    if (verify(nandt[1], bs[0]))
                    {
                        verifyUrl = url;
                        var te = await Help.Instance.LoadTextureWWW(imagepath);
                        if (!gameObject)
                            return;
                        preiew.texture = te;
                        err.text = "Load success";
                        cgroup.interactable = true;
                        return;
                    }
                }
            }
            err.text = "Load failed";
        }
        else if (url.StartsWith("/"))
        {
            url = Help.Instance.CombineCustomdataPath(url.Remove(0, 1));
            url = url.Replace("\\", "/");
            var filename = url.Substring(url.LastIndexOf("/"));
            var filenameWE = filename.Remove(url.IndexOf("."));
            if (filenameWE.Length < 3)
            {
                err.text = "File naming is not standardized.the format should be like bed_stand,fan_appliances";
                return;
            }
            var nandt = filenameWE.Split('_');
            if (nandt.Length != 2)
            {
                return;
            }
            cgroup.interactable = false;
            var bundle = await Help.Instance.LoadAsherlinkdataInWWW(url, new System.Uri(url));
            if (!gameObject)
                return;
            if (bundle != null)
            {
                var imagepath = url.Remove(url.LastIndexOf("/") + 1) + nandt[0] + ".png";
                var bs = bundle.LoadAllAssets();
                if (bs != null && bs.Length > 0)
                {
                    if (verify(nandt[1], bs[0]))
                    {
                        verifyUrl = url;
                        var te = await Help.Instance.LoadTextureWWW(imagepath);
                        if (!gameObject)
                            return;
                        preiew.texture = te;
                        err.text = "Load success";
                        cgroup.interactable = true;
                        return;
                    }
                }
            }
            err.text = "Load failed";
        }
        cgroup.interactable = true;
    }

    private bool verify(string ty, Object wwwR)
    {
        var t = Help.Instance.string2HouseEntityType(ty);

        switch (t)
        {
            case HouseEntityType.wall:
            case HouseEntityType.floor:
            case HouseEntityType.stand:
                if (wwwR is GameObject)
                    return true;
                break;
            case HouseEntityType.door:
            case HouseEntityType.appliances:
                if (wwwR is GameObject)
                {
                    var g = wwwR as GameObject;
                    if (g.GetComponent<HassEntity>())
                    {
                        return true;
                    }
                }
                break;
            case HouseEntityType.eye:
                if (wwwR is GameObject)
                {
                    var g = wwwR as GameObject;
                    if (g.GetComponent<Eye>())
                    {
                        return true;
                    }
                }
                break;
            case HouseEntityType.sky:
                if (wwwR is Material)
                    return true;
                break;
            case HouseEntityType.arealight:
                break;
            case HouseEntityType.flowLine:
                if (wwwR is GameObject)
                {
                    var g = wwwR as GameObject;
                    if (g.GetComponent<LineEntity>())
                    {
                        return true;
                    }
                }
                break;
            case HouseEntityType.animal:
                if (wwwR is GameObject)
                    return true;
                break;
            case HouseEntityType.weather:
                if (wwwR is GameObject)
                {
                    var g = wwwR as GameObject;
                    if (g.GetComponent<ParticleSystem>())
                    {
                        return true;
                    }
                }
                break;
            case HouseEntityType.none:
                return false;
            default:
                break;
        }
        return false;
    }
}
