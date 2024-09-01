using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawDragBase : MonoBehaviour
{
    public Toggle prefab;
    public RawImage imprefab;

    protected string fenlei;
    protected EditScenePanel.StandZiyuan choose;
    protected Toggle[] alltos;
    protected string[] menu;
    protected string[] menuID;
    protected virtual void Awake()
    {
    }

    protected void LoadMemu()
    {
        if (fenlei == null || menu == null || menuID == null)
        {
            Debug.LogError("没有定义分类");
            return;
        }
        imprefab.gameObject.SetActive(false);
        alltos = new Toggle[menu.Length];
        for (int i = 0; i < menu.Length; i++)
        {
            var n = i;
            alltos[n] = Instantiate(prefab, prefab.transform.parent);
            alltos[n].GetComponentInChildren<Text>().text = menu[n];
            alltos[n].onValueChanged.AddListener((x) =>
            {
                if (x)
                {
                    LoadPrefab(menuID[n]);
                }
            });
        }
        prefab.gameObject.SetActive(false);
    }

    protected virtual async void LoadPrefab(string name)
    {
        foreach (var item in alltos) item.interactable = false;
        await QL();
        var ep = GetComponentInParent<EditScenePanel>();
        await new WaitUntilAsync(() => ep.Standdic != null);
        var dic = ep.Standdic;
        if (dic.ContainsKey(fenlei))
        {
            var fs = dic[fenlei];
            foreach (var item in fs)
            {
                var f = item;
                if (f.key.StartsWith(name))
                {
                    var te = await Help.Instance.LoadTextureLocal(true, f.previewpath);
                    if (!te) continue;
                    var p = Instantiate(imprefab, imprefab.transform.parent);
                    p.gameObject.SetActive(true);
                    p.texture = te;
                    var drag = p.GetComponent<DragIcon>();
                    drag.BeginDrag = () =>
                    {
                        choose = f;
                        EditMode.Instance.CloseHandle();
                        CameraControllerForUnity.Instance.cameraMove = false;
                        CameraControllerForUnity.Instance.cameraRot = false;
                        EditMode.BeginDraw();
                    };
                    drag.EndDrag = (pointer) =>
                    {
                        EditMode.Instance.OpenHandle();
                        CameraControllerForUnity.Instance.cameraMove = true;
                        CameraControllerForUnity.Instance.cameraRot = true;
                        EditMode.EndDraw();
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            Creat(choose);
                        }
                        choose = null;
                    };
                }
            }
        }
        foreach (var item in alltos) item.interactable = true;
    }

    protected virtual void Creat(EditScenePanel.StandZiyuan choose)
    {

    }

    private async Task QL()
    {
        for (int i = 1; i < imprefab.transform.parent.childCount; i++)
        {
            Destroy(imprefab.transform.parent.GetChild(i).gameObject);
        }
        await new WaitForUpdate();
        await Resources.UnloadUnusedAssets();
        System.GC.Collect();
        await new WaitForUpdate();
    }

}
