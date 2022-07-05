using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverEntity : HassEntity
{
    public float scaleFilter = 1.47f;
    /// <summary>¿ª </summary>
    public float openW;
    public Transform left, right;
    protected virtual void Start()
    {
        if (openW > transform.localScale.x || openW <= 0)
            openW = transform.localScale.x - 0.1f;
        GetComponent<Collider>().enabled = true;
    }

    protected override void TrunOn()
    {
        StopAllCoroutines();
        StartCoroutine(To(openW));
    }
    protected override void TrunOff()
    {
        StopAllCoroutines();
        StartCoroutine(To(0));
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
        var end = (e - f) * 0.5f / (scaleFilter * e);
        var start = left.localScale.x;
        while (transform)
        {
            var a = Mathf.LerpAngle(start, end, l += t * Time.deltaTime);
            var s = left.localScale;
            s.x = a;
            left.localScale = s;
            right.localScale = s;
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
            StartCoroutine(To(f * 0.01f * openW));
        }
    }
    public override void ReconstitutionMode(bool enter)
    {

    }
}
