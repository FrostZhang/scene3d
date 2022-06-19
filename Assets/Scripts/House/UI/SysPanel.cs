using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SysPanel : MonoBehaviour
{
    public Button save, weather, addwwwResource;
    public Toggle autoSun;
    public Slider sunIn, ro;
    public GameObject suninpa;
    private void Start()
    {
        save.onClick.AddListener(Save);
        weather.onClick.AddListener(() =>
        {
            ButtonList.Instance.Show(true, (n) =>
            {
                weather.GetComponentInChildren<Text>().text = n;
                HouseWeather.Instance.SetEntity(n);
            });
        });
        weather.GetComponentInChildren<Text>().text = HouseWeather.Instance.Entity_id;

        autoSun.onValueChanged.AddListener((x) =>
        {
            suninpa.SetActive(!x);
            CacheData.AutoSun = x;
        });
        suninpa.SetActive(!autoSun.isOn);
        ro.onValueChanged.AddListener((x) => { CacheData.RoSpeed = x; });
        sunIn.onValueChanged.AddListener((x) =>
        {
            CacheData.SunIntensity = x;
            HouseWeather.Instance.SetTianGuang(x);
        });
        sunIn.value = CacheData.SunIntensity;

        addwwwResource.onClick.AddListener(() =>
        {
            var ap = Resources.Load<GameObject>("UI/AddModelPanel");
            ap = Instantiate(ap, House.Instance.canvas);
        });
    }

    private void Save()
    {
        var hs = House.Instance.House2Json();
        Shijie.Config3D(hs);
#if UNITY_EDITOR
        var path = Application.streamingAssetsPath + "/Customdata/houseconnfig.json";
        var dir = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        System.IO.File.WriteAllText(path, hs);
#endif
    }
}
