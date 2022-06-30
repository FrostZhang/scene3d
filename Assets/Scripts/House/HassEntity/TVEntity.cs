using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVEntity : HassEntity
{
    public GameObject onOnj, high;
    public TextMesh te;
    string[] tes;
    void Start()
    {
        tes = new string[te.text.Length];
        string str = string.Empty;
        for (int i = 0; i < te.text.Length; i++)
        {
            str += te.text[i];
            tes[i] = str.Clone() as string;
        }
        onOnj.SetActive(false);
        GetComponent<Collider>().enabled = true;
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
        base.TrunOn();
        if (!onOnj.activeSelf)
        {
            onOnj.SetActive(true);
            StartCoroutine(AnimText());
            RunPanel.instance.Show(transform, "TV On");
        }
    }

    protected override void TrunOff()
    {
        onOnj.SetActive(false);
        RunPanel.instance.Show(transform, "TV Off");
    }

    public override void ReconstitutionMode(bool enter)
    {
        //onOnj.SetActive(enter);
    }

    IEnumerator AnimText()
    {
        while (onOnj.activeSelf)
        {
            for (int i = 0; i < tes.Length; i++)
            {
                te.text = tes[i];
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
