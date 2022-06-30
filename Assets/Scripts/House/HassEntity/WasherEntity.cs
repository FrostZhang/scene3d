using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasherEntity : HassEntity
{
    Vector3 e;
    void Start()
    {
        e = transform.eulerAngles;
        GetComponent<BoxCollider>().enabled = false;
    }
    protected override void TrunOn()
    {
        base.TrunOn();
        RunPanel.instance.Show(transform, "Washer On");
    }

    protected override void TrunOff()
    {
        base.TrunOff();
        RunPanel.instance.Show(transform, "Washer Off");
    }

    float interval = 3;
    float _interval;
    void Update()
    {
        if (open)
        {
            if ((_interval -= Time.deltaTime) < 0)
            {
                _interval = interval;
                StartCoroutine(Anim());
            }
        }
    }

    IEnumerator Anim()
    {
        for (int i = 0; i < 5; i++)
        {
            transform.eulerAngles += UnityEngine.Random.onUnitSphere * 2;
            yield return new WaitForSeconds(0.1f);
        }
        transform.eulerAngles = e;
    }
}
