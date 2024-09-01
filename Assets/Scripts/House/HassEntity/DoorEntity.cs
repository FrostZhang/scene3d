using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEntity : HassEntity
{
    Coroutine coroutine;
    public float angleclose, angleopen;
    public MeshRenderer editR;
    private Material yuanMa;

    public Material YuanMa { get => yuanMa; }

    private void Awake()
    {
        yuanMa = editR.material = new Material(editR.material);

    }
    protected virtual void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        transform.eulerAngles = new Vector3(0, angleclose, 0);
        TrunOff();
    }

    protected override void TrunOn()
    {
        StopAllCoroutines();
        coroutine = StartCoroutine(To(angleopen));
    }
    protected override void TrunOff()
    {
        StopAllCoroutines();
        coroutine = StartCoroutine(To(angleclose));
    }
    protected override void HassStateOn()
    {
        EmergencyPanel.instance.Show(transform, "Door On!");
    }
    protected override void HassStateOff()
    {
        EmergencyPanel.instance.Show(transform, "Door Off!");
    }
    IEnumerator To(float f)
    {
        float t = 1 / 2f;
        float l = 0;
        var e = transform.eulerAngles;
        while (true)
        {
            var a = Mathf.LerpAngle(e.y, f, l += t * Time.deltaTime);
            transform.eulerAngles = new Vector3(e.x, a, e.z);
            yield return new WaitForEndOfFrame();
            if (l > 1)
                break;
        }
    }

    public virtual async void Sim()
    {
        await To(angleopen);
        await To(angleclose);
    }
}
