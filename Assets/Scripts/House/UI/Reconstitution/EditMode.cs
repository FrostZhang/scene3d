using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditMode : MonoBehaviour
{
    public static EditMode Instance;
    public static EditScenePanel scenePanel;
    public static EditPropPanle prop;
    public static bool canCheckClick = true;
    public static bool UnLockCamera
    {
        get { return CameraControllerForUnity.Instance.cameraMove; }
        set
        {
            CameraControllerForUnity.Instance.cameraMove = value;
            CameraControllerForUnity.Instance.cameraRot = value;
        }
    }

    public Action<Transform> OnClick;
    Camera mainCa;
    Transform cs;
    private RaycastHit hit;
    private float maxDistance = 25;
    CHHandle chhadle;
    List<RTEditor.Gizmo> ligizmo;
    Vector3 offset = new Vector3(1, 0, 1) * 0.125f;
    HouseEntityData chooseData;
    Transform chooseTr;

    void Start()
    {
        mainCa = Camera.main;
        chhadle = new CHHandle();
        //caTr = mainCa.transform;
        ligizmo = new List<RTEditor.Gizmo>(5);
    }
    private void OnDestroy()
    {
        chhadle.DeleteAll();
    }
    public static void BeginDraw()
    {
        scenePanel.LockCameraMode(1);
        UnLockCamera = false;
    }

    public static void EndDraw()
    {
        scenePanel.UnLockCameraMode();
        UnLockCamera = true;

    }

    private void WantDelete()
    {
        UnLockCamera = true;
        ClearHandle();
        House.Instance.Delete(chooseTr);
        scenePanel.OnTrDelete(chooseTr);
        prop.Show(false);
        chooseTr = null;
        chooseData = null;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void OpenHandle()
    {

    }

    public void CloseHandle()
    {
        prop.Show(false);
        chooseTr = null;
        chooseData = null;
        ClearHandle();
    }
    public static void EditScene(bool edit)
    {
        if (edit)
        {
            if (Instance == null)
            {
                var g = new GameObject("EditMode", typeof(EditMode));
                Instance = g.GetComponent<EditMode>();
                Instance._EditScene(true);
            }
        }
        else
        {
            if (Instance)
            {
                Instance._EditScene(false);
                Destroy(Instance.gameObject);
            }
        }
        CacheData.isEditorMode = edit;
        CacheData.OnEditorMode?.Invoke(edit);
    }

    void _EditScene(bool edit)
    {
        if (!edit)
        {
            CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
            cs = House.Instance.Root;
            foreach (Transform item in cs)
            {
                var e = item.GetComponent<HassEntity>();
                if (e)
                {
                    e.ReconstitutionMode(false);
                }
                else
                {
                    var eye = item.GetComponent<Eye>();
                    if (eye)
                    {
                        eye.ReconstitutionMode(false);
                    }
                    else
                    {
                        var c = item.GetComponent<Collider>();
                        if (c) c.enabled = false;
                    }
                }
            }
            //BoundsPanel.Instance.Bingding(null, null);
            CloseHandle();
            scenePanel = null;
            Surface.surface.RemovePanel<EditScenePanel>();
            prop = null;
            Surface.surface.RemovePanel<EditPropPanle>();
            Surface.surface.LoadPanel<HousePanel>();
        }
        else
        {
            scenePanel = Surface.surface.LoadPanel<EditScenePanel>();
            prop = Surface.surface.LoadPanel<EditPropPanle>();
            prop.Ondel = WantDelete;
            CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
            //CameraControllerForUnity.Instance.MobaFollow_orthogonal(rigTr, new Vector2(90, 0), 15, 5);
            //CameraControllerForUnity.Instance.orthographicSizeLimit = new Vector2(2, 10);

            //通知所有单位进入编辑模式
            cs = House.Instance.Root;
            //bounds = new Dictionary<Transform, Bounds>(cs.childCount);
            foreach (Transform item in cs)
            {
                var e = item.GetComponent<HassEntity>();
                if (e) e.ReconstitutionMode(true);
                else
                {
                    var eye = item.GetComponent<Eye>();
                    if (eye)
                    {
                        eye.ReconstitutionMode(true);
                    }
                    else
                    {
                        var c = item.GetComponent<Collider>();
                        if (c) c.enabled = true;
                    }
                }
            }
            OpenHandle();
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var item in ligizmo)
            {
                //点击到gzm
                if (item.IsReadyForObjectManipulation())
                {
                    UnLockCamera = false;
                    return;
                }
            }
            if (canCheckClick && Physics.Raycast(mainCa.ScreenPointToRay(Input.mousePosition), out hit, maxDistance))
            {
                if (hit.transform != chooseTr)
                {
                    ClearHandle();
                    SelectTr(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            UnLockCamera = true;
        }
    }

    private void ClearHandle()
    {
        foreach (var item in ligizmo)
            chhadle.ReHandle(item);
        ligizmo.Clear();
    }

    public void EditTransform(Transform obj)
    {
        if (chooseTr != obj)
        {
            chooseTr = obj;
            ClearHandle();
            SelectTr(chooseTr);
        }
    }

    private void SelectTr(Transform lasthit)
    {
        if (House.Instance.HouseData.ContainsKey(lasthit))
        {
            chooseData = House.Instance.HouseData[lasthit];
            chooseTr = lasthit;
        }
        else
        {
            Debug.LogError("选中了非场景内的东西" + lasthit.name);
            return;
        }
        OnClick?.Invoke(lasthit);
        var h = chhadle.PositionHandle(lasthit.position + offset, Quaternion.identity, (x) =>
             {
                 lasthit.position += x;
                 FlushPropPanle();
             });
        h.SetAxisVisibility(false, 1);
        h.AxisLength = 2.5f;
        ligizmo.Add(h);
        switch (chooseData.entityType)
        {
            case HouseEntityType.wall:
            case HouseEntityType.floor:
            case HouseEntityType.arealight:
            case HouseEntityType.animal:
                var s = chhadle.VolumeHandle(lasthit.gameObject, (a, b) =>
                {
                    lasthit.localScale = a;
                    lasthit.position = b;
                    h.transform.position = lasthit.position + offset;
                    FlushPropPanle();
                });
                s.SetAxisVisibility(false, 1);
                ligizmo.Add(s);
                break;
            case HouseEntityType.stand:
            case HouseEntityType.appliances:
            case HouseEntityType.eye:
            case HouseEntityType.door:

                break;
            default:
                break;
        }
        ShowDetail(lasthit);
    }


    void ShowDetail(Transform target)
    {
        switch (chooseData.entityType)
        {
            case HouseEntityType.wall:
                ShowWallProp(target);
                break;
            case HouseEntityType.floor:
                ShowWallFloor(target);
                break;
            case HouseEntityType.door:
                ShowDoor(target);
                break;
            case HouseEntityType.stand:
                ShowStand(target);
                break;
            //case HouseEntityType.sky:
            //    break;
            case HouseEntityType.arealight:
                ShowAreaLight(target);
                break;
            case HouseEntityType.appliances:
                ShowAppliances(target);
                break;
            //case HouseEntityType.flowLine:
            //    break;
            case HouseEntityType.animal:
                ShowAnimal(target);
                break;
            //case HouseEntityType.weather:
            //    break;
            case HouseEntityType.eye:
                ShowEye(target);
                break;
            default:
                prop.Show(false);
                break;
        }
    }

    private void ShowAnimal(Transform target)
    {
        prop.Clear();
        prop.GetV2("位置", target.position.x, target.position.z,
            (x) =>
            {
                var pc = target.position;
                pc.x = x;
                target.position = pc;
                FlushGizmo(pc);
            }, (x) =>
            {
                var pc = target.position;
                pc.z = x;
                target.position = pc;
                FlushGizmo(pc);
            });
        var anie = target.GetComponent<AnimalEntity>();
        if (anie.canmove)
        {
            prop.GetV2("区域", target.localScale.x, target.localScale.z,
                    (x) =>
                    {
                        var pc = target.localScale;
                        if (x == 0) x = 0.1f;
                        pc.x = x;
                        target.localScale = pc;
                    }, (x) =>
                    {
                        var pc = target.localScale;
                        if (x == 0) x = 0.1f;
                        pc.z = x;
                        target.localScale = pc;
                    });
        }
        prop.Show(true);
    }

    private void FlushPropPanle()
    {
        switch (chooseData.entityType)
        {
            case HouseEntityType.wall:
                var p = chooseTr.position;
                var sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 3, 4, 5 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.floor:
                p = chooseTr.position;
                sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 3 }, p.x, p.z, sc.x, sc.z);
                break;
            case HouseEntityType.door:
                p = chooseTr.position;
                sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 3, 4, 5 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.stand:
                p = chooseTr.position;
                sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 6, 7, 8 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.sky:
                break;
            case HouseEntityType.arealight:
                p = chooseTr.position;
                sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 3 }, p.x, p.z, sc.x, sc.z);
                break;
            case HouseEntityType.appliances:
                p = chooseTr.position;
                sc = chooseTr.localScale;
                prop.Flush(new int[] { 0, 1, 2, 6, 7, 8 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.flowLine:
                break;
            case HouseEntityType.animal:
                break;
            case HouseEntityType.weather:
                break;
            default:
                break;
        }
    }

    private void ShowAppliances(Transform target)
    {
        prop.Clear();
        prop.GetV3("位置", target.position.x, target.position.z, target.position.y,
            (x) =>
            {
                var pc = target.position;
                pc.x = x;
                target.position = pc;
                FlushGizmo(pc);
            }, (x) =>
            {
                var pc = target.position;
                pc.z = x;
                target.position = pc;
                FlushGizmo(pc);
            }, (x) =>
            {
                var pc = target.position;
                pc.y = x;
                target.position = pc;
                FlushGizmo(pc);
            });
        prop.GetV3("角度", target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z,
            (x) =>
            {
                var pc = target.localEulerAngles;
                pc.x = x;
                target.localEulerAngles = pc;
            }, (x) =>
            {
                var pc = target.localEulerAngles;
                pc.y = x;
                target.localEulerAngles = pc;
            }, (x) =>
            {
                var pc = target.localEulerAngles;
                pc.z = x;
                target.localEulerAngles = pc;
            });
        prop.GetV3("缩放", target.localScale.x, target.localScale.y, target.localScale.z,
            (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.x = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.y = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.z = x;
                target.localScale = pc;
            });
        var door = target.GetComponent<HassEntity>();
        if (door is AlarmEntity)
        {
            var e = door as AlarmEntity;
            var ma = e.YuanMa;
            prop.GetColor("颜色", ma.GetColor("_Color"), (x) => ma.SetColor("_Color", x));
            //PropPanle.Instance.GetColor("Edge", ma.GetColor("_HeightLight"), (x) => ma.SetColor("_HeightLight", x));
            //PropPanle.Instance.GetColor("Depth", ma.GetColor("_DepthColor"), (x) => ma.SetColor("_DepthColor", x));
        }
        else if (door is LampEntity)
        {
            var e = door as LampEntity;
            prop.GetColor("颜色", e.emColor, (x) => e.emColor = x);
        }
        else if (door is TextEntity)
        {
            var e = door as TextEntity;
            prop.GetText("前缀", e.front, (x) =>
            {
                e.front = x;
                e.FlushText();
            });
            prop.GetText("后缀", e.back, (x) =>
            {
                e.back = x;
                e.FlushText();
            });
            prop.GetColor("颜色", e.textMesh.color, (x) => e.textMesh.color = x);
            prop.GetV1("字号", e.textMesh.fontSize, (x) =>
            {
                e.textMesh.fontSize = (int)x;
            });
        }
        else if (door is CoverEntity)
        {
            var e = door as CoverEntity;
            prop.GetV1("Open", e.openW, (x) =>
            {
                e.openW = x;
            });
        }
        prop.GetEntity("ID", door.Entity_id, (x) =>
        {
            door.SetEntity(x);
        });
        prop.Show(true);
    }

    private void ShowAreaLight(Transform target)
    {
        var door = target.GetComponent<LightEntity>();
        prop.Clear();
        prop.GetV2("位置", target.position.x, target.position.z, (x) =>
        {
            var pc = target.position;
            pc.x = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.z = x;
            target.position = pc;
            FlushGizmo(pc);
        });
        prop.GetV2("区域", target.localScale.x, target.localScale.z, (x) =>
        {
            var pc = target.localScale;
            pc.x = x;
            target.localScale = pc;
        }, (x) =>
        {
            var pc = target.localScale;
            pc.z = x;
            target.localScale = pc;
        });
        prop.GetV1("亮度", door.Max, (x) =>
        {
            door.Max = x;
        });
        prop.GetColor("颜色", door.clight.color, (x) => door.clight.color = x);
        prop.GetEntity("绑定", door.Entity_id, (x) =>
        {
            door.SetEntity(x);
        });
        prop.Show(true);
    }

    private void FlushGizmo(Vector3 pc)
    {
        if (ligizmo.Count > 0)
        {
            ligizmo[0].transform.position = pc + offset;
        }
    }

    private void ShowWallFloor(Transform target)
    {
        prop.Clear();
        prop.GetV2("位置", target.position.x, target.position.z, (x) =>
        {
            var pc = target.position;
            pc.x = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.z = x;
            target.position = pc;
            FlushGizmo(pc);
        });
        prop.GetV2("区域", target.localScale.x * 10f, target.localScale.z * 10f, (x) =>
          {
              var pc = target.localScale;
              pc.x = x * 0.1f;
              target.localScale = pc;
          }, (x) =>
          {
              var pc = target.localScale;
              pc.z = x * 0.1f;
              target.localScale = pc;
          });
        var ma = target.GetComponent<MeshRenderer>().material;
        prop.GetV2("平铺", ma.GetTextureScale("_BaseMap").x, ma.GetTextureScale("_BaseMap").y, (x) =>
        {
            var s = ma.GetTextureScale("_BaseMap");
            ma.SetTextureScale("_BaseMap", new Vector2(x, s.y));
        }, (x) =>
        {
            var s = ma.GetTextureScale("_BaseMap");
            ma.SetTextureScale("_BaseMap", new Vector2(s.x, x));
        });
        prop.GetV1("层级", target.position.y * 100f, (x) =>
        {
            var p = target.position;
            p.y = x * 0.01f;
            target.position = p;
        });
        prop.Show(true);
    }

    private void ShowDoor(Transform target)
    {
        prop.Clear();
        prop.GetV3("位置", target.position.x, target.position.z, target.position.y, (x) =>
        {
            var pc = target.position;
            pc.x = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.z = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.y = x;
            target.position = pc;
            FlushGizmo(pc);
        });
        prop.GetV3("缩放", target.localScale.x, target.localScale.y, target.localScale.z,
            (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.x = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.y = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.z = x;
                target.localScale = pc;
            });

        var door = target.GetComponent<DoorEntity>();
        if (door)
        {
            prop.GetV1("角度关", door.angleclose, (x) =>
            {
                var pc = target.eulerAngles;
                pc.y = x;
                target.eulerAngles = pc;
                door.angleclose = x;
            });
            prop.GetV1("角度开", door.angleopen, (x) =>
            {
                door.angleopen = x;
            });
            prop.GetEntity("绑定", door.Entity_id, (x) =>
            {
                door.SetEntity(x);
            });
            prop.GetColor("颜色", door.YuanMa.color, (x) => door.YuanMa.color = x);
            prop.GetButton("模拟", "---  ---", () =>
            {
                door.Sim();
            });
        }
        else
        {
            var sdoor = target.GetComponent<SlidingDoorEntity>();
            if (sdoor)
            {
                prop.GetV1("角度", target.localEulerAngles.y, (x) =>
                {
                    var pc = target.localEulerAngles;
                    pc.y = x;
                    target.localEulerAngles = pc;
                });
                prop.GetBoolen("左移", sdoor.iszuoyi, (x) =>
                {
                    sdoor.iszuoyi = x;
                });
                prop.GetEntity("绑定", sdoor.Entity_id, (x) =>
                {
                    sdoor.SetEntity(x);
                });
                prop.GetColor("颜色", sdoor.YuanMa.color, (x) => sdoor.YuanMa.color = x);
                prop.GetButton("模拟", "---  ---", () =>
                {
                    sdoor.Sim();
                });
            }
        }

        prop.Show(true);
    }

    private void ShowStand(Transform target)
    {
        prop.Clear();
        prop.GetV3("位置", target.position.x, target.position.z, target.position.y,
            (x) =>
        {
            var pc = target.position;
            pc.x = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.z = x;
            target.position = pc;
            FlushGizmo(pc);
        }, (x) =>
        {
            var pc = target.position;
            pc.y = x;
            target.position = pc;
            FlushGizmo(pc);
        });
        prop.GetV3("角度", target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z,
            (x) =>
        {
            var pc = target.localEulerAngles;
            pc.x = x;
            target.localEulerAngles = pc;
        }, (x) =>
        {
            var pc = target.localEulerAngles;
            pc.y = x;
            target.localEulerAngles = pc;
        }, (x) =>
        {
            var pc = target.localEulerAngles;
            pc.z = x;
            target.localEulerAngles = pc;
        });
        prop.GetV3("缩放", target.localScale.x, target.localScale.y, target.localScale.z,
            (x) =>
        {
            var pc = target.localScale;
            if (x == 0) x = 0.1f;
            pc.x = x;
            target.localScale = pc;
        }, (x) =>
        {
            var pc = target.localScale;
            if (x == 0) x = 0.1f;
            pc.y = x;
            target.localScale = pc;
        }, (x) =>
        {
            var pc = target.localScale;
            if (x == 0) x = 0.1f;
            pc.z = x;
            target.localScale = pc;
        });


        prop.Show(true);
    }

    private void ShowEye(Transform target)
    {
        ShowStand(target);
        var e = target.GetComponent<Eye>();
        prop.GetV1("序列", e.priority, (x) => e.priority = (int)x);
        prop.GetText("标记", e.textMesh.text, (x) => e.textMesh.text = x);
    }

    private void ShowWallProp(Transform target)
    {
        prop.Clear();
        prop.GetV3("位置", target.position.x, target.position.z, target.position.y,
             (x) =>
             {
                 var pc = target.position;
                 pc.x = x;
                 target.position = pc;
                 FlushGizmo(pc);
             }, (x) =>
             {
                 var pc = target.position;
                 pc.z = x;
                 target.position = pc;
                 FlushGizmo(pc);
             }, (x) =>
             {
                 var pc = target.position;
                 pc.y = x;
                 target.position = pc;
                 FlushGizmo(pc);
             });
        prop.GetV3("长高宽", target.localScale.x, target.localScale.y, target.localScale.z,
            (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.x = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.y = x;
                target.localScale = pc;
            }, (x) =>
            {
                var pc = target.localScale;
                if (x == 0) x = 0.1f;
                pc.z = x;
                target.localScale = pc;
            });
        prop.GetV1("角度", target.localEulerAngles.y, (x) =>
        {
            var pc = target.localEulerAngles;
            pc.y = x;
            target.localEulerAngles = pc;
        });
        var r = target.GetComponent<MeshRenderer>().material;
        prop.GetColor("颜色", r.color, (x) => r.color = x);
        prop.Show(true);
    }

    /// <summary>
    /// 由于线条不可选中，所以采用点击UI列表展示
    /// </summary>
    public void EditLine(LineEntity e, Action<Color> onchangeC)
    {
        chooseTr = e.transform;
        chooseData = House.Instance.HouseData[chooseTr];
        ClearHandle();
        prop.Clear();
        var l = e.line;
        for (int i = 0; i < l.positionCount; i++)
        {
            var n = i;
            var p = l.GetPosition(n);
            var m = chhadle.PositionHandle(p, Quaternion.identity, (x) =>
            {
                var pn = l.GetPosition(n);
                pn += x;
                l.SetPosition(n, pn);
                prop.Flush(new int[] { n * 3, n * 3 + 1, n * 3 + 2 }, pn.x, pn.z, pn.y);
            });
            m.MultiAxisSquareSize = 0;
            m.AxisLength = 2;
            m.SetAxisVisibility(true, 0);
            m.SetAxisVisibility(true, 1);
            m.SetAxisVisibility(true, 2);
            ligizmo.Add(m);
            prop.GetV3(n.ToString(), p.x, p.z, p.y, (x) =>
            {
                var pn = l.GetPosition(n);
                pn.x = x;
                l.SetPosition(n, pn);
                m.transform.position = pn;
            }, (y) =>
            {
                var pn = l.GetPosition(n);
                pn.z = y;
                l.SetPosition(n, pn);
                m.transform.position = pn;
            }, (z) =>
            {
                var pn = l.GetPosition(n);
                pn.y = z;
                l.SetPosition(n, pn);
                m.transform.position = pn;
            });
        }
        prop.GetColor("开", e.Oncolor, (x) =>
        {
            e.SetOnc(x);
            onchangeC?.Invoke(x);
        });
        prop.GetColor("关", e.Offcolor, (x) =>
        {
            e.SetOffc(x);
        });
        chooseTr = l.transform;
        prop.GetEntity("绑定", e.Entity_id, (x) =>
        {
            e.SetEntity(x);
        });
        prop.Show(true);
    }

}