using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverEntity : HassEntity
{
    public float angleclose, angleopen;

    protected virtual void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        angleclose = transform.localScale.x;
        TrunOff();
    }

    protected override void TrunOn()
    {
        StopAllCoroutines();
        StartCoroutine(To(angleopen));
    }
    protected override void TrunOff()
    {
        StopAllCoroutines();
        StartCoroutine(To(angleclose));
    }
    protected override void HassStateOn()
    {
        RunPanel.instance.Show(transform, "Door On!");
    }
    protected override void HassStateOff()
    {
        RunPanel.instance.Show(transform, "Door Off!");
    }
    IEnumerator To(float f)
    {
        float t = 1 / 2f;
        float l = 0;
        var e = transform.localScale.x;
        while (true)
        {
            var a = Mathf.LerpAngle(e, f, l += t * Time.deltaTime);
            transform.localScale = new Vector3(a, transform.localScale.y, transform.localScale.z);
            yield return new WaitForEndOfFrame();
            if (l > 1)
                break;
        }
    }
    protected override void HassDestine(string state)
    {
        float f;
        if (float.TryParse(state, out f))
        {
            StopAllCoroutines();
            StartCoroutine(To(f));
        }
    }
}
