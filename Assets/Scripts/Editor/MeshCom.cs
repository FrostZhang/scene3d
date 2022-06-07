using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshCombin
{

    [MenuItem("Plugins/合并选择mesh")]
    public static void CombineMesh()
    {
        GameObject[] objs = Selection.gameObjects;
        for (int j = 0; j < objs.Length; j++)
        {
            MeshFilter[] meshfilters = objs[j].GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshfilters.Length];

            //世界转局部坐标的矩阵，乘以世界坐标可以得到局部坐标
            Matrix4x4 matrix = objs[j].transform.worldToLocalMatrix;

            for (int i = 0; i < meshfilters.Length; i++)
            {
                MeshFilter mf = meshfilters[i];

                MeshRenderer mr = mf.GetComponent<MeshRenderer>();

                if (mr == null) continue;

                combine[i].mesh = mf.sharedMesh;

                //子mesh坐标转父mesh坐标的矩阵
                //先通过子mesh转世界矩阵，得到世界坐标，然后通过世界转父mesh坐标的矩阵，得到父mesh坐标
                combine[i].transform = matrix * mf.transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();

            mesh.name = objs[j].name;

            mesh.CombineMeshes(combine, true);

            string path = @"Assets/Art/Mode/" + mesh.name + ".asset";

            AssetDatabase.CreateAsset(mesh, path);
        }
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("CombineMesh", "Combine successfully!", "OK", "");
    }
}