using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class EditScenePanel : MonoBehaviour, IPanel
{
    public static RawImage Icon;
    public Dictionary<string, List<StandZiyuan>> Standdic { get; private set; }
    public Transform Tr { get; set; }

    public StatesBtn changeCamera, roOrMove;
    public Button resetCamera, setting;
    public Button jed,save, exit;
    public Transform mainP;
    public Transform btns, panels;
    public RawImage icon;
    public DrawLine line;

    private void Awake()
    {
        Icon = icon;
        Standdic = new Dictionary<string, List<StandZiyuan>>(200);
    }

    public void LockCameraMode(int state)
    {
        changeCamera.SetValue(state);
        changeCamera.GetComponent<Button>().interactable = false;
        roOrMove.GetComponent<Button>().interactable = false;
    }

    public void UnLockCameraMode()
    {
        changeCamera.GetComponent<Button>().interactable = true;
        roOrMove.GetComponent<Button>().interactable = true;
        CameraControllerForUnity.Instance.roOrMove = roOrMove.State == 0;
        Set3D2D(changeCamera.State);
    }

    public void ChangeCameraMoe(int state)
    {
        changeCamera.SetValue(state);
    }

    public void Start()
    {
        icon.gameObject.SetActive(false);
        changeCamera.onValueChanged = (x) =>
        {
            Set3D2D(x);
        };

        roOrMove.onValueChanged = (x) =>
        {
            CameraControllerForUnity.Instance.roOrMove = x == 0;
        };
        CameraControllerForUnity.Instance.roOrMove = roOrMove.State == 0;

        resetCamera.onClick.AddListener(() =>
        {
            if (changeCamera.State == 0)
            {
                CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
            }
            else if (changeCamera.State == 1)
            {
                CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
                CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);
            }
        });

        setting.onClick.AddListener(() =>
        {
            mainP.gameObject.SetActive(true);
        });
        exit.onClick.AddListener(() =>
        {
            EditMode.EditScene(false);
        });
        jed.onClick.AddListener(() =>
        {
            mainP.gameObject.SetActive(false);
        });
        save.onClick.AddListener(Save);
        var bs = btns.GetComponentsInChildren<Button>();
        for (int i = 0; i < bs.Length; i++)
        {
            var b = bs[i];
            if (b.name == "daoru")
            {
                b.onClick.AddListener(() => OnDaoRu());
            }
            else
                b.onClick.AddListener(() =>
                {
                    foreach (Transform item in panels)
                    {
                        if (item.name != b.name)
                        {
                            item.gameObject.SetActive(false);
                        }
                        else
                        {
                            item.gameObject.SetActive(true);
                            mainP.gameObject.SetActive(false);
                        }
                    }
                });
        }
        LoadStandPrefab();
    }

    private void OnDaoRu()
    {
        Shijie.LoadWebFile("EditScenePanel", ".gldf");
    }

    //Shijie.LoadWebFile 回调
    public void FileDialogResult(string JsFileInfoStr)
    {
        mainP.gameObject.SetActive(false);
        Debug.Log(JsFileInfoStr);
        //JSFileInfo jsFileInfo = JsonConvert.DeserializeObject<JSFileInfo>(JsFileInfoStr);
        //Debug.Log(jsFileInfo.Path);
        //Debug.Log(jsFileInfo.FileName);
        //StartCoroutine(LoadData(jsFileInfo, null));
    }

    private void Set3D2D(int x)
    {
        if (x == 0 && CameraControllerForUnity.Instance.mode != CameraControllerForUnity.Mode.third)
        {
            var p = CameraControllerForUnity.Instance.transform.position;
            CameraControllerForUnity.Instance.Thirdfocus(p, new Vector2(45, 0), 12);
        }
        if (x == 1 && CameraControllerForUnity.Instance.mode != CameraControllerForUnity.Mode.moba)
        {
            var p = CameraControllerForUnity.Instance.transform.position;
            CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);
            CameraControllerForUnity.Instance.transform.position = p;
        }
    }

    internal void OnTrDelete(Transform chooseTr)
    {
        if (line.gameObject.activeSelf)
        {
            line.OnLineDelete(chooseTr);
        }
    }

    private void Save()
    {
        var hs = House.Instance.House2Json();
        Shijie.Config3D(hs);
        EmergencyPanel.instance.Show("已发送保存指令，建议刷新网页验证保存效果");
#if UNITY_EDITOR
        var path = Application.streamingAssetsPath + "/Customdata/houseconnfig.json";
        var dir = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        System.IO.File.WriteAllText(path, hs);
#endif
    }

    private async Task LoadStandPrefab()
    {
        var te = await Help.Instance.LoadStreamingAssetsText("3dscene.json", false);
        if (te != null)
        {
            var jd = LitJson.JsonMapper.ToObject(te);
            if (jd.IsArray)
            {
                jd.Foreach((cusname, jd) =>
                {
                    var dic = jd as IDictionary;
                    if (dic.Contains("datapath"))
                    {
                        StandZiyuan ziyuan = new StandZiyuan();
                        var d = dic["datapath"]?.ToString();
                        if (!string.IsNullOrEmpty(d))
                        {
                            d = d.Substring(d.IndexOf('/') + 1);
                            d = d.Remove(d.IndexOf('.'));
                            var ds = d.Split('_');
                            ziyuan.key = ds[0];
                            ziyuan.root = ds[1];
                            if (Standdic.ContainsKey(ziyuan.root))
                            {
                                Standdic[ziyuan.root].Add(ziyuan);
                            }
                            else
                            {
                                Standdic.Add(ziyuan.root, new List<StandZiyuan>() { ziyuan });
                            }
                        }
                        if (dic.Contains("previewpath"))
                        {
                            var p = dic["previewpath"];
                            ziyuan.previewpath = p?.ToString();
                        }
                    }
                });
            }
        }
    }


    public void OnDestroy()
    {

    }

    public class StandZiyuan
    {
        public string root;
        public string key;
        public string previewpath;
        public string supplier;
        public override string ToString()
        {
            return supplier + " " + root + " " + key + " " + previewpath;
        }
    }

}
