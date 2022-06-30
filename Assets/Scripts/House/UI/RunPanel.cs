using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunPanel : MonoBehaviour
{
    public static RunPanel instance;
    public Transform right, left;
    public Material discMaterial;
    private bool candraw;
    public Vectrosity.VectorLine line;
    void Awake()
    {
        instance = this;
        //discMaterial = RTEditor.MaterialPool.Instance.GizmoSolidComponent;
        //discMaterial.SetInt("_ZTest", 0);
        //discMaterial.SetInt("_ZWrite", 1);
        //discMaterial.SetInt("_IsLit", 0);
        //discMaterial.SetInt("_CullMode", 0);
        right.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
    }

    bool anim = false;
    Transform curretBind;
    public async void Show(Transform bind, string mes)
    {
        if (anim)
            return;
        curretBind = bind;
        right.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        var start = Camera.main.WorldToScreenPoint(bind.position);
        Transform end = null;
        if (start.x > Screen.width * 0.5f)
            end = left;
        else
            end = right;
        //right
        line.points2 = new List<Vector2>(3);
        line.points2.Add(Vector2.zero);
        line.points2.Add(Vector2.zero);
        line.points2.Add(Vector2.zero);
        line.Draw();
        var pos2 = new Vector2(start.x + (end.position.x - start.x) * 0.5f, end.position.y);
        float t = 0;
        while ((t += Time.deltaTime) < 0.5f)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            line.points2[1]= Vector2.Lerp(start, pos2, t / 0.5f);
            await new WaitForEndOfFrame();
        }
        line.points2[1] = pos2;
        t = 0;
        while ((t += Time.deltaTime) < 0.5f)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            line.points2[2] = Vector2.Lerp(pos2, end.position, t / 0.5f);
            await new WaitForEndOfFrame();
        }
        end.gameObject.SetActive(true);
        end.GetComponentInChildren<Text>().text = mes;
        t = 0;
        while ((t += Time.deltaTime) < 0.35f)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            end.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, t / 0.5f);
            await new WaitForEndOfFrame();
        }
        anim = false;
        t = 0;
        while (curretBind == bind && ((t += Time.deltaTime) < 3f))
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            await new WaitForEndOfFrame();
        }
        if (curretBind == bind)
        {
            curretBind = null;
            line.points2.Clear();
            end.gameObject.SetActive(false);
        }
    }
}
