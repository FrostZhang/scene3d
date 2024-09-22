using EnhancedUI.EnhancedScroller;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebGLSupport.Detail;

public class EntityListPanel : MonoBehaviour, IPanel, IEnhancedScrollerDelegate
{
    public Button exit, kong;
    public InputField sec;
    System.Action<string> callback;
    public EnhancedScroller scroller;
    public EntityListChild cellViewPrefab;

    public Transform Tr { get; set; }
    List<JsonData> temp;
    void Awake()
    {
        temp = new List<JsonData>(999);
        sec.onEndEdit.AddListener((x) =>
        {
            if (House.Instance.Hass == null)
            {
                sec.text = "Î´¼ÓÔØEntity!";
                sec.textComponent.color = Color.red;
                return;
            }
            if (string.IsNullOrEmpty(x))
            {
                LoadAll();
            }
            else
            {
                LoadHasStr(x);
            }
        });

        kong.onClick.AddListener(() =>
        {
            callback?.Invoke("");
        });
    }

    private void LoadHasStr(string x)
    {
        temp.Clear();
        House.Instance.Hass.Foreach((key, jd) =>
        {
            if (key != null)
            {
                if (key.Contains(x))
                {
                    temp.Add(jd);
                }
            }
            else
            {
                if (jd.Contains("attributes"))
                    if (jd["attributes"].Contains("friendly_name"))
                    {
                        var sn = jd["attributes"]["friendly_name"].ToString();
                        if (sn.Contains(x))
                            temp.Add(jd);
                    }
            }
        });
        scroller.ReloadData();
    }

    public void Start()
    {
        scroller.Delegate = this;
        exit.onClick.AddListener(() =>
        {
            Surface.surface.RemovePanel<EntityListPanel>();
        });

        if (House.Instance.Hass == null)
        {
            sec.text = "Î´¼ÓÔØEntity!";
            sec.textComponent.color = Color.red;
            return;
        }
        LoadAll();
    }

    private void LoadAll()
    {
        temp.Clear();
        House.Instance.Hass.Foreach((key, jd) =>
        {
            temp.Add(jd);
        });
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return temp.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 30f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        EntityListChild cellView = scroller.GetCellView(cellViewPrefab) as EntityListChild;
        var jd = temp[dataIndex];
        var sn = "";
        if (jd.Contains("attributes"))
        {
            if (jd["attributes"].Contains("friendly_name"))
            {
                sn = jd["attributes"]["friendly_name"].ToString();
            }
        }
        if (jd.Contains("entity_id"))
        {
            sn += jd["entity_id"].ToString();
        }
        cellView.GetComponentInChildren<Text>().text = sn;
        cellView.GetComponent<Button>().onClick.RemoveAllListeners();
        cellView.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (jd.Contains("entity_id"))
            {
                callback?.Invoke(jd["entity_id"].ToString());
            }
        });
        return cellView;
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

    public void ChooseEntity(System.Action<string> back)
    {
        transform.SetAsLastSibling();
        this.callback = back;
    }

    public void OnDestroy()
    {

    }
}
