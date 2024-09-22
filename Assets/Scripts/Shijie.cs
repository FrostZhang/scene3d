using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Shijie : MonoBehaviour
{
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLink3DStart();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLink3DClickMessage(string mes);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLink3DLongClickMessage(string mes);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLink3DWebLog(string mes);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLink3DConfig(string mes);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void WebglAsherLinkUIConfig(string mes);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern float WebglHelloFloat();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern void LoadFile(string gameobjectName, string fitter);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern string WebglGetHass();

    public static Action<string> OnGetConfig;
    public static Action<string> OnHassFlush;
    public static string domain;
    float lastUpdateT;
    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Shijie.Link3DStart();
        WebGLInput.captureAllKeyboardInput = false;
#endif
        var path = Application.streamingAssetsPath;
        var index = path.IndexOf('/', 8);
        if (index != -1)
        {
            domain = path.Remove(index);
            Debug.Log(domain);
        }
    }

    public void HassConfig(string configStr)
    {
        OnGetConfig.Invoke(configStr);
    }

    public void HassUpdate(string hassmeg)
    {
        //var res = WebglGetHass();
        //if (res != null)
        {
            OnHassFlush?.Invoke(hassmeg);
        }
    }

    public static void Link3DStart()
    {
#if UNITY_EDITOR
        Debug.Log("Link3DStart");
#else
         WebglAsherLink3DStart();
#endif
    }

    public static void ClickMessage(string mes)
    {
#if UNITY_EDITOR
        Debug.Log(mes);
#else
         WebglAsherLink3DClickMessage(mes);
#endif
    }

    public static void LongClickMessage(string mes)
    {
#if UNITY_EDITOR
        Debug.Log(mes);
#else
         WebglAsherLink3DLongClickMessage(mes);
#endif
    }

    public static void Link3DWebLog(string mes)
    {
#if UNITY_EDITOR
        Debug.Log(mes);
#else
         WebglAsherLink3DWebLog(mes);
#endif
    }

    public static void Config3D(string mes)
    {
#if UNITY_EDITOR
        Debug.Log(mes);
#else
         WebglAsherLink3DConfig(mes);
#endif
    }

    public static void ConfigUI(string mes)
    {
#if UNITY_EDITOR
        Debug.Log(mes);
#else
         WebglAsherLinkUIConfig(mes);
#endif
    }

    public static string GetHass()
    {
#if UNITY_EDITOR
        Debug.Log("get hass back");
        return null;
#else
        return WebglGetHass();
#endif
    }

    public static float WhichStand()
    {
#if UNITY_EDITOR
        return 0;
#else
        return  WebglHelloFloat();
#endif
    }

    public static void LoadWebFile(string gameobjectName, string fitter)
    {
#if UNITY_EDITOR
        Debug.Log("请求载入本地资源");
        return;
#else
        LoadFile(gameobjectName, fitter);
#endif
    }
}

public class HssConfig
{
    public string canting;
    public string keting;
    public string fuwo;
    public string zhuwo;
    public string xiaowoshi;
    public string cufang;
    public string guodao;
    public string weishenjian;
}

public class HassServerMessage
{
    public string head;
    public string cmd;
    public string entity_id;
}

public class HassMoreInfo
{
    public string entity_id;
}