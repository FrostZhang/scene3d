using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraControllerForUnity : MonoBehaviour
{
    public static CameraControllerForUnity Instance;
    public Transform followtarget;
    [Header("接受unity输入")]
    public bool cameraRot = true;
    public bool cameraMove = true;
    public bool cameraZoom = true;
    public bool reversalXY = false;
    //public bool useCustomMouse_xy = false;
    public bool enableUIInput = false;
    public int pc_mouseRot = 1;
    public bool roOrMove = true;
    [Header("最小视距")]
    public float minFarClipPlane = 5000;
    [Header("第三 设定距离")]
    public float curdistance;
    public float targetdis;
    [Header("正交")]
    private float curorthographicSize;
    public float targetorthographicSize;
    public Mode mode = Mode.third;
    [Header("第三人称镜头角度 注意xy是反的")]
    public float xAngle, yAngle;
    float xspeed = 5;   //放大系数
    float yspeed = 5;
    public Vector2 thirdLimitAngleY = new Vector2(-45, 90);
    public Vector2 firstLimitAngleY = new Vector2(-45, 45);
    public Vector2 mobaLimitDistance = new Vector2(2, 500);
    public Vector2 orthographicSizeLimit = new Vector2(2, 500);
    public Vector3 offset;

    [Header("第一人称fov")]
    public float targetfov;
    public Vector2 fovlimit = new Vector2(23, 70);

    [Header("第一三 视距平滑"), Range(0, 1)]
    public float widenSmooth = 0.2f;
    [Header("旋转平滑"), Range(0, 1)]
    public float roSmooth = 0.1f;
    public bool autoRotate;

    public Transform focus;
    Quaternion rot;
    Quaternion pivotRot;

    float normalnearClipPlane;
    protected Camera came;
    private Transform ca;
    public Transform CaTr { get => ca; }
    private Vector3 triniPivot;
    protected virtual void Awake()
    {
        Instance = this;
        triniPivot = transform.rotation.eulerAngles;
        yAngle = triniPivot.x;
        xAngle = triniPivot.y;
        came = GetComponentInChildren<Camera>();
        ca = came.transform;
        ca.LookAt(transform.position);
        targetdis = curdistance = Vector3.Distance(transform.position, ca.position);
        targetfov = came.fieldOfView;
        normalnearClipPlane = came.nearClipPlane;
        curorthographicSize = targetorthographicSize = came.orthographicSize;

    }

    float mousex;
    float mousey;
    float mousez;
    float checkAutoRo = 0;
    const float checkInterval = 4;
    Vector3 lastMousepos;


    void LateUpdate()
    {
        bool isOnUI = VerifyUI();
        if (isOnUI && !enableUIInput)
        {
            mousex = 0;
            mousey = 0;
            mousez = 0;
        }
        else
        {
            //if (useCustomMouse_xy)
            //{
            //    var offset = (Input.mousePosition - lastMousepos) * 50;
            //    lastMousepos = Input.mousePosition;
            //    mousex = offset.x / Screen.safeArea.width;
            //    mousey = offset.y / Screen.safeArea.height;
            //}
            //else
            Entry();
        }

        #region 视角缩进，拉远，跟随
        CameraZoom();
        #endregion

        #region 旋转
        CameraRotatePC();
        CameraRotateTouch();
        CameraRotate();
        #endregion

        #region 视角平移
        CameraMovePC();
        CameraMoveTO();
        #endregion
    }

    private void Entry()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null)
        {
            var tcs = Touchscreen.current.touches;
            if (tcs.Count == 1)
            {
                var t = tcs[0];
                if (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    var p = t.delta.ReadValue();
                    mousex = p.x * Time.deltaTime;
                    mousey = p.y * Time.deltaTime;
                }
            }
            else if (tcs.Count == 2)
            {
                if (cameraZoom)
                    Touch2Zoom();
            }
        }
        else
        {
            var mp = Mouse.current.delta.ReadValue();
            mousex = mp.x * Time.deltaTime * 2;//对标旧版本
            mousey = mp.y * Time.deltaTime * 2;//对标旧版本
            if (cameraZoom)
                mousez = Mathf.Clamp(Mouse.current.scroll.ReadValue().y, -0.1f, 0.1f);//对标旧版本
        }
