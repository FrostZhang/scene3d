using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACEntity : HassEntity
{
    [SerializeField] ParticleSystem eff;
    [SerializeField] GameObject high;
    public override void MouseOn()
    {
        high.SetActive(true);
    }
    public override void MouseExit()
    {
        high.SetActive(false);
    }
    void Awake()
    {
        eff.gameObject.SetActive(false);
    }
    protected override void TrunOn()
    {
        eff.gameObject.SetActive(true);
    }
    protected override void TrunOff()
    {
        eff.gameObject.SetActive(false);
    }
    protected override void HassDestine(string state)
    {
        if (state == "cool")
        {
            eff.gameObject.SetActive(true);
            var m = eff.main;
            var c = m.startColor;
            c.color = Color.blue;
            m.startColor = c;
        }
        else if (state == "heat")
        {
            eff.gameObject.SetActive(true);
            var m = eff.main;
            var c = m.startColor;
            c.color = Color.red;
            m.startColor = c;
        }
        else if (state == "heat_cool")
        {
            eff.gameObject.SetActive(true);
            var m = eff.main;
            var c = m.startColor;
            c.color = Color.green;
            m.startColor = c;
        }
        else
        {
            eff.gameObject.SetActive(false);
        }
        RunPanel.instance.Show(transform, "AC " + state + "!");
    }
    protected override void HassStateOn()
    {
        RunPanel.instance.Show(transform, "AC On!");
    }

    protected override void HassStateOff()
    {
        RunPanel.instance.Show(transform, "AC Off!");
    }
    public override void ReconstitutionMode(bool enter)
    {
        if (enter)
        {
            eff.gameObject.SetActive(false);
        }
        else
        {
            if (open)
            {
                eff.gameObject.SetActive(true);
            }
        }
    }
}
