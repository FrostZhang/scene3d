using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEntity : HassEntity
{
    Vector3 iniPos;
    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        iniPos = transform.position;
    }
    protected override void TrunOn()
    {
        StopAllCoroutines();
        StartCoroutine(Anim(true));
    }
    protected override void TrunOff()
    {
        StopAllCoroutines();
        StartCoroutine(Anim(false));
    }
    protected override void HassStateOn()
    {
        RunPanel.instance.Show(transform, "Car home!");
    }
    protected override void HassStateOff()
    {
        RunPanel.instance.Show(transform, "Car Leave home!");
    }
    protected override void HassDestine(string state)
    {
        RunPanel.instance.Show(transform, state);
    }
    public override void ReconstitutionMode(bool enter)
    {
        base.ReconstitutionMode(enter);
        if (enter)
        {
            transform.position = iniPos;
            transform.gameObject.SetActive(true);
        }
        else
        {
            iniPos = transform.position;
            if (!open)
            {
                transform.position = transform.forward * 3 + transform.position;
                transform.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator Anim(bool home)
    {
        Vector3 endpos;
        Vector3 start = transform.position;
        if (home)
        {
            endpos = iniPos;
        }
        else
        {
            endpos = transform.forward * 3 + transform.position;
        }
        transform.gameObject.SetActive(true);
        float f = 0;
        while (transform && (f += Time.deltaTime) < 1)
        {
            var v = Vector3.Lerp(start, endpos, f);
            transform.position = v;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endpos;
        if (!home)
        {
            transform.gameObject.SetActive(false);
        }
    }
}