#else
        if (Input.touchCount == 1)
        {
            var t = Input.GetTouch(0);
            if (t.phase == UnityEngine.TouchPhase.Moved)
            {
                var p = t.deltaPosition;
                mousex = p.x * Time.deltaTime;
                mousey = p.y * Time.deltaTime;
            }
        }
        else
        {
            mousex = Input.GetAxis("Mouse X");
            mousey = Input.GetAxis("Mouse Y");
        }

        if (Input.touchCount != 0)
        {
            if (cameraZoom)
                Touch2Zoom();
        }
        else
        {
            if (cameraZoom)
                mousez = Input.GetAxis("Mouse ScrollWheel");
        }
#endif
    }

    private bool VerifyUI()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null)
        {
            var tcs = Touchscreen.current.touches;
            if (tcs.Count > 0)
            {
                foreach (var item in Input.touches)
                {
                    if (EventSystem.current.IsPointerOverGameObject(item.fingerId))
                    {
                        if (item.phase != UnityEngine.TouchPhase.Moved)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
        }
#else
        if (Input.touchCount > 0)
        {
            foreach (var item in Input.touches)
            {
                if (EventSystem.current.IsPointerOverGameObject(item.fingerId))
                {
                    if (item.phase != UnityEngine.TouchPhase.Moved)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
#endif
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    Vector3 oP1, oP2;
    private void CameraZoom()
    {
        if (mode == Mode.third || mode == Mode.moba)
        {
            if (mousez != 0)
            {
                if (!came.orthographic)
                {
                    //相机离的越远越快
                    targetdis = Mathf.Clamp(targetdis + mousez * (targetdis * 0.66f + 6.6f), mobaLimitDistance.x, mobaLimitDistance.y);
                    SetFarClipPlane(targetdis * 40.5f + 240f);
                    if (focus)  //改变焦点
                    {
                        var sc = targetdis * .03f;
                        focus.localScale = new Vector3(sc, sc, sc);
                    }
                }
                else
                {
                    targetorthographicSize = Mathf.Clamp(targetorthographicSize +
                        mousez * (targetorthographicSize * 0.22f + 2.2f), orthographicSizeLimit.x, orthographicSizeLimit.y);
                    SetFarClipPlane(targetorthographicSize + 250f);
                    if (focus)  //改变焦点
                    {
                        var sc = targetorthographicSize * .03f;
                        focus.localScale = new Vector3(sc, sc, sc);
                    }
                }
            }
            //结算 相机与物体的距离
            if (came.orthographic)
            {
                //正交模式 改变size 
                came.orthographicSize = curorthographicSize = Mathf.Lerp(came.orthographicSize, targetorthographicSize, roSmooth);
                ca.position = transform.position - ca.forward.normalized * targetdis;
            }
            else
            {
                //广角模式 改变距离
                targetdis = Mathf.Clamp(targetdis, mobaLimitDistance.x, mobaLimitDistance.y);
                curdistance = Mathf.Lerp(curdistance, targetdis, 5 * 0.02f);
                ca.position = transform.position - ca.forward.normalized * curdistance;
            }

            if (followtarget)
            {
                //跟随
                transform.position = Vector3.Lerp(transform.position, followtarget.position + offset, widenSmooth);
            }
        }
        else if (mode == Mode.first)
        {
            //第一人称跟随
            if (followtarget)
            {
                transform.position = followtarget.position;
                //第一视角锁死下  可变fov
                targetfov += mousez * 3;
                targetfov = Mathf.Clamp(targetfov, fovlimit.x, fovlimit.y);
            }
            //else
            //{
            //    transform.position -= (transform.forward).normalized * mousez * 3;
            //}
            ca.position = transform.position + offset;   //camera is too far with people . fixed the dis;
        }
        //相机广角fov 平滑
        came.fieldOfView = Mathf.Lerp(came.fieldOfView, targetfov, 0.2f);
    }

    int touch1ID = -1, touch2ID = -1, touch1Index;

    Vector2 oldPos1;
    Vector2 oldPos2;
    bool istouch1, istouch2;
    public void Touch2Zoom()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current == null) return;
        var tcs = Touchscreen.current.touches;
        if (tcs.Count != 2)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            return;
        }
        istouch2 = true;
        var tc1 = tcs[0];
        var tc2 = tcs[0];
        var phase1 = tc1.phase.ReadValue();
        var phase2 = tc2.phase.ReadValue();
        var pos1 = tc1.position.ReadValue();
        var pos2 = tc2.position.ReadValue();
        if (touch1ID == -1 || touch2ID == -1)
        {
            touch1ID = tc1.touchId.ReadValue();
            touch2ID = tc2.touchId.ReadValue();
        }
        else
        {
            if (touch1ID != tc1.touchId.ReadValue() || touch2ID != tc2.touchId.ReadValue())
            {
                //id不同 不执行 
                touch1ID = -1;
                touch2ID = -1;
                istouch1 = false;
                istouch2 = false;
                return;
            }
        }

        //双指操作
        if (tc2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            oldPos1 = tc1.position.ReadValue();
            oldPos2 = tc2.position.ReadValue();
            return;
        }

        if (phase1 == UnityEngine.InputSystem.TouchPhase.Moved ||
            phase2 == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            float oldDistance = Vector2.Distance(oldPos1, oldPos2);  //计算原先两指的距离
            float newDistance = Vector2.Distance(pos1, pos2);  //当前移动后两指的距离

            //（新距离）减去（旧距离）得出的差如果是负数的话缩小，正数就放大
            float offset = newDistance - oldDistance;
            mousez = -offset / 100;
            //记住最新的触摸点位置，下次使用  
            oldPos1 = pos1;
            oldPos2 = pos2;
        }

        if (phase1 == UnityEngine.InputSystem.TouchPhase.Ended || phase2 == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            Debug.Log("手指抬起2");
        }

        if (phase1 == UnityEngine.InputSystem.TouchPhase.Canceled || phase2 == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            Debug.Log("系统取消对触摸的跟踪2");
        }
#else
        if (Input.touchCount != 2)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            return;
        }
        istouch2 = true;

        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        //判断id是否相同
        if (touch1ID == -1 || touch2ID == -1)
        {
            touch1ID = touch1.fingerId;
            touch2ID = touch2.fingerId;
        }
        else
        {
            if (touch1ID != touch1.fingerId || touch2ID != touch1.fingerId)
            {
                //id不同 不执行 
                touch1ID = -1;
                touch2ID = -1;
                istouch1 = false;
                istouch2 = false;
                return;
            }
        }

        //双指操作
        if (touch2.phase == UnityEngine.TouchPhase.Began)
        {
            oldPos1 = touch1.position;
            oldPos2 = touch2.position;
            return;
        }

        if (touch1.phase == UnityEngine.TouchPhase.Moved || touch2.phase == UnityEngine.TouchPhase.Moved)
        {
            float oldDistance = Vector2.Distance(oldPos1, oldPos2);  //计算原先两指的距离
            float newDistance = Vector2.Distance(touch1.position, touch2.position);  //当前移动后两指的距离

            //（新距离）减去（旧距离）得出的差如果是负数的话缩小，正数就放大
            float offset = newDistance - oldDistance;
            mousez = -offset / 100;
            //记住最新的触摸点位置，下次使用  
            oldPos1 = touch1.position;
            oldPos2 = touch2.position;
        }

        if (touch1.phase == UnityEngine.TouchPhase.Ended || touch2.phase == UnityEngine.TouchPhase.Ended)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            Debug.Log("手指抬起2");
        }

        if (touch1.phase == UnityEngine.TouchPhase.Canceled || touch2.phase == UnityEngine.TouchPhase.Canceled)
        {
            touch1ID = -1;
            touch2ID = -1;
            istouch1 = false;
            istouch2 = false;
            Debug.Log("系统取消对触摸的跟踪2");
        }
#endif
    }

    private void CameraMovePC()
    {
#if ENABLE_INPUT_SYSTEM
        if ((Touchscreen.current != null && Touchscreen.current.touches.Count > 0) || !cameraMove)
            return;
        bool mbtn0 = Mouse.current.leftButton.isPressed;
        bool mbtn2 = Mouse.current.middleButton.isPressed;
#else
        if (Input.touchCount > 0 || !cameraMove)
            return;
        bool mbtn0 = Input.GetMouseButton(0);
        bool mbtn2 = Input.GetMouseButton(2);
#endif
        if ((mbtn0 && !roOrMove) || mbtn2)
        {
            if (reversalXY)
            {
                mousex = -mousex;
                mousey = -mousey;
            }
            if (mode == Mode.third)
            {
                Vector3 move = new Vector3(mousex, mousey, 0) * curdistance * 0.02f;
                transform.Translate(move, Space.Self);
                if (followtarget)
                {
                    offset = transform.position - followtarget.position;
                }
            }
            else
            {
                if (came.orthographic)
                    transform.Translate(new Vector3(curorthographicSize * 0.02f * mousex, curorthographicSize * 0.02f * mousey, 0), Space.Self);
                else
                    transform.Translate(new Vector3(curdistance * 0.02f * mousex, curdistance * 0.02f * mousey, 0), Space.Self);
                if (followtarget)
                    offset = transform.position - followtarget.position;
            }
        }
    }

    private void CameraMoveTO()
    {
        if (roOrMove || !cameraMove)
            return;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current == null || Touchscreen.current.touches.Count != 1)
            return;
#else
        if (Input.touchCount != 1)
            return;
#endif
        if (reversalXY)
        {
            mousex = -mousex;
            mousey = -mousey;
        }
        if (mode == Mode.third)
        {
            Vector3 move = new Vector3(mousex, mousey, 0) * curdistance * 0.03f;
            transform.Translate(move, Space.Self);
            if (followtarget)
            {
                offset = transform.position - followtarget.position;
            }
        }
        else if (mode == Mode.moba)
        {
            if (came.orthographic)
                transform.Translate(new Vector3(curorthographicSize * 0.02f * mousex, curorthographicSize * 0.02f * mousey, 0), Space.Self);
            else
                transform.Translate(new Vector3(curdistance * 0.02f * mousex, curdistance * 0.02f * mousey, 0), Space.Self);
            if (followtarget)
                offset = transform.position - followtarget.position;
        }
    }

    private void CameraRotatePC()
    {
        if (!roOrMove || !cameraRot)
            return;
#if ENABLE_INPUT_SYSTEM
        bool btn = pc_mouseRot == 0 ? Mouse.current.leftButton.isPressed : Mouse.current.rightButton.isPressed;
#else
        bool btn = Input.GetMouseButton(pc_mouseRot);
#endif
        if (btn)
        {
            if (mode == Mode.third)
            {
                xAngle += mousex * xspeed;
                yAngle -= mousey * yspeed;
                yAngle = Mathf.Clamp(yAngle, thirdLimitAngleY.x, thirdLimitAngleY.y);
            }
            else if (mode == Mode.first)
            {
                xAngle -= mousex * xspeed;
                yAngle += mousey * yspeed;
                yAngle = Mathf.Clamp(yAngle, firstLimitAngleY.x, firstLimitAngleY.y);
            }
        }
    }

    private void CameraRotate()
    {
        float smooth = roSmooth;
        if (autoRotate && followtarget)
        {
#if ENABLE_INPUT_SYSTEM
            bool btn = pc_mouseRot == 0 ? Mouse.current.leftButton.isPressed : Mouse.current.rightButton.isPressed;
#else
            bool btn = Input.GetMouseButton(pc_mouseRot);
#endif
            if (btn)
            {
                checkAutoRo = checkInterval;
            }
            else if ((checkAutoRo -= Time.deltaTime) < 0)
            {
                smooth = 1;
                if (checkAutoRo > -2)
                {
                    if (mode == Mode.third)
                    {
                        //Quaternion rot = transform.rotation;
                        Quaternion toTarget = Quaternion.LookRotation(followtarget.forward);
                        var to = (Quaternion.Euler(0, -triniPivot.y, 0) * toTarget).eulerAngles.y;
                        xAngle = to;
                    }
                    else if (mode == Mode.first)
                    {
                        var e = Quaternion.LookRotation(followtarget.forward);
                        xAngle = Mathf.LerpAngle(xAngle, e.eulerAngles.y, roSmooth);
                        yAngle = Mathf.LerpAngle(yAngle, e.eulerAngles.x, roSmooth);
                    }
                }
                else
                {
                    checkAutoRo = -2;
                    if (mode == Mode.third)
                    {
                        //Quaternion rot = transform.rotation;
                        Quaternion toTarget = Quaternion.LookRotation(followtarget.forward);
                        var to = (Quaternion.Euler(0, -triniPivot.y, 0) * toTarget).eulerAngles.y;
                        xAngle = to;
                    }
                    else if (mode == Mode.first)
                    {
                        var e = Quaternion.LookRotation(followtarget.forward);
                        xAngle = e.eulerAngles.y;
                        yAngle = e.eulerAngles.x;
                    }
                }
            }
        }

        rot = Quaternion.Euler(0f, xAngle, 0f);
        pivotRot = Quaternion.Euler(yAngle, triniPivot.y, triniPivot.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot * pivotRot, smooth);
    }

    private void CameraRotateTouch()
    {
        if (!roOrMove || !cameraRot)
            return;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current == null || Touchscreen.current.touches.Count != 1) return;
