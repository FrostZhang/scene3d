using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drawlight : DrawAreaBase
{
    public ToggleGroup tg;
    Color32 lightC;
    protected override void Awake()
    {
        fls = new List<Toggle>(tg.GetComponentsInChildren<Toggle>());
        foreach (var item in fls)
        {
            item.onValueChanged.AddListener(OnSelectChange);
        }
    }

    private void OnSelectChange(bool b)
    {
        if (b)
        {
            foreach (var item in fls)
            {
                if (item.isOn)
                {
                    lightC = item.targetGraphic.color;
                    StartChoose();
                    return;
                }
            }
        }
        else
        {
            EndChoose();
        }
    }

    protected override async void DrawObject()
    {
        var p = Input.mousePosition;
        if (Mathf.Abs(p.x - drawStartpos.x) < 10 || Mathf.Abs(p.y - drawStartpos.y) < 10) return;
        drawStartpos = Camera.main.ScreenToWorldPoint(drawStartpos);
        p = Camera.main.ScreenToWorldPoint(p);
        var size = p - drawStartpos;
        var center = drawStartpos + size * 0.5f;
        var light = House.Instance.CreatAreaLight($"{center.x},{center.z},{Mathf.Abs(size.x)},{Mathf.Abs(size.z)},3"
            , null
            , $"{lightC.r},{lightC.g},{lightC.b},{lightC.a}"
            );
        await new WaitForEndOfFrame();
        light.ReconstitutionMode(true);
        foreach (var item in fls)
        {
            item.isOn = false;
        }
        EditMode.Instance.EditTransform(light.transform);
    }

}
