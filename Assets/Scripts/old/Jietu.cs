using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[ExecuteAlways]
public class Jietu : MonoBehaviour
{
    public Camera ca;
    public RectTransform rect;
    public string path;

    private void Awake()
    {
        if (!ca)
        {
            ca = FindObjectOfType<Camera>();
        }
    }

    public void StartJT(string f)
    {
        path = f;
        CaptureScreenshot2(rect.rect);
    }


    /// <summary>
    /// Captures the screenshot2.
    /// </summary>
    /// <returns>The screenshot2.</returns>
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>
    Texture2D CaptureScreenshot2(Rect rect)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);//先创建一个的空纹理，大小可根据实现需要来设置
#pragma warning disable UNT0017 // SetPixels invocation is slow
        screenShot.ReadPixels(rect, 0, 0);//读取屏幕像素信息并存储为纹理数据，
#pragma warning restore UNT0017 // SetPixels invocation is slow
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();//然后将这些纹理数据，成一个png图片文件

        string filename = path;
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张图片: {0}", filename));
        //最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。
        return screenShot;
    }
}

[CustomEditor(typeof(Jietu))]
public class JietuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("截图"))
        {
            var f = EditorUtility.SaveFilePanel("保存图片", System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "图片", ".png");
            var jt = target as Jietu;
            jt.rect.gameObject.SetActive(false);
            (target as Jietu).StartJT(f);
            jt.rect.gameObject.SetActive(true);
        }
    }
}
#endif