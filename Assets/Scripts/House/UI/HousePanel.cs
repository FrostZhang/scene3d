using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousePanel : MonoBehaviour, IPanel
{
    public static HousePanel Instance;
    public StatesBtn changeCamera, roOrMove;
    public Button resetCamera, setting;

    public Transform Tr { get; set; }

    void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        changeCamera.onValueChanged = (x) =>
          {
              if (x == 0)
              {
                  foreach (var item in House.Instance.CureetHouse)
                  {
                      if (item.Value.entityType == HouseEntityType.eye)
                      {
                          var e = item.Key.GetComponent<Eye>();
                          e.Show(false);
                      }
                  }
                  CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
              }
              if (x == 1)
              {
                  CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
                  CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);
              }
              else if (x == 2)
              {
                  List<Eye> eyes = new List<Eye>();
                  foreach (var item in House.Instance.CureetHouse)
                  {
                      if (item.Value.entityType == HouseEntityType.eye)
                      {
                          var e = item.Key.GetComponent<Eye>();
                          e.Show(true);
                          eyes.Add(e);
                      }
                  }
                  if (eyes.Count > 0)
                  {
                      roOrMove.SetValue(0);   //第一人称 不允许平移
                      eyes.Sort((x, y) => x.priority - y.priority);
                      eyes[0].OnMouseUpAsButton();
                  }
                  else
                  {
                      changeCamera.SetValue(0);
                  }
              }
          };

        resetCamera.onClick.AddListener(() =>
        {
            if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.third)
            {
                CameraControllerForUnity.Instance.Thirdfocus(new Vector3(0, -1, 0), new Vector2(45, 0), 12);
            }
            else if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.moba)
            {
                CameraControllerForUnity.Instance.transform.position = new Vector3(0, -1, 0);
                CameraControllerForUnity.Instance.MobaFollow_orthogonal(CameraControllerForUnity.Instance.transform, new Vector2(90, 0), 15, 5);

            }
            else if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.first)
            {

            }
        });
        roOrMove.onValueChanged = (x) =>
            {
                if (CameraControllerForUnity.Instance.mode == CameraControllerForUnity.Mode.first)
                {
                    roOrMove.SetIsOnWithoutNotify(0);
                    return;
                }
                CameraControllerForUnity.Instance.roOrMove = x == 0;
            };
        CameraControllerForUnity.Instance.roOrMove = roOrMove.State == 0;

        setting.onClick.AddListener(() => Surface.surface.LoadPanel<SettingPanel>());
    }

    public void OnDestroy()
    {

    }
}
