using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanel
{
    Transform Tr { get; set; }
    void Start();
    void OnDestroy();
}

public class Surface : MonoBehaviour
{
    public Canvas canvas;

    public List<IPanel> panels { get; private set; }
    public static bool IsFullScreen => Screen.fullScreen;
    public static Surface surface;
    private void Awake()
    {
        surface = this;
        panels = new List<IPanel>(10);
    }

    /// <summary>
    /// windows 全屏 无边框  隐藏底栏
    /// </summary>
    public static void FullScreenWindow()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
    }

    /// <summary>
    /// windows 全屏转windows
    /// </summary>
    public static void Windowed()
    {
        Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
    }

    /// <summary>
    /// T 继承mono时，Start由mono调用。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ct"></param>
    /// <returns></returns>
    public T LoadPanel<T>(params string[] uipath) where T : IPanel, new()
    {
        string typename = typeof(T).Name;
        var c = canvas;
        var t = panels.FindIndex((x) => x is T);
        if (t != -1)
        {
            var temp = panels[t];
            if (temp != panels[panels.Count - 1])
            {
                panels[t] = panels[panels.Count - 1];
                panels[panels.Count - 1] = temp;
            }
            var res = (T)temp;
            res.Tr.SetParent(c.transform);
            res.Tr.gameObject.SetActive(true);
            return res;
        }
        else
        {
            string path = "UI";
            if (null != uipath)
            {
                foreach (var item in uipath)
                    path += "/" + item;
            }
            path += "/" + typename;
            var p = Resources.Load<Transform>(path);
            if (!p)
            {
                Debug.LogError("non ui with " + typename);
            }
            Transform tr = Object.Instantiate(p, c.transform);
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                T va = tr.GetComponent<T>();
                if (va != null)
                {
                    va.Tr = tr;
                    //va.Start();
                    panels.Add(va);
                    return va;
                }
            }
            T panel = new T();
            panel.Tr = tr;
            panel.Start();
            panels.Add(panel);
            return panel;
        }
    }

    public T GetPanel<T>() where T : IPanel
    {
        var t = panels.FindIndex((x) => x is T);
        if (t != -1)
        {
            var temp = panels[t];
            if (temp != panels[panels.Count - 1])
            {
                panels[t] = panels[panels.Count - 1];
                panels[panels.Count - 1] = temp;
            }
            var res = (T)temp;
            res.Tr.gameObject.SetActive(true);
            return res;
        }
        return default(T);
    }

    public void RemovePanel<T>() where T : IPanel
    {
        var t = panels.Find(x => x is T);
        if (t != null)
        {
            RemovePanel<T>(t as IPanel);
            panels.Remove(t);
        }
    }

    private void RemovePanel<T>(IPanel t) where T : IPanel
    {
        if (t != null)
        {
            if (!typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                t.OnDestroy();
            }
            Object.Destroy(t.Tr.gameObject);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            RemovePanel<IPanel>(panels[i]);
        }
        panels.Clear();
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
}