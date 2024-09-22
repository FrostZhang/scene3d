using UnityEngine;
using UnityEngine.UI;

public class EditoUIPanel : MonoBehaviour, IPanel
{
    public GameObject caidan;
    public Toggle xianshi;
    public InputField pyx;
    public InputField pyy;
    public InputField chicunx;
    public InputField chicuny;
    public InputField bgurl;
    public Button bgbd;
    public InputField qianzhui;
    public Button bd, bdx, close, editT, editYS, editZS, editYX, editZX, save, exit;
    private HousePanel hup;

    public Transform Tr { get; set; }
    LitJson.JsonData uiconfig;
    public void OnDestroy()
    {

    }

    public void Start()
    {
        caidan.SetActive(false);
        close.onClick.AddListener(() => { caidan.SetActive(false); });
        save.onClick.AddListener(Save);
        exit.onClick.AddListener(Exit);
        hup = Surface.surface.LoadPanel<HousePanel>();
        uiconfig = hup.UiConfig;
        hup.title.gameObject.SetActive(true);
        hup.youshang.gameObject.SetActive(true);
        hup.youxia.gameObject.SetActive(true);
        hup.zuoshang.gameObject.SetActive(true);
        hup.zuoxia.gameObject.SetActive(true);

        if (uiconfig == null)
        {
            uiconfig = new LitJson.JsonData();

            LitJson.JsonData d = SetDefaultJson(hup.title);
            uiconfig["title"] = d;

            d = SetDefaultJson(hup.youshang);
            uiconfig["youshang"] = d;

            d = SetDefaultJson(hup.zuoshang);
            uiconfig["zuoshang"] = d;

            d = SetDefaultJson(hup.youxia);
            uiconfig["youxia"] = d;

            d = SetDefaultJson(hup.zuoxia);
            uiconfig["zuoxia"] = d;
        }

        editT.onClick.AddListener(() =>
        {
            EditTarget(hup.title, "title");
        });

        editYS.onClick.AddListener(() =>
        {
            EditTarget(hup.youshang, "youshang");
        });

        editYX.onClick.AddListener(() =>
        {
            EditTarget(hup.youxia, "youxia");
        });

        editZS.onClick.AddListener(() =>
        {
            EditTarget(hup.zuoshang, "zuoshang");
        });

        editZX.onClick.AddListener(() =>
        {
            EditTarget(hup.zuoxia, "zuoxia");
        });
    }

