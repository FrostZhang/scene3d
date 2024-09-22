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
        StopAllCoroutines();
        StartCoroutine(Anim());
    }
    protected override void TrunOff()
    {
        StopAllCoroutines();
    }

    float interval = 3;
    IEnumerator Anim()
    {
        while (transform)
        {
            for (int i = 0; i < 5; i++)
            {
                transform.eulerAngles += UnityEngine.Random.onUnitSphere * 2;
                yield return new WaitForSeconds(0.1f);
            }
            transform.eulerAngles = e;
            yield return new WaitForSeconds(interval);
        }
    }
}
