using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawDoor : DrawDragBase
{
    protected override void Awake()
    {
        fenlei = "door";
        menu = new string[] { "√≈", "“∆√≈" };
        menuID = new string[] { "swingdoor", "slidingdoor" };
        LoadMemu();
    }

    protected override async void Creat(EditScenePanel.StandZiyuan choose)
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        var wall = await House.Instance.CreatDoor(choose.key
                             , $"{p.x},{p.z},0,1,1,1,0,0"
                             , null
                             , null
                             );
        await new WaitForEndOfFrame();
        wall.ReconstitutionMode(true);
        EditMode.Instance.EditTransform(wall.transform);
    }
}
