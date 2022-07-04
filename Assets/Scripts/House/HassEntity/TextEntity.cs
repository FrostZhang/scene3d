using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEntity : HassEntity
{

    [SerializeField] MeshRenderer mr;
    public TextMesh textMesh;

    public string front, back;
    public string value = "?";
    void Start()
    {
        textMesh.font = Help.Instance.chinese;
        textMesh.GetComponent<MeshRenderer>().material = Help.Instance.chinese.material;
        mr.enabled = false;
        FlushText();
        editC.enabled = false;
    }
    protected override void HassDestine(string state)
    {
        if (value != state)
        {
            value = state;
            FlushText();
        }
    }

    protected override void TrunOn()
    {
        if (value != "On")
        {
            value = "On";
            FlushText();
        }
    }

    protected override void TrunOff()
    {
        if (value != "Off")
        {
            value = "Off";
            FlushText();
        }
    }

    public override void ReconstitutionMode(bool enter)
    {
        base.ReconstitutionMode(enter);
        if (enter)
        {
            mr.enabled = true;
        }
        else
        {
            mr.enabled = false;
        }
    }

    internal void FlushText()
    {
        textMesh.text = front + value + back;
        RunPanel.instance.Show(transform, textMesh.text);
    }
}
