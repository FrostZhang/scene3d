using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddModelPanel : MonoBehaviour
{
    public Button bj;
    public Button add;
    public InputField inputField;
    void Start()
    {
        bj.onClick.AddListener(() => Destroy(gameObject));
        inputField.onEndEdit.AddListener((x) =>
        {

        });
        add.onClick.AddListener(() =>
        {

        });
        add.interactable = false;
    }

}
