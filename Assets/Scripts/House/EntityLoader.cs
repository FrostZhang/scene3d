using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EntityLoader
{
    public Dictionary<Transform, HouseEntityData> CureetHouse { get => cureetHouse; }
    private Dictionary<Transform, HouseEntityData> cureetHouse;
    
}

public class StandProxy
{
    private Dictionary<Transform, HouseEntityData> cureetHouse;
    private Transform parent;
    public StandProxy(Dictionary<Transform, HouseEntityData> cureetHouse,Transform pa)
    {
        this.cureetHouse = cureetHouse;
        this.parent = pa;
    }
    /// <summary>posxzy anglexyz scalexyz </summary>
    public async Task<Transform> CreatStand(string cusname, string str, string color)
    {
        if (string.IsNullOrEmpty(str)) return null;
        var ss = str.Split(',');
        if (ss.Length == 9)
        {
            float va, x, y, z, rx, ry, rz, sx, sy, sz;
            if (float.TryParse(ss[0], out va)) x = va; else return null;
            if (float.TryParse(ss[1], out va)) y = va; else return null;
            if (float.TryParse(ss[2], out va)) z = va; else return null;
            if (float.TryParse(ss[3], out va)) rx = va; else return null;
            if (float.TryParse(ss[4], out va)) ry = va; else return null;
            if (float.TryParse(ss[5], out va)) rz = va; else return null;
            if (float.TryParse(ss[6], out va)) sx = va; else return null;
            if (float.TryParse(ss[7], out va)) sy = va; else return null;
            if (float.TryParse(ss[8], out va)) sz = va; else return null;

            var tr = await CreatStand(cusname, color);
            if (tr)
            {
                tr.transform.position = new Vector3(x, z, y);
                tr.transform.eulerAngles = new Vector3(rx, ry, rz);
                tr.transform.localScale = new Vector3(sx, sy, sz);
            }
        }
        return null;
    }

    /// <summary>posxzy anglexyz scalexyz </summary>
    public async Task<Transform> CreatStand(string cusname, string color)
    {
        var ab = await Help.Instance.Load_Asdata_SP("stand", cusname);
        if (ab == null) return null;
        var tr = ab.LoadAsset<GameObject>(cusname);
        tr = Object.Instantiate(tr, parent);
        tr.name = cusname;
        HouseEntityData data = new HouseEntityData(cusname, HouseEntityType.stand);
        cureetHouse.Add(tr.transform, data);
        var collider = tr.GetComponent<Collider>();
        if (collider) collider.enabled = false;
        return tr.transform;
    }

    public async Task<Transform> CreatStandFromWWW(string url)
    {
        AssetBundle ab = null;
        if (url.StartsWith("http"))
        {
            ab = await Help.Instance.Load_Asdata_www(url, new System.Uri(url));
        }
        else if (url.StartsWith("/"))
        {
            var cusurl = Help.Instance.CombineCustomdataPath(url.Remove(0, 1));
            ab = await Help.Instance.Load_Asdata_www(cusurl, new System.Uri(cusurl));
        }
        if (ab == null) return null;
        var bs = ab.LoadAllAssets();
        if (bs != null && bs.Length > 0)
        {
            var tr = bs[0] as GameObject;
            if (tr)
            {
                tr = Object.Instantiate(tr, parent);
                //tr.name = cusname;
                HouseEntityData data = new HouseEntityData(url);
                cureetHouse.Add(tr.transform, data);
                var collider = tr.GetComponent<Collider>();
                if (collider) collider.enabled = false;
                return tr.transform;
            }
        }
        return null;
    }
}