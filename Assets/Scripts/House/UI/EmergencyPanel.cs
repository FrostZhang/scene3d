using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmergencyPanel : MonoBehaviour, IPanel
{
    public static EmergencyPanel instance;
    public Transform Tr { get; set; }
    public Transform content;
    public Text te;
    void Awake()
    {
        content.gameObject.SetActive(false);
        instance = this;
        CacheData.OnEditorMode += OnEditorMode;
    }

    private void OnEditorMode(bool v)
    {
        if (v)
        {
            StopAllCoroutines();
        }
    }

    public void Show(Transform bind, string mes)
    {
        if (CacheData.isEditorMode)
            return;
        StopAllCoroutines();
        StartCoroutine(_Anim(mes));
    }

    IEnumerator _Anim(string mes)
    {
        content.gameObject.SetActive(true);
        te.text = mes;
        yield return new WaitForSeconds(3);
        content.gameObject.SetActive(false);
    }

    public void Hide()
    {
        
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {

    }
}
