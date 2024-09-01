using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawWall : DrawAreaBase
{

    void Start()
    {
        fenlei = "wall";
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
        size.x = Mathf.Abs(size.x);
        size.z = Mathf.Abs(size.z);
        Transform wall = null;

        if (size.x > size.z)
        {
            wall = await House.Instance.CreatWall(choose.key
                                , $"{center.x},{center.z},0,0,{size.x},1,{size.z}"
                                , "195,216,235,255"
                                );
        }
        else
        {
            wall = await House.Instance.CreatWall(choose.key
                    , $"{center.x},{center.z},0,90,{size.z},1,{size.x}"
                    , "195,216,235,255"
                    );
        }
        await new WaitForEndOfFrame();
        wall.GetComponent<Collider>().enabled = true;
        foreach (var item in fls)
            item.isOn = false;
        EditMode.Instance.EditTransform(wall);
    }
}
