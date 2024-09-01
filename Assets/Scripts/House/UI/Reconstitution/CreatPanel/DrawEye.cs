using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawEye : DrawDragBase
{
    protected override void Awake()
    {
        fenlei = "look";
        menu = new string[] { "µ„Œª"};
        menuID = new string[] { "eye" };
        LoadMemu();
    }

    protected override async void Creat(EditScenePanel.StandZiyuan choose)
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        Transform wall = null;
        wall = await House.Instance.CreatEye(choose.key);
        p.y = 1.2f;
        wall.transform.position = p;
        wall.transform.eulerAngles = new Vector3(15, 0, 0);
        await new WaitForEndOfFrame();
        EditMode.Instance.EditTransform(wall);
        wall.GetComponent<Eye>().ReconstitutionMode(true);
    }
}
