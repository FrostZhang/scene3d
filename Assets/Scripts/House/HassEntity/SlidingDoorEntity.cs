using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorEntity : HassEntity
{
    public Material YuanMa { get => yuanMa; }
    public Transform left, right;
    public bool iszuoyi;
    public MeshRenderer leftM, rightM;
    private Material yuanMa;
    Coroutine coroutine;


    private void Awake()
    {
        yuanMa = rightM.material = leftM.material = new Material(leftM.material);
    }

    protected void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        TrunOff();
    }

    protected override void TrunOn()
    {
        StopAllCoroutines();
        coroutine = StartCoroutine(Open());
    }

    protected override void TrunOff()
    {
        StopAllCoroutines();
        coroutine = StartCoroutine(Close());
    }
    IEnumerator Open()
    {
        float l = 0;
        float t = 1 / 2f;
        if (iszuoyi)
        {
            var pl = left.localPosition;
            Vector3 end = new Vector3(-0.5f, 0, 0);
            while (true)
            {
                left.localPosition = Vector3.Lerp(pl, end, l += t * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                if (l > 1)
                    break;
            }
        }
        else
        {
            var pl = right.localPosition;
            Vector3 end = new Vector3(0.5f, 0, 0.05f);
            while (true)
            {
                right.localPosition = Vector3.Lerp(pl, end, l += t * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                if (l > 1)
                    break;
            }
        }
    }

    IEnumerator Close()
    {
        float l = 0;
        float t = 1 / 2f;
        if (iszuoyi)
        {
            var pl = left.localPosition;
            Vector3 end = new Vector3(0.5f, 0, 0);
            while (true)
            {
                right.localPosition = Vector3.Lerp(pl, end, l += t * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                if (l > 1)
                    break;
            }
        }
        else
        {
            var pl = right.localPosition;
            Vector3 end = new Vector3(-0.5f, 0, 0.05f);
            while (true)
            {
                right.localPosition = Vector3.Lerp(pl, end, l += t * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                if (l > 1)
                    break;
            }
        }
    }

    bool issim;
    public async void Sim()
    {
        if (issim)
            return;
        issim = true;
        await Open();
        await Close();
        issim = false;
    }
}
