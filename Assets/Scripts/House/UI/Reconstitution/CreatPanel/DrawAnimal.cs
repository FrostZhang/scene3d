using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawAnimal : DrawAreaBase
{

    void Start()
    {
        fenlei = "animal";
        LoadMemu();
    }

    protected override async void DrawObject()
    {
        var p = Input.mousePosition;
        if (Mathf.Abs(p.x - drawStartpos.x) < 10 || Mathf.Abs(p.y - drawStartpos.y) < 10) return;
        drawStartpos = Camera.main.ScreenToWorldPoint(drawStartpos);
        p = Camera.main.ScreenToWorldPoint(p);
        var size = p - drawStartpos;
        var center = drawStartpos + size * 0.5f;
        var light = await House.Instance.CreatAnim(choose.key, $"{center.x},{center.z},{Mathf.Abs(size.x)},{Mathf.Abs(size.z)}");
        await new WaitForEndOfFrame();
        foreach (var item in fls)
        {
            item.isOn = false;
        }
        EditMode.Instance.EditTransform(light.transform);
    }
}
