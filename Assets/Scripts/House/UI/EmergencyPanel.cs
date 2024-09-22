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
    }

    public void Show(string mes)
    {
        Tr.SetAsLastSibling();
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
        StopAllCoroutines();
        content.gameObject.SetActive(false);
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {

    }
}
