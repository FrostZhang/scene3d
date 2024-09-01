using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawApp : DrawDragBase
{
    protected override void Awake()
    {
        fenlei = "appliances";
        menu = new string[] { "空调", "帘", "吊扇", "小灯", "文字", "电视", "洗衣机", "报警器", "其他" };
        menuID = new string[] { "ac", "cover", "fan", "lamp", "text", "tv", "washer", "alarm", "other" };
        LoadMemu();
    }

    protected override async void Creat(EditScenePanel.StandZiyuan choose)
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        var wall = await House.Instance.CreatAppliances(choose.key, null, choose.supplier);
        p.y = 0;
        wall.transform.position = p;
        await new WaitForEndOfFrame();
        wall.ReconstitutionMode(true);
        EditMode.Instance.EditTransform(wall.transform);
    }
}
