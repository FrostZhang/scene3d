using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Reconstitution : MonoBehaviour
{
    public static Reconstitution Instance;
    public bool IsEdit { get => isEdit; }
    public Action<Transform> ChangeEditorTarget;
    private bool isEdit;
    Camera mainCa;
    Transform rigTr;
    Transform cs;
    private RaycastHit hit;
    private float maxDistance = 25;
    private Transform lasthit;
    CHHandle chhadle;
    List<RTEditor.Gizmo> ligizmo;
    Vector3 offset = new Vector3(1, 0, 1) * 0.125f;
    void Start()
    {
        Instance = this;
        mainCa = Camera.main;

        chhadle = new CHHandle();
        //caTr = mainCa.transform;
        rigTr = CameraControllerForUnity.Instance.transform;
        ligizmo = new List<RTEditor.Gizmo>(5);
    }

    private void OnDelete()
    {
        CameraControllerForUnity.Instance.cameraMove = true;
        ClearHandle();
        House.Instance.Delete(lasthit);
        PropPanle.Instance.Show(false);
        lasthit = null;
    }

    public void OpenRTEditor()
    {

    }

    public void CloseRTEditor()
    {
        PropPanle.Instance.Show(false);
        lasthit = null;
        ClearHandle();
    }

    public void EditScene(bool edit)
    {
        isEdit = edit;
        if (!edit)
        {
            CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
            cs = House.Instance.entities;
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
            CloseRTEditor();
            Surface.surface.RemovePanel<EditScenePanel>();
            if (PropPanle.Instance)
                Destroy(PropPanle.Instance.gameObject);
            Surface.surface.LoadPanel<HousePanel>();
            Resources.UnloadUnusedAssets();
        }
        else
        {
            Surface.surface.LoadPanel<EditScenePanel>();
            var t = Resources.Load<Transform>("UI/PropPanle");
            Instantiate(t, House.Instance.canvas);
            PropPanle.Instance.Ondel += OnDelete;
            rigTr.position = new Vector3(0, -1, 0);
            //CameraControllerForUnity.Instance.MobaFollow_orthogonal(rigTr, new Vector2(90, 0), 15, 5);
            //CameraControllerForUnity.Instance.orthographicSizeLimit = new Vector2(2, 10);

            //通知所有单位进入编辑模式
            cs = GetComponent<House>().entities;
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
            OpenRTEditor();
        }
    }

    RaycastHit[] jiances;
    float interval;
    void Update()
    {
        if (!isEdit || EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var item in ligizmo)
            {
                if (item.IsReadyForObjectManipulation())
                {
                    CameraControllerForUnity.Instance.cameraMove = false;
                    return;
                }
            }
            if (Physics.Raycast(mainCa.ScreenPointToRay(Input.mousePosition), out hit, maxDistance))
            {
                if (hit.transform != lasthit)
                {
                    lasthit = hit.transform;
                    ClearHandle();
                    SelectTr(lasthit);
                }
            }
            else
            {
                ClearHandle();
                lasthit = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CameraControllerForUnity.Instance.cameraMove = true;
        }
    }

    private void ClearHandle()
    {
        foreach (var item in ligizmo)
            chhadle.ReHandle(item);
        ligizmo.Clear();
    }

    public void EditorTransform(Transform obj)
    {
        if (lasthit != obj)
        {
            lasthit = obj;
            ClearHandle();
            SelectTr(lasthit);
        }
    }

    private void SelectTr(Transform lasthit)
    {
        ChangeEditorTarget?.Invoke(lasthit);
        var h = chhadle.PositionHandle(lasthit.position + offset, Quaternion.identity, (x) =>
             {
                 lasthit.position += x;
                 FlushPropPanle();
             });
        h.SetAxisVisibility(false, 1);
        h.AxisLength = 2.5f;
        ligizmo.Add(h);
        var s = chhadle.VolumeHandle(lasthit.gameObject, (a, b) =>
         {
             lasthit.localScale = a;
             lasthit.position = b;
             h.transform.position = lasthit.position + offset;
             FlushPropPanle();
         });
        s.SetAxisVisibility(false, 1);
        ligizmo.Add(s);
        ShowDetail(lasthit);
    }

    HouseEntityType cureetET;
    void ShowDetail(Transform target)
    {
        if (House.Instance.CureetHouse.ContainsKey(target))
        {
            var t = House.Instance.CureetHouse[target];
            cureetET = t.entityType;
            switch (cureetET)
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
                    PropPanle.Instance.Show(false);
                    break;
            }
        }
    }

    private void ShowAnimal(Transform target)
    {
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV2("位置", target.position.x, target.position.z,
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
        PropPanle.Instance.GetV2("区域", target.localScale.x, target.localScale.z,
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
        PropPanle.Instance.Show(true);
    }

    private void FlushPropPanle()
    {
        switch (cureetET)
        {
            case HouseEntityType.wall:
                var p = lasthit.position;
                var sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 3, 4, 5 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.floor:
                p = lasthit.position;
                sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 3 }, p.x, p.z, sc.x, sc.z);
                break;
            case HouseEntityType.door:
                p = lasthit.position;
                sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 3, 4, 5 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.stand:
                p = lasthit.position;
                sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 6, 7, 8 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
                break;
            case HouseEntityType.sky:
                break;
            case HouseEntityType.arealight:
                p = lasthit.position;
                sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 3 }, p.x, p.z, sc.x, sc.z);
                break;
            case HouseEntityType.appliances:
                p = lasthit.position;
                sc = lasthit.localScale;
                PropPanle.Instance.Flush(new int[] { 0, 1, 2, 6, 7, 8 }, p.x, p.z, p.y, sc.x, sc.y, sc.z);
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
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV3("位置", target.position.x, target.position.z, target.position.y,
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
        PropPanle.Instance.GetV3("角度", target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z,
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
        PropPanle.Instance.GetV3("体积", target.localScale.x, target.localScale.y, target.localScale.z,
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
            PropPanle.Instance.GetColor("颜色", ma.GetColor("_Color"), (x) => ma.SetColor("_Color", x));
            //PropPanle.Instance.GetColor("Edge", ma.GetColor("_HeightLight"), (x) => ma.SetColor("_HeightLight", x));
            //PropPanle.Instance.GetColor("Depth", ma.GetColor("_DepthColor"), (x) => ma.SetColor("_DepthColor", x));
        }
        else if (door is LampEntity)
        {
            var e = door as LampEntity;
            PropPanle.Instance.GetColor("颜色", e.emColor, (x) => e.emColor = x);
        }
        else if (door is TextEntity)
        {
            var e = door as TextEntity;
            PropPanle.Instance.GetText("前缀", e.front, (x) =>
            {
                e.front = x;
                e.FlushText();
            });
            PropPanle.Instance.GetText("后缀", e.back, (x) =>
            {
                e.back = x;
                e.FlushText();
            });
            PropPanle.Instance.GetColor("颜色", e.textMesh.color, (x) => e.textMesh.color = x);
            PropPanle.Instance.GetV1("字号", e.textMesh.fontSize, (x) =>
            {
                e.textMesh.fontSize = (int)x;
            });
        }
        else if (door is CoverEntity)
        {
            var e = door as CoverEntity;
            PropPanle.Instance.GetV1("Open", e.openW, (x) =>
            {
                e.openW = x;
            });
        }
        PropPanle.Instance.GetEntity("ID", door.Entity_id, (x) =>
        {
            door.SetEntity(x);
        });
        PropPanle.Instance.Show(true);
    }

    private void ShowAreaLight(Transform target)
    {
        var door = target.GetComponent<LightEntity>();
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV2("位置", target.position.x, target.position.z, (x) =>
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
        PropPanle.Instance.GetV2("区域", target.localScale.x, target.localScale.z, (x) =>
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
        PropPanle.Instance.GetV1("亮度", door.Max, (x) =>
        {
            door.Max = x;
        });
        PropPanle.Instance.GetColor("颜色", door.clight.color, (x) => door.clight.color = x);
        PropPanle.Instance.GetEntity("绑定", door.Entity_id, (x) =>
        {
            door.SetEntity(x);
        });
        PropPanle.Instance.Show(true);
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
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV2("区域", target.position.x, target.position.z, (x) =>
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
        PropPanle.Instance.GetV2("区域", target.localScale.x * 10f, target.localScale.z * 10f, (x) =>
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
        PropPanle.Instance.GetV2("平铺", ma.GetTextureScale("_BaseMap").x, ma.GetTextureScale("_BaseMap").y, (x) =>
        {
            var s = ma.GetTextureScale("_BaseMap");
            ma.SetTextureScale("_BaseMap", new Vector2(x, s.y));
        }, (x) =>
        {
            var s = ma.GetTextureScale("_BaseMap");
            ma.SetTextureScale("_BaseMap", new Vector2(s.x, x));
        });
        PropPanle.Instance.GetV1("层级", target.position.y * 100f, (x) =>
        {
            var p = target.position;
            p.y = x * 0.01f;
            target.position = p;
        });
        PropPanle.Instance.Show(true);
    }

    private void ShowDoor(Transform target)
    {
        var door = target.GetComponent<DoorEntity>();
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV3("位置", target.position.x, target.position.z, target.position.y, (x) =>
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
        PropPanle.Instance.GetV3("尺寸", target.localScale.x, target.localScale.y, target.localScale.z,
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
        PropPanle.Instance.GetV1("角度关", target.eulerAngles.y, (x) =>
         {
             var pc = target.eulerAngles;
             pc.y = x;
             target.eulerAngles = pc;
         });
        if (door is DoorEntitySliding)
        {

        }
        else
        {
            PropPanle.Instance.GetV1("角度开", door.angleopen, (x) =>
            {
                door.angleopen = x;
            });
        }

        PropPanle.Instance.GetEntity("绑定", door.Entity_id, (x) =>
        {
            door.SetEntity(x);
        });
        PropPanle.Instance.Show(true);
    }

    private void ShowStand(Transform target)
    {
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV3("位置", target.position.x, target.position.z, target.position.y,
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
        PropPanle.Instance.GetV3("角度", target.localEulerAngles.x, target.localEulerAngles.y, target.localEulerAngles.z,
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
        PropPanle.Instance.GetV3("尺寸", target.localScale.x, target.localScale.y, target.localScale.z,
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


        PropPanle.Instance.Show(true);
    }

    private void ShowEye(Transform target)
    {
        ShowStand(target);
        var e = target.GetComponent<Eye>();
        PropPanle.Instance.GetV1("序列", e.priority, (x) => e.priority = (int)x);
        PropPanle.Instance.GetText("标记", e.textMesh.text, (x) => e.textMesh.text = x);
    }

    private void ShowWallProp(Transform target)
    {
        PropPanle.Instance.Clear();
        PropPanle.Instance.GetV3("文字", target.position.x, target.position.z, target.position.y,
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
        PropPanle.Instance.GetV3("长高宽", target.localScale.x, target.localScale.y, target.localScale.z,
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
        PropPanle.Instance.GetV1("角度", target.localEulerAngles.y, (x) =>
        {
            var pc = target.localEulerAngles;
            pc.y = x;
            target.localEulerAngles = pc;
        });
        var r = target.GetComponent<MeshRenderer>().material;
        PropPanle.Instance.GetColor("颜色", r.color, (x) => r.color = x);
        PropPanle.Instance.Show(true);
    }

    /// <summary>
    /// 由于线条不可选中，所以采用点击UI列表展示
    /// </summary>
    public void EditLine(LineEntity e, Action<Color> onchangeC)
    {
        //CameraControllerForUnity.Instance.cameraMove = false;
        ClearHandle();
        PropPanle.Instance.Clear();
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
                PropPanle.Instance.Flush(new int[] { n * 3, n * 3 + 1, n * 3 + 2 }, pn.x, pn.z, pn.y);
            });
            m.MultiAxisSquareSize = 0;
            m.AxisLength = 1;
            m.SetAxisVisibility(false, 1);
            ligizmo.Add(m);
            PropPanle.Instance.GetV3(n.ToString(), p.x, p.z, p.y, (x) =>
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
        PropPanle.Instance.GetColor("开", e.Oncolor, (x) =>
        {
            e.SetOnc(x);
            onchangeC?.Invoke(x);
        });
        PropPanle.Instance.GetColor("关", e.Offcolor, (x) =>
        {
            e.SetOffc(x);
        });
        lasthit = l.transform;
        PropPanle.Instance.GetEntity("绑定", e.Entity_id, (x) =>
        {
            e.SetEntity(x);
        });
        PropPanle.Instance.Show(true);
    }

}
