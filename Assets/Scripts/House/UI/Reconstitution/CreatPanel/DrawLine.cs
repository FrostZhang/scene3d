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
            SpawnUI(e);
        });

        tos = new List<Toggle>(20);
        var dic = House.Instance.HouseData;
        foreach (var item in dic)
        {
            if (item.Value.entityType == HouseEntityType.flowLine)
            {
                var e = item.Key.GetComponent<LineEntity>();
                SpawnUI(e);
            }
        }
    }

    public void OnLineDelete(Transform chooseTr)
    {
        if (currentTo)
        {
            tos.Remove(currentTo);
            Destroy(currentTo.gameObject);
            currentTo = null;
        }
    }

    private void SpawnUI(LineEntity e)
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
                    EditMode.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
                    EditMode.canCheckClick = false;
                }
                else
                {
                    EditMode.Instance.CloseHandle();
                    EditMode.canCheckClick = true;
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
                EditMode.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
            });
            del.GetComponent<Button>().onClick.AddListener(() =>
            {
                l.positionCount -= 1;
                EditMode.Instance.EditLine(e, (x) => { to.targetGraphic.color = x; });
            });
        }
    }

    void OnDisable()
    {
        currentTo = null;
    }


}
