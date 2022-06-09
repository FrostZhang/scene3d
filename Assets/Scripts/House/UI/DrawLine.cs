using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    public Toggle prefab;
    List<Toggle> tos;
    public Button add;

    static Toggle currentTo;
    void Start()
    {
        prefab.gameObject.SetActive(false);
        add.onClick.AddListener(async () =>
        {
            var e = await House.Instance.CreatFlowLine("0,0,0.1,2,0,0.1", -1, "255,255,255,255", "255,255,255,255", null);
            ShowLine(e);
        });
    }

    private void Ondel()
    {
        if (currentTo)
        {
            tos.Remove(currentTo);
            Destroy(currentTo.gameObject);
        }
    }

    private void ShowLine(LineEntity e)
    {
        if (e)
        {
            var l = e.line;
            var to = Instantiate(prefab, prefab.transform.parent);
            tos.Add(to);
            to.gameObject.SetActive(true);
            to.targetGraphic.color = e.line.startColor;
            var add = to.transform.Find("add");
            var del = to.transform.Find("del");
            to.onValueChanged.AddListener((x) =>
            {
                if (x)
                {
                    currentTo = to;
                    Reconstitution.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
                }
                else
                {
                    Reconstitution.Instance.CloseRTEditor();
                }
                add.gameObject.SetActive(x);
                del.gameObject.SetActive(x);
            });
            add.GetComponent<Button>().onClick.AddListener(() =>
            {
                var a = l.positionCount;
                l.positionCount = a + 1;
                var p = l.GetPosition(a - 1);
                l.SetPosition(a, p + Vector3.right);
                Reconstitution.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
            });
            del.GetComponent<Button>().onClick.AddListener(() =>
            {
                l.positionCount -= 1;
                Reconstitution.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
            });
            //to.isOn = true;
        }
    }

    void OnEnable()
    {
        tos = new List<Toggle>(20);
        var dic = House.Instance.CureetHouse;
        foreach (var item in dic)
        {
            if (item.Value == HouseEntityType.flowLine)
            {
                var e = item.Key.GetComponent<LineEntity>();
                ShowLine(e);
            }
        }
        PropPanle.Instance.Ondel += Ondel;
        Reconstitution.Instance.ChangeEditorTarget += ChangeEditorTarget;
    }

    private void ChangeEditorTarget(Transform obj)
    {
        if (currentTo)
        {
            currentTo.isOn = false;
            currentTo = null;
        }
    }

    void OnDisable()
    {
        Reconstitution.Instance.CloseRTEditor();
        foreach (var item in tos)
        {
            Destroy(item.gameObject);
        }
        tos.Clear();
        currentTo = null;
        PropPanle.Instance.Ondel -= Ondel;
        Reconstitution.Instance.ChangeEditorTarget -= ChangeEditorTarget;
    }

}
