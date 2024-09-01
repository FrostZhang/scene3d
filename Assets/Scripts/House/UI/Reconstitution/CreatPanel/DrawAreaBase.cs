using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawAreaBase : MonoBehaviour
{
    public Toggle prefab;
    protected string fenlei;

    protected List<Toggle> fls;

    protected bool begindraw;
    protected Vector3 drawStartpos;
    protected bool hasChooseData;
    protected EditScenePanel.StandZiyuan choose;

    protected virtual void Awake()
    {
        prefab.gameObject.SetActive(false);
    }
    protected async void LoadMemu()
    {
        if (fenlei == null)
        {
            Debug.LogError("没有定义分类");
            return;
        }
        var ep = GetComponentInParent<EditScenePanel>();
        await new WaitUntilAsync(() => ep.Standdic != null);
        var dic = ep.Standdic;
        if (dic.ContainsKey(fenlei))
        {
            var fs = dic[fenlei];
            fls = new List<Toggle>(fs.Count);
            for (int i = 0; i < fs.Count; i++)
            {
                var f = fs[i];
                var te = await Help.Instance.LoadTextureLocal(true, f.previewpath);
                if (!te) continue;
                var p = Instantiate(prefab, prefab.transform.parent);
                p.gameObject.SetActive(true);
                p.onValueChanged.AddListener((x) =>
                {
                    if (x)
                    {
                        choose = f;
                        StartChoose();
                    }
                    else
                    {
                        choose = null;
                        EndChoose();
                    }
                });
                (p.targetGraphic as RawImage).texture = te;
                fls.Add(p);
            }
        }
    }

    protected virtual void Update()
    {
        if (hasChooseData)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                EditMode.BeginDraw();
                drawStartpos = Input.mousePosition;
                begindraw = true;
            }
            else if (begindraw && Input.GetMouseButtonUp(0))
            {
                EditMode.EndDraw();
                begindraw = false;
                DrawObject();
            }
        }
    }

    protected virtual void DrawObject()
    {

    }

    protected virtual void EndChoose()
    {
        hasChooseData = false;
        EditMode.canCheckClick = true;
    }

    protected virtual void StartChoose()
    {
        hasChooseData = true;
        EditMode.canCheckClick = false;
        EditMode.Instance.CloseHandle();
        EditMode.scenePanel.ChangeCameraMoe(1);
    }

    protected virtual void OnRenderObject()
    {
        if (begindraw)
        {
            GL.PushMatrix();
            RTEditor.MaterialPool.Instance.GizmoLine.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.red);
            var v1 = new Vector3(drawStartpos.x / Screen.width, drawStartpos.y / Screen.height, drawStartpos.z);
            var v2 = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, Input.mousePosition.z);
            GL.Vertex(v1);
            GL.Vertex(new Vector3(v1.x, v2.y, v2.z));
            GL.Vertex(v2);
            GL.Vertex(new Vector3(v2.x, v1.y, v2.z));
            GL.Vertex(v1);
            GL.Color(Color.magenta);
            GL.End();
            GL.PopMatrix();
        }

    }
}
