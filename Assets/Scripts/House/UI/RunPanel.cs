using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunPanel : MonoBehaviour
{
    public static RunPanel instance;
    public Transform right, left, right2, left2;
    public Material discMaterial;
    private bool candraw;
    Vectrosity.VectorLine line;
    void Awake()
    {
        instance = this;
        right.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        left2.gameObject.SetActive(false);
        right2.gameObject.SetActive(false);
        line = new Vectrosity.VectorLine("line", new List<Vector2>(), 2, Vectrosity.LineType.Continuous);
        line.material = discMaterial;
    }

    bool anim = false;
    Transform curretBind;
    float speed = 0.15f;
    public  void Show(Transform bind, string mes)
    {
        StopAllCoroutines();
        curretBind = bind;
        right.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        left2.gameObject.SetActive(false);
        right2.gameObject.SetActive(false);
        line.points2.Clear();
        StartCoroutine(_Anim(bind, mes));
    }

    IEnumerator _Anim(Transform bind, string mes)
    {
        var start = Camera.main.WorldToScreenPoint(bind.position);
        Transform end = null;
        if (start.x > Screen.width * 0.5f)
        {
            if (start.y > Screen.height * 0.5f)
                end = left2;
            else
                end = left;
        }
        else
        {
            if (start.y > Screen.height * 0.5f)
                end = right2;
            else
                end = right;
        }
        //if (start.x > Screen.width * 0.5f)
        //{
        //    if (start.y > Screen.height * 0.5f)
        //        end = right;
        //    else
        //        end = right2;
        //}
        //else
        //{
        //    if (start.y > Screen.height * 0.5f)
        //        end = left;
        //    else
        //        end = left2;
        //}
        //right
        var pos2 = new Vector2(start.x + (end.position.x - start.x) * 0.5f, end.position.y);
        line.points2.Add(pos2);
        line.points2.Add(pos2);
        line.Draw();
        float t = 0;
        while ((t += Time.deltaTime) < speed)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            line.points2[1] = Vector2.Lerp(start, pos2, t / speed);
            line.Draw();
            yield return new WaitForEndOfFrame();
        }
        line.points2[1] = pos2;
        line.points2.Add(pos2);
        t = 0;
        while ((t += Time.deltaTime) < speed)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            line.points2[2] = Vector2.Lerp(pos2, end.position, t / speed);
            line.Draw();
            yield return new WaitForEndOfFrame();
        }
        line.points2[2] = end.position;
        end.gameObject.SetActive(true);
        end.GetComponentInChildren<Text>().text = mes;
        t = 0;
        while ((t += Time.deltaTime) < speed)
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            end.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, t / speed);
            line.Draw();
            yield return new WaitForEndOfFrame();
        }
        t = 0;
        while (((t += Time.deltaTime) < 3f))
        {
            var spos = Camera.main.WorldToScreenPoint(bind.position);
            line.points2[0] = spos;
            line.Draw();
            yield return new WaitForEndOfFrame();
        }
        curretBind = null;
        line.points2.Clear();
        line.Draw();
        end.gameObject.SetActive(false);
    }
}
