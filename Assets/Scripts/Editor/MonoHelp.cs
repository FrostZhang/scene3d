using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MonoHelp : EditorWindow
{
    [ContextMenuItem("1244", "Iniaa")]
    public int set;

    public static void CreateMediaPlayerWidthUnityAudioEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierachyGUI;
    }

    private static void OnHierachyGUI(int instanceID, Rect selectionRect)
    {
        var go = Selection.activeGameObject;
        if (go)
        {
            var scripts = go.GetComponents<MonoBehaviour>();
            if (scripts.Length == 0)
                return;
            GUIContent[] contens = new GUIContent[scripts.Length];
            for (int i = 0; i < contens.Length; i++)
            {
                contens[i] = new GUIContent(scripts[i].GetType().Name);
            }
            EditorUtility.DisplayCustomMenu(new Rect(0, 100, 0, 0),
    contens, -1, Select, scripts, true);
        }
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierachyGUI;
    }

    private static void Select(object userData, string[] options, int selected)
    {
        var scripts = userData as MonoBehaviour[];
        var field = scripts[selected].GetType().GetFields();
        StringBuilder sb = new StringBuilder();
        foreach (var item in field)
        {
            var va = item.GetValue(scripts[selected]) as UnityEngine.Object;
            if (va)
            {
                Transform tr = va as Transform;
                if (tr)
                {
                    string path = GetTrPath(scripts[0].transform, tr, string.Empty);
                    sb.AppendLine(item.Name + " = tr.Find(\"" + path + "\").GetComponent<Transform>();");
                }
                MonoBehaviour mono = va as MonoBehaviour;
                if (mono)
                {
                    string path = GetTrPath(scripts[0].transform, mono.transform, string.Empty);
                    sb.AppendLine(item.Name + " = tr.Find(\"" + path + "\").GetComponent<" +
                        item.GetValue(scripts[selected]).GetType().Name + ">();");
                }
            }
        }
        GUIUtility.systemCopyBuffer = sb.ToString();
        EditorUtility.DisplayDialog("复制信息", sb.ToString(), "确认");
    }

    public static string GetTrPath(Transform pa, Transform self, string str)
    {
        str = "/" + self.name + str;
        if (self.parent && self.parent != pa)
        {
            return GetTrPath(pa, self.parent, str);
        }
        else
        {
            return str.Remove(0, 1);
        }
    }


    [MenuItem("GameObject/----Asher----/脚本重映射", false, 1)]
    public static void Mapping()
    {
        var go = Selection.activeGameObject;
        if (go)
        {
            var b = EditorUtility.DisplayDialog("脚本重映射",
    "当脚本信息丢失时，根据脚本自定字段重印射物体，慎用！", "继续", "取消");
            if (!b)
            {
                return;
            }
            var scripts = go.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                var field = script.GetType().GetFields();
                foreach (var item in field)
                {
                    var t = item.FieldType;
                    var n = item.Name;
                    if (!item.IsNotSerialized)
                    {
                        var va = item.GetValue(script) as UnityEngine.Object;
                        if (va)
                        {

                        }
                        else
                        {
                            var c = FindObj(go, n, true);
                            if (c)
                            {
                                if (t.Name != "GameObject")
                                {
                                    var res = c.GetComponent(t.Name);
                                    if (res)
                                    {
                                        item.SetValue(script, res);
                                    }
                                }
                                else
                                {
                                    item.SetValue(script, c.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private static GameObject FindFirstLevelChildObjByName(GameObject go, string childObjName, bool includeInactive = true)
    {
        int childCount = go.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform t = go.transform.GetChild(i);
            if ((includeInactive || !includeInactive && t.gameObject.activeInHierarchy) && t.name.Equals(childObjName))
                return t.gameObject;

            GameObject result = FindFirstLevelChildObjByName(t.gameObject, childObjName, includeInactive);
            if (result)
                return result;
        }

        return null;
    }

    private static GameObject FindObj(GameObject go, string childObjName, bool includeInactive = true)
    {
        if ((includeInactive || !includeInactive && go.activeInHierarchy) && go.name.Equals(childObjName))
            return go;

        return FindFirstLevelChildObjByName(go, childObjName, includeInactive);
    }

    [MenuItem("GameObject/----Asher----/创建\\修改脚本", false, 2)]
    public static void CreatSc()
    {
        var go = Selection.activeGameObject;
        if (go)
        {
            if (!Directory.Exists("Assets/Scripts"))
                Directory.CreateDirectory("Assets/Scripts");
            AssetDatabase.Refresh();
            if (string.IsNullOrWhiteSpace(go.name))
                return;
            select = go;
            trs = new List<TransformEx>();
            GetChildComponent(go.transform);
            GetWindow<MonoHelp>().Show();
        }
    }

    private static void GetChildComponent(Transform tr)
    {
        if ((tr.name[0] >= 'A' && tr.name[0] <= 'Z'))
        {
            //不记录系统默认名字
        }
        else if (tr.hideFlags == HideFlags.None)
        {
            var cs = (from a in tr.GetComponents<Component>()
                      select a.GetType().Name).ToArray();
            trs.Add(new TransformEx() { tr = tr, cs = cs, choose = cs.Length - 1 });
        }
        if (tr.hideFlags == HideFlags.None)
            foreach (Transform item in tr)
            {
                GetChildComponent(item);
            }
    }

    static List<TransformEx> trs;
    static bool hasfile = false;
    static GameObject select;
    Vector2 pos;
    public class TransformEx
    {
        public string[] cs;
        public Transform tr;
        public int choose;
    }

    private void OnGUI()
    {
        if (trs != null && trs.Count > 0)
        {
            EditorGUILayout.ObjectField(select, typeof(GameObject), false);
            using (var sc = new EditorGUILayout.ScrollViewScope(pos))
            {
                pos = sc.scrollPosition;
                for (int i = 0; i < trs.Count; i++)
                {
                    if (trs[i].tr != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(trs[i].tr.name);
                        trs[i].choose = EditorGUILayout.Popup(trs[i].choose, trs[i].cs);
                        EditorGUILayout.ObjectField(trs[i].tr, typeof(Transform), false);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
        if (GUILayout.Button("刷新"))
        {
            CreatSc();
        }
        if (GUILayout.Button("创建/修改脚本"))
        {
            var n = select.name.Substring(0, 1).ToUpper() + select.name.Substring(1);

            var p = $"Assets/Scripts/{n}.cs";
            var info = new FileInfo(p);
            StreamWriter writer = null;

            if (info.Exists)
            {
                writer = info.AppendText();
                hasfile = true;
            }
            else
            {
                writer = info.CreateText();
            }

            StringBuilder sb = new StringBuilder(5000);
            if (!hasfile)
            {
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine("using UnityEngine.UI;");
                sb.AppendLine(string.Empty);
                sb.AppendLine($"public class {n} : MonoBehaviour{{");
            }
            foreach (var item in trs)
            {
                if (item != null)
                {
                    sb.AppendLine($"    public {item.cs[item.choose]} {item.tr.name};");
                }
            }
            if (!hasfile)
            {
                sb.AppendLine("}");
            }
            writer.Write(sb.ToString());
            writer.Flush();
            writer.Dispose();
            writer = null;
            AssetDatabase.ImportAsset(p);
        }
        if (GUILayout.Button("复制自动脚本代码"))
        {
            GUIUtility.systemCopyBuffer = GetTempScript() + GetBingding();
            EditorUtility.DisplayDialog("复制信息", GUIUtility.systemCopyBuffer, "确认");
        }
        GUILayout.Space(20);
    }

    private string GetTempScript()
    {
        var n = select.name.Substring(0, 1).ToUpper() + select.name.Substring(1);
        StringBuilder sb = new StringBuilder(5000);

        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine(string.Empty);
        sb.AppendLine($"public class {n} : MonoBehaviour{{");

        foreach (var item in trs)
        {
            if (item != null)
            {
                sb.AppendLine($"    public {item.cs[item.choose]} {item.tr.name};");
            }
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string GetBingding()
    {
        StringBuilder sb = new StringBuilder(5000);
        sb.AppendLine(string.Empty);
        sb.AppendLine("void Awake(){");
        foreach (var item in trs)
        {
            if (item != null)
            {
                string path = GetTrPath(select.transform, item.tr, string.Empty);
                sb.AppendLine(item.tr.name + " = tr.Find(\"" + path + "\").GetComponent<" +
                    item.cs[item.choose] + ">();");
            }
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
}
