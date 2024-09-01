using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawStand : DrawDragBase
{

    protected override void Awake()
    {
        fenlei = "stand";
        menu = new string[] { "´²", "ÂÌÖ²", "ÒÎ", "¹ñ", "²è¼¸", "×À", "É³·¢", "Ô¡ÊÒ", "´°","ÆäËû" };
        menuID = new string[] { "bed", "lvzhi", "chair", "guizi", "chaji", "zhuozi", "sofa", "yushi", "window","other" };
        LoadMemu();
    }


    protected override async void Creat(EditScenePanel.StandZiyuan choose)
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        Transform wall = null;
        wall = await House.Instance.CreatStand(choose.key, null);
        p.y = 0;
        wall.transform.position = p;
        await new WaitForEndOfFrame();
        wall.GetComponent<Collider>().enabled = true;
        EditMode.Instance.EditTransform(wall);
    }
}
