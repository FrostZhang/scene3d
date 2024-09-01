using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SouquanPanel : MonoBehaviour, IPanel
{
    public static SouquanPanel instance;
    public Button bj;

    public Transform Tr { get; set; }

    public void OnDestroy()
    {

    }

    public void Start()
    {

    }

    private void Awake()
    {
        instance = this;
        bj.onClick.AddListener(async () =>
        {
            Destroy(gameObject);
            await new WaitForUpdate();
            await Resources.UnloadUnusedAssets();
            System.GC.Collect();
        });
    }
}
