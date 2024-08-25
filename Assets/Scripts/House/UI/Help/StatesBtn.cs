using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatesBtn : MonoBehaviour
{
    public int State { get; private set; }
    public Action<int> onValueChanged;
    public Sprite[] statesSps;
    public Color[] statesCrs;
    public Image image;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        State++;
        if (State >= statesSps.Length)
        {
            State = 0;
        }
        onValueChanged?.Invoke(State);
        SetIsOnWithoutNotify(State);
    }

    public void SetIsOnWithoutNotify(int sta)
    {
        State = sta;
        image.sprite = statesSps[State];
        if (statesCrs.Length > State)
        {
            image.color = statesCrs[sta];
        }
    }

    public void SetValue(int sta)
    {
        SetIsOnWithoutNotify(sta);
        onValueChanged?.Invoke(sta);
    }
}
