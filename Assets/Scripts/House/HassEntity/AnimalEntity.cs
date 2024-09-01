using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEntity : HassEntity
{
    public static int info = Animator.StringToHash("AnimInfo");

    public Animator anim;
    public bool canmove;
    public Transform body;

    void Start()
    {
        ShowEditMesh(CacheData.isEditorMode);
        GetComponent<BoxCollider>().enabled = false;
        NextAnim();
        body.SetParent(transform.parent);
        body.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        if (body.parent != transform)
        {
            Destroy(body.gameObject);
        }
    }

    private void NextAnim()
    {
        if (!transform)
        {
            Debug.LogError("£¡£¡£¡£¡£¡");
            return;
        }
        if (!canmove)
        {
            StartCoroutine(SetPos());
        }
        else
        {
            if (Random.value > 0.4f)
            {
                Vector3 pos = transform.TransformPoint(new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f));
                StartCoroutine(WalkTo(pos));
            }
            else
            {
                StartCoroutine(StandAnim());
            }
        }
    }

    IEnumerator SetPos()
    {
        body.position = transform.position;
        yield return new WaitForSeconds(Random.value * 10);
        NextAnim();
    }

    IEnumerator StandAnim()
    {
        anim.SetInteger(info, 0);
        yield return new WaitForSeconds(Random.value * 10);
        NextAnim();
    }

    IEnumerator WalkTo(Vector3 pos)
    {
        var p = body.position;
        float t = 0;
        var speed = Vector3.Distance(pos, p);
        speed = 1 / speed;
        anim.SetInteger(info, 1);
        body.LookAt(pos);
        while (body)
        {
            body.position = Vector3.Lerp(p, pos, t += Time.deltaTime * speed);
            if (t > 1)
                break;
            yield return new WaitForEndOfFrame();
        }
        NextAnim();
    }

    public override void ReconstitutionMode(bool enter)
    {
        base.ReconstitutionMode(enter);
        ShowEditMesh(enter);
    }

    void ShowEditMesh(bool isedit)
    {
        GetComponent<MeshRenderer>().enabled = isedit;
    }
}
