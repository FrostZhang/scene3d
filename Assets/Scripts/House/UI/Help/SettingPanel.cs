using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour, IPanel
{
    public Slider roSpeed, sunlight, caH, zhiliang;
    public Toggle autosun, quanping;
    public Button weather, bianji;
    public Button guanyu;
    public Button coffee, exit;

    public Transform tos;
    public Transform toOBJs;

    public Transform Tr { get; set; }

    public void Start()
    {
        roSpeed.value = CacheData.RoSpeed;
        roSpeed.onValueChanged.AddListener((x) => { CacheData.RoSpeed = x; });

        autosun.isOn = CacheData.AutoSun;
        autosun.onValueChanged.AddListener((x) =>
        {
            sunlight.enabled = x;
            CacheData.AutoSun = x;
        });

        sunlight.enabled = autosun.isOn;
        sunlight.value = CacheData.SunIntensity;
        sunlight.onValueChanged.AddListener((x) =>
        {
            CacheData.SunIntensity = x;
            HouseWeather.Instance.SetTianGuang(x);
        });

        weather.GetComponentInChildren<Text>().text = HouseWeather.Instance.Entity_id;
        weather.onClick.AddListener(() =>
        {
            Surface.surface.LoadPanel<EntityListPanel>().ChooseEntity((n) =>
            {
                weather.GetComponentInChildren<Text>().text = n;
                HouseWeather.Instance.SetEntity(n);
            });
        });

        caH.value = CameraControllerForUnity.Instance.mobaLimitDistance.y;
        caH.onValueChanged.AddListener((x) =>
        {
            CameraControllerForUnity.Instance.mobaLimitDistance.y = x;
        });

        zhiliang.value = QualitySettings.GetQualityLevel();
        zhiliang.onValueChanged.AddListener((x) =>
        {
            QualitySettings.SetQualityLevel((int)x);
        });


        quanping.onValueChanged.AddListener(SetBig);

        if (Shijie.WhichStand() == 1)
        {
            HouseWeather.Instance.mainlight.shadows = LightShadows.None;
            bianji.interactable = false;
        }
        else
            bianji.onClick.AddListener(Edit);

        guanyu.onClick.AddListener(() =>
        {
            Surface.surface.LoadPanel<SouquanPanel>();
        });

        var tts = tos.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < tts.Length; i++)
        {
            var n = i;
            tts[i].onValueChanged.AddListener((x) =>
            {
                foreach (Transform item in toOBJs)
                {
                    item.gameObject.SetActive(false);
                }
                toOBJs.GetChild(n).gameObject.SetActive(true);
            });
        }
        tts[0].isOn = true;
        foreach (Transform item in toOBJs)
        {
            item.gameObject.SetActive(false);
        }
        toOBJs.GetChild(0).gameObject.SetActive(true);

        coffee.onClick.AddListener(() => Application.OpenURL("https://www.buymeacoffee.com/3762375193"));
        exit.onClick.AddListener(() => Surface.surface.RemovePanel<SettingPanel>());
    }

    private void SetBig(bool quanping)
    {
        Shijie.Link3DBigView(quanping ? 1 : 0);
    }

    private void Edit()
    {
        Surface.surface.RemovePanel<SettingPanel>();
        Surface.surface.RemovePanel<HousePanel>();
        EditMode.EditScene(true);
    }

    public void OnDestroy()
    {

    }
}
