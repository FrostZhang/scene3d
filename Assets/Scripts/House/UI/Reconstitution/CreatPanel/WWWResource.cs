using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WWWResource : MonoBehaviour
{
    public Transform togPrefab;
    public Transform dragPrefab;
    public Transform btnprefab;

    public Sprite wall, floor, door, stand, sky, appliances, animal, eye;

    public class Ziyuan
    {
        public string url;
        public string priview;
        public HouseEntityType type;
    }
    List<Ziyuan> ziyuans;

    public Ziyuan choose;

    private void Awake()
    {
        ziyuans = new List<Ziyuan>(10);
        togPrefab.gameObject.SetActive(false);
        dragPrefab.gameObject.SetActive(false);
        btnprefab.gameObject.SetActive(false);
    }

    internal void Add(string url, string priview, HouseEntityType type)
    {
        if (ziyuans.Exists((x) => x.url == url))
            return;
        Ziyuan ziyuan = new Ziyuan();
        ziyuan.url = url;
        ziyuan.priview = priview;
        ziyuan.type = type;
        ziyuans.Add(ziyuan);
        switch (type)
        {
            case HouseEntityType.wall:
                break;
            case HouseEntityType.floor:
                break;
            case HouseEntityType.door:
                break;
            case HouseEntityType.stand:
                AddStandUI(ziyuan);
                break;
            case HouseEntityType.sky:
                break;
            case HouseEntityType.arealight:
                break;
            case HouseEntityType.appliances:
                break;
            case HouseEntityType.flowLine:
                break;
            case HouseEntityType.animal:
                break;
            case HouseEntityType.weather:
                break;
            case HouseEntityType.eye:
                break;
            case HouseEntityType.none:
                break;
            default:
                break;
        }
    }

    private async void AddStandUI(Ziyuan ziyuan)
    {
        var p = Instantiate(dragPrefab, dragPrefab.parent);
        p.gameObject.SetActive(true);
        p.Find("icon").GetComponent<Image>().sprite = type2Sp(ziyuan.type);
        var drag = p.GetComponentInChildren<DragIcon>();

        drag.BeginDrag = () =>
        {
            choose = ziyuan;
            EditMode.Instance.CloseHandle();
            CameraControllerForUnity.Instance.cameraMove = false;

        };
        drag.EndDrag = (pointer) =>
        {
            EditMode.Instance.OpenHandle();
            CameraControllerForUnity.Instance.cameraMove = true;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                CreatStand();
            }
            choose = null;
        };
        if (ziyuan.priview.StartsWith("http"))
        {
            var te = await Help.Instance.LoadTextureWWW(ziyuan.priview);
            (drag.targetGraphic as RawImage).texture = te;
        }
        else if (!string.IsNullOrEmpty(ziyuan.priview))
        {
            var te = await Help.Instance.LoadTextureWWW(Help.Instance.CombineCustomdataPath(ziyuan.priview));
            (drag.targetGraphic as RawImage).texture = te;
        }
    }

    private async void CreatStand()
    {
        var p = Input.mousePosition;
        p = Camera.main.ScreenToWorldPoint(p);
        Transform wall = null;
        wall = await House.Instance.CreatStandFromWWW(choose.url);
        p.y = 0;
        wall.transform.position = p;
        await new WaitForEndOfFrame();
        wall.GetComponent<Collider>().enabled = true;
        EditMode.Instance.EditTransform(wall);
    }

    private Sprite type2Sp(HouseEntityType entityType)
    {
        switch (entityType)
        {
            case HouseEntityType.wall:
                return wall;
            case HouseEntityType.floor:
                return floor;
            case HouseEntityType.door:
                return door;
            case HouseEntityType.stand:
                return stand;
            case HouseEntityType.sky:
                return sky;
            case HouseEntityType.arealight:
                return null;
            case HouseEntityType.appliances:
                return appliances;
            case HouseEntityType.flowLine:
                break;
            case HouseEntityType.animal:
                return animal;
            case HouseEntityType.eye:
                return eye;
        }
        return null;
    }
}
