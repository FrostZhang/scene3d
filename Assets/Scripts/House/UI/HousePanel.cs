using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HousePanel : MonoBehaviour, IPanel
{
    public StatesBtn changeCamera, roOrMove;
    public Button resetCamera, setting;

    public RectTransform title, youshang, zuoshang, youxia, zuoxia;
    public JsonData UiConfig { get; set; }

    public Transform Tr { get; set; }

    public void Start()
    {
        changeCamera.onValueChanged = (x) =>
        {
            ChangCamMode(x);
        };

        resetCamera.onClick.AddListener(() =>
        {
            ResetView();
        });

        roOrMove.onValueChanged = (x) =>
        {
            if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.first)
            {
                roOrMove.SetIsOnWithoutNotify(0);
                return;
            }
            CameraControllerForUnity.Instance.roOrMove = x == 0;
        };
        CameraControllerForUnity.Instance.roOrMove = roOrMove.State == 0;

        setting.onClick.AddListener(() => Surface.surface.LoadPanel<SettingPanel>());

        ClearGC().WrapErrors();

        title.gameObject.SetActive(false);
        youshang.gameObject.SetActive(false);
        youxia.gameObject.SetActive(false);
        zuoshang.gameObject.SetActive(false);
        zuoxia.gameObject.SetActive(false);
        LoadUIConfig().WrapErrors();
    }

    async Task LoadUIConfig()
    {
        var te = await Help.Instance.LoadCustomDirText("uiconfig.json", false);
        if (te != null)
        {
            if (te.StartsWith("\""))
                te = te.TrimStart('\"').TrimEnd('\"');
            te = te.Replace("None", "null");
            var dic = JsonMapper.ToObject(te);
            if (dic.Contains("data"))
                UiConfig = dic["data"];
            if (UiConfig == null)
                return;
            InitRext(title, "title");
            InitRext(youshang, "youshang");
            InitRext(youxia, "youxia");
            InitRext(zuoshang, "zuoshang");
            InitRext(zuoxia, "zuoxia");
        }
    }

    private async void InitRext(RectTransform target, string key)
    {
        var d = UiConfig[key];
        target.gameObject.SetActive((int)d["s"] == 1);
        if (!target.gameObject.activeSelf) return;
        var ps = d["p"].ToString().Split(',');
        target.anchoredPosition = new Vector2(int.Parse(ps[0]), int.Parse(ps[1]));
        ps = d["w"].ToString().Split(',');
        target.sizeDelta = new Vector2(int.Parse(ps[0]), int.Parse(ps[1]));
        if (string.IsNullOrEmpty(d["bgid"].ToString()))
        {
            var pd = d["bgurl"].ToString();
            if (!string.IsNullOrEmpty(pd))
                await LoadUrlTexture(target, pd);
        }

        var tqz = d["tqz"].ToString();
        target.GetComponentInChildren<Text>().text = tqz;
    }

    public async Task LoadUrlTexture(RectTransform target, string pd)
    {
        if (string.IsNullOrEmpty(pd))
        {
            target.GetComponent<RawImage>().texture = null;
            return;
        }
        if (pd.StartsWith("http"))
        {
            var tex = await Help.Instance.LoadTextureWWW(pd);
            target.GetComponent<RawImage>().texture = tex;
        }
        else if (pd.StartsWith("/"))
        {
            var dir = Help.Instance.customdirpath + pd;
            Debug.Log(dir);
            var tex = await Help.Instance.LoadTextureWWW(dir);
            target.GetComponent<RawImage>().texture = tex;
        }
    }

    bool isbuzy;
    private async void Update()
    {
        if (isbuzy || UiConfig == null) return;
        isbuzy = true;
        if (youshang.gameObject.activeSelf)
        {
            await LoadCam("youshang", youshang);
        }
        if (youxia.gameObject.activeSelf)
        {
            await LoadCam("youxia", youxia);
        }
        if (zuoshang.gameObject.activeSelf)
        {
            await LoadCam("zuoshang", zuoshang);
        }
        if (zuoxia.gameObject.activeSelf)
        {
            await LoadCam("zuoxia", zuoxia);
        }
        isbuzy = false;
    }

    private async Task LoadCam(string key, RectTransform target)
    {
        var id = UiConfig[key]["bgid"].ToString();
        if (id.StartsWith("camera"))
        {
            if (House.Instance.Hass.Contains(id))
            {
                var ca = House.Instance.Hass[id];
                var p = ca["attributes"]["entity_picture"];
                var dir = Shijie.domain + p;
                var te = await Help.Instance.LoadTextureWWW(dir);
                target.GetComponent<RawImage>().texture = te;
            }
        }
    }

    private static void ResetView()
    {
        if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.third)
        {
            CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
        }
        else if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.moba)
        {
            CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
            CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);

        }
        else if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.first)
        {

        }
    }

    private void ChangCamMode(int x)
    {
        if (x == 0)
        {
            foreach (var item in House.Instance.HouseData)
            {
                if (item.Value.entityType == HouseEntityType.eye)
                {
                    var e = item.Key.GetComponent<Eye>();
                    e.Show(false);
                }
            }
            CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
        }
        if (x == 1)
        {
            CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
            CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);
        }
        else if (x == 2)
        {
            List<Eye> eyes = new List<Eye>();
            foreach (var item in House.Instance.HouseData)
            {
                if (item.Value.entityType == HouseEntityType.eye)
                {
                    var e = item.Key.GetComponent<Eye>();
                    e.Show(true);
                    eyes.Add(e);
                }
            }
            if (eyes.Count > 0)
            {
                roOrMove.SetValue(0);   //第一人称 不允许平移
                eyes.Sort((x, y) => x.priority - y.priority);
                eyes[0].OnMouseUpAsButton();
            }
            else
            {
                changeCamera.SetValue(0);
            }
        }
    }

    private static async Task ClearGC()
    {
        await Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void OnDestroy()
    {

    }
}
