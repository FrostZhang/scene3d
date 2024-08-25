using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawStand : MonoBehaviour
{
    EditScenePanel.StandZiyuan choose;
    public Button l, r;
    public Transform li;

    List<Transform> prefabs;
    List<Transform> fls;
    void Awake()
    {
        prefabs = new List<Transform>(8);
        foreach (Transform item in li)
        {
            var p = item.Find("Viewport/Content/Image");
            prefabs.Add(p);
            p.gameObject.SetActive(false);
            item.gameObject.SetActive(false);
        }
        li.GetChild(0).gameObject.SetActive(true);
        l.onClick.AddListener(() =>
        {
            foreach (Transform item in li)
            {
                if (item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(false);
                    if (item.GetSiblingIndex() == 0)
                    {
                        li.GetChild(li.childCount - 1).gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        li.GetChild(item.GetSiblingIndex() - 1).gameObject.SetActive(true);
                        return;
                    }
                }
            }
            li.GetChild(li.childCount - 1).gameObject.SetActive(true);
        });
        r.onClick.AddListener(() =>
        {
            foreach (Transform item in li)
            {
                if (item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(false);
                    if (item.GetSiblingIndex() + 1 >= li.childCount)
                    {
                        li.GetChild(0).gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        li.GetChild(item.GetSiblingIndex() + 1).gameObject.SetActive(true);
                        return;
                    }
                }
            }
            li.GetChild(0).gameObject.SetActive(true);
        });
    }

    void Start()
    {
        Ini();
    }

    async void Ini()
    {
        var ep = GetComponentInParent<EditScenePanel>();
        await new WaitUntilAsync(() => ep.Standdic != null);
        var dic = ep.Standdic;
        if (dic.ContainsKey("stand"))
        {
            var fs = dic["stand"];
            fls = new List<Transform>(fs.Count);
            for (int i = 0; i < fs.Count; i++)
            {
                var f = fs[i];
                var te = await Help.Instance.LoadTextureLocal(true, f.previewpath);
                if (!te) continue;
                var index = i % prefabs.Count;
                var p = Instantiate(prefabs[index], prefabs[index].parent);
                p.gameObject.SetActive(true);
                var drag = p.GetComponent<DragIcon>();
                drag.BeginDrag = () =>
                {
                    choose = f;
                    Reconstitution.Instance.CloseRTEditor();
                    CameraControllerForUnity.Instance.cameraMove = false;

                };
                drag.EndDrag = (pointer) =>
                {
                    Reconstitution.Instance.OpenRTEditor();
                    CameraControllerForUnity.Instance.cameraMove = true;
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        Creat();
                    }
                    choose = null;
                };
                (drag.targetGraphic as RawImage).texture = te;
                fls.Add(p);
            }
        }
    }

    private async void Creat()
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        Transform wall = null;
        wall = await House.Instance.CreatStand(choose.key, null);
        p.y = 0;
        wall.transform.position = p;
        await new WaitForEndOfFrame();
        wall.GetComponent<Collider>().enabled = true;
        Reconstitution.Instance.EditorTransform(wall);
    }
}