    void EditTarget(RectTransform target, string key)
    {
        caidan.SetActive(true);
        var config = uiconfig[key];
        xianshi.onValueChanged.RemoveAllListeners();
        xianshi.isOn = (int)config["s"] == 1;
        xianshi.onValueChanged.AddListener((x) =>
        {
            config["s"] = x ? 1 : 0;
        });

        pyx.onEndEdit.RemoveAllListeners();
        pyy.onEndEdit.RemoveAllListeners();
        pyx.text = target.anchoredPosition.x.ToString("f2");
        pyy.text = target.anchoredPosition.y.ToString("f2");
        pyx.onEndEdit.AddListener((x) =>
        {
            if (string.IsNullOrEmpty(x))
            {
                pyx.text = "0";
                return;
            }
            var p = target.anchoredPosition;
            p.x = float.Parse(x);
            target.anchoredPosition = p;

            var px = Mathf.RoundToInt(target.anchoredPosition.x);
            var py = Mathf.RoundToInt(target.anchoredPosition.y);
            config["p"] = px + "," + py;
        });
        pyy.onEndEdit.AddListener((x) =>
        {
            if (string.IsNullOrEmpty(x))
            {
                pyy.text = "0";
                return;
            }
            var p = target.anchoredPosition;
            p.y = float.Parse(x);
            target.anchoredPosition = p;

            var px = Mathf.RoundToInt(target.anchoredPosition.x);
            var py = Mathf.RoundToInt(target.anchoredPosition.y);
            config["p"] = px + "," + py;
        });

        chicunx.onEndEdit.RemoveAllListeners();
        chicuny.onEndEdit.RemoveAllListeners();
        chicunx.text = target.sizeDelta.x.ToString("f2");
        chicuny.text = target.sizeDelta.y.ToString("f2");
        chicunx.onEndEdit.AddListener((x) =>
        {
            if (string.IsNullOrEmpty(x))
            {
                chicunx.text = "0";
                return;
            }
            var p = target.sizeDelta;
            p.x = float.Parse(x);
            target.sizeDelta = p;

            var px = Mathf.RoundToInt(target.sizeDelta.x);
            var py = Mathf.RoundToInt(target.sizeDelta.y);
            config["w"] = px + "," + py;
        });
        chicuny.onEndEdit.AddListener((x) =>
        {
            if (string.IsNullOrEmpty(x))
            {
                chicuny.text = "0";
                return;
            }
            var p = target.sizeDelta;
            p.y = float.Parse(x);
            target.sizeDelta = p;

            var px = Mathf.RoundToInt(target.sizeDelta.x);
            var py = Mathf.RoundToInt(target.sizeDelta.y);
            config["w"] = px + "," + py;
        });

        bgbd.onClick.RemoveAllListeners();
        bgbd.GetComponentInChildren<Text>().text = config["bgid"].ToString();
        bgbd.onClick.AddListener(() =>
        {
            var el = Surface.surface.LoadPanel<EntityListPanel>();
            el.ChooseEntity((x) =>
            {
                bgbd.GetComponentInChildren<Text>().text = x;
                config["bgid"] = x;
            });
        });

        bgurl.onEndEdit.RemoveAllListeners();
        bgurl.text = config["bgurl"].ToString();
        bgurl.onEndEdit.AddListener((x) =>
        {
            if (x == null) x = string.Empty;
            config["bgurl"] = x;
            bgurl.text = x;
            var id = config["bgid"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                hup.LoadUrlTexture(target, x);
            }
        });

        qianzhui.onValueChanged.RemoveAllListeners();
        qianzhui.text = config["tqz"].ToString();
        qianzhui.onEndEdit.AddListener((x) =>
        {
            if (x == null) x = string.Empty;
            target.GetComponentInChildren<Text>().text = x;
            config["tqz"] = x;
        });
    }

    private LitJson.JsonData SetDefaultJson(RectTransform target)
    {
        var d = new LitJson.JsonData();
        d["s"] = 0;
        var px = Mathf.RoundToInt(target.anchoredPosition.x);
        var py = Mathf.RoundToInt(target.anchoredPosition.y);
        d["p"] = px + "," + py;
        px = Mathf.RoundToInt(target.sizeDelta.x);
        py = Mathf.RoundToInt(target.sizeDelta.y);
        d["w"] = px + "," + py;
        d["bgid"] = "";
        d["bgurl"] = "";
        d["tqz"] = "";
        return d;
    }

    void Save()
    {
        if (uiconfig == null) return;
        var hs = uiconfig.ToJson();
        Shijie.ConfigUI(hs);
        EmergencyPanel.instance.Show("已发送保存指令，建议刷新网页验证保存效果");

#if UNITY_EDITOR
        var path = Application.streamingAssetsPath + "/Customdata/uiconfig.json";
        var dir = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        System.IO.File.WriteAllText(path, hs);
#endif
    }

    void Exit()
    {
        hup.title.gameObject.SetActive((int)uiconfig["title"]["s"] == 1);
        hup.youshang.gameObject.SetActive((int)uiconfig["youshang"]["s"] == 1);
        hup.youxia.gameObject.SetActive((int)uiconfig["youxia"]["s"] == 1);
        hup.zuoshang.gameObject.SetActive((int)uiconfig["zuoshang"]["s"] == 1);
        hup.zuoxia.gameObject.SetActive((int)uiconfig["zuoxia"]["s"] == 1);
        Surface.surface.RemovePanel<EditoUIPanel>();
    }

    void OnEnable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = true;
#endif
    }

    void OnDisable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }
}
