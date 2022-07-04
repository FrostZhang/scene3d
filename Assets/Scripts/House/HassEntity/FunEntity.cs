using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunEntity : HassEntity
{
    public Transform fan;
    public GameObject high;
    void Awake()
    {
        high.SetActive(false);
    }
    public override void MouseOn()
    {
        high.SetActive(true);
    }
    public override void MouseExit()
    {
        high.SetActive(false);
    }
    protected override void TrunOn()
    {
        StopAllCoroutines();
        StartCoroutine(Run());
    }

    protected override void TrunOff()
    {
        StopAllCoroutines();
    }
    protected override void HassStateOn()
    {
        RunPanel.instance.Show(transform, "Fun On!");
    }
    protected override void HassStateOff()
    {
        RunPanel.instance.Show(transform, "Fun Off!");
    }
    public override void ReconstitutionMode(bool enter)
    {
        if (enter)
        {
            StopAllCoroutines();
        }
        else
        {
            if (open)
            {
                StopAllCoroutines();
                StartCoroutine(Run());
            }
        }
    }

    IEnumerator Run()
    {
        while (transform)
        {
            var e = fan.eulerAngles;
            e.y += 1;
            if (e.y > 360)
            {
                e.y -= 360;
            }
            fan.eulerAngles = e;
            yield return null;
        }
    }
}