#else
        if (Input.touchCount != 1)
            return;
#endif
        if (mode == Mode.third)
        {
            xAngle += mousex * xspeed;
            yAngle -= mousey * yspeed;
            yAngle = Mathf.Clamp(yAngle, thirdLimitAngleY.x, thirdLimitAngleY.y);
        }
        else if (mode == Mode.first)
        {
            xAngle -= mousex * xspeed;
            yAngle += mousey * yspeed;
            yAngle = Mathf.Clamp(yAngle, firstLimitAngleY.x, firstLimitAngleY.y);
        }
    }

    private void SetFarClipPlane(float f)
    {
        came.farClipPlane = Mathf.Max(minFarClipPlane, f);
    }

    public void MobaFollow_orthogonal(Transform target, Vector2 angle, float dis, float size)
    {
        mode = Mode.moba;
        followtarget = target;
        xAngle = angle.y;
        yAngle = angle.x;
        targetfov = 60;
        widenSmooth = 1;
        offset = Vector3.zero;
        came.nearClipPlane = normalnearClipPlane;
        came.orthographic = true;
        targetdis = Mathf.Clamp(dis, mobaLimitDistance.x, mobaLimitDistance.y);
        targetorthographicSize = size < 1 ? 1 : size;
    }

    public void SetRoSpeed(float speed)
    {
        xspeed = speed;
        yspeed = speed;
    }

    public void MobaFollow(Transform target, Vector2 angle, float dis)
    {
        mode = Mode.moba;
        came.orthographic = false;
        followtarget = target;
        xAngle = angle.y;
        yAngle = angle.x;
        targetdis = Mathf.Clamp(dis, mobaLimitDistance.x, mobaLimitDistance.y);
        targetfov = 60;
        widenSmooth = 1;
        offset = Vector3.zero;
        came.nearClipPlane = normalnearClipPlane;
        SetFarClipPlane(targetdis * 40.5f + 240f);
    }

    public void Mobafocus(Vector3 pos, Vector2 angle, float dis)
    {
        mode = Mode.moba;
        came.orthographic = false;
        followtarget = null;
        transform.position = pos;
        xAngle = angle.y;
        yAngle = angle.x;
        targetdis = Mathf.Clamp(dis, mobaLimitDistance.x, mobaLimitDistance.y);
        targetfov = 60;
        widenSmooth = 0.2f;
        offset = Vector3.zero;
        came.nearClipPlane = normalnearClipPlane;
        SetFarClipPlane(targetdis * 40.5f + 240f);
    }

    public void Mobafocus(Vector2 angle, float dis)
    {
        Mobafocus(transform.position, angle, dis);
    }

    public void Thirdfocus(Vector3 pos, Vector2 angle, float dis)
    {
        mode = Mode.third;
        came.orthographic = false;
        followtarget = null;
        transform.position = pos;
        xAngle = angle.y;
        yAngle = angle.x;
        targetdis = Mathf.Clamp(dis, mobaLimitDistance.x, mobaLimitDistance.y);
        targetfov = 60;
        widenSmooth = 0.2f;
        offset = Vector3.zero;
        came.nearClipPlane = normalnearClipPlane;
        SetFarClipPlane(targetdis + 250f);
    }

    public void Thirdfocus(Vector2 angle, float dis)
    {
        Thirdfocus(transform.position, angle, dis);
    }

    public void ThirdFollow(Transform target, Vector2 angle, float dis)
    {
        mode = Mode.third;
        came.orthographic = false;
        followtarget = target;
        xAngle = angle.y;
        yAngle = angle.x;
        targetdis = Mathf.Clamp(dis, mobaLimitDistance.x, mobaLimitDistance.y);
        targetfov = 60;
        //紧跟物体，不平滑，防止物体走的时候相机抖动。
        widenSmooth = 1f;
        offset = Vector3.zero;
        came.nearClipPlane = normalnearClipPlane;
        SetFarClipPlane(targetdis + 250f);
    }

    public void Firstfocus(Vector3 pos, Vector2 angle)
    {
        mode = Mode.first;
        came.orthographic = false;
        followtarget = null;
        transform.position = pos;
        xAngle = angle.y;
        yAngle = angle.x;
        targetfov = 60;
        widenSmooth = 1f;
        offset = Vector3.zero;
        came.nearClipPlane = 0.3f;
        SetFarClipPlane(minFarClipPlane);
    }

    public void FirstFollow(Transform target, Vector2 angle)
    {
        mode = Mode.first;
        came.orthographic = false;
        followtarget = target;
        targetfov = 60;
        xAngle = angle.y;
        yAngle = angle.x;
        //紧跟物体，不平滑，防止物体走的时候相机抖动。
        widenSmooth = 1f;
        offset = Vector3.zero;
        came.nearClipPlane = 0.3f;
        SetFarClipPlane(minFarClipPlane);
    }

    public enum Mode
    {
        third,
        first,
        moba
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(CameraControllerForUnity))]
public class CameraControllerE : Editor
{
    Transform targetforca;
    Vector2 angle;
    float dis;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("------------   以下为测试   ------------");
        targetforca = EditorGUILayout.ObjectField("target", targetforca, typeof(Transform), true) as Transform;
        angle = EditorGUILayout.Vector2Field("angle", angle);
        dis = EditorGUILayout.FloatField("dis", dis);
        if (GUILayout.Button("第三人称切换"))
        {
            (target as CameraControllerForUnity).ThirdFollow(targetforca, angle, dis);
        }
        if (GUILayout.Button("第一人称切换"))
        {
            (target as CameraControllerForUnity).FirstFollow(targetforca, angle);
        }
        if (GUILayout.Button("moba切换"))
        {
            (target as CameraControllerForUnity).MobaFollow(targetforca, angle, dis);
        }
        if (GUILayout.Button("moba正交切换"))
        {
            (target as CameraControllerForUnity).MobaFollow_orthogonal(targetforca, angle, dis, dis);
        }
        Undo.RecordObject(target, target.name);
    }
}

#endif