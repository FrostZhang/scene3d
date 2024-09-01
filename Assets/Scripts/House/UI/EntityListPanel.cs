using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityListPanel : MonoBehaviour, IPanel
{
    public Button prefab, exit;
    public InputField sec;
    List<Button> buttons;
    System.Action<string> callback;

    Button self;

    public Transform Tr { get; set; }

    void Awake()
    {
        buttons = new List<Button>(100);

        sec.onEndEdit.AddListener((x) =>
        {
            if (string.IsNullOrEmpty(x))
            {
                foreach (var item in buttons)
                {
                    if (item.GetComponentInChildren<Text>().text.Contains(x))
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                return;
            }
            foreach (var item in buttons)
            {
                if (item.GetComponentInChildren<Text>().text.Contains(x))
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        });
    }

    public void Start()
    {
        exit.onClick.AddListener(() =>
        {
            Surface.surface.RemovePanel<EntityListPanel>();
        });

        if (House.Instance.HAeys.Count == 0)
        {
            sec.text = "Œ¥º”‘ÿEntity!";
            sec.textComponent.color = Color.red;
            prefab.gameObject.SetActive(false);
            return;
        }

        prefab.gameObject.SetActive(true);
        var p1 = Instantiate(prefab, prefab.transform.parent);
        p1.GetComponentInChildren<Text>().text = string.Empty;
        p1.onClick.AddListener(() => callback?.Invoke(string.Empty));
        buttons.Add(p1);
        foreach (var item in House.Instance.HAeys)
        {
            var p = Instantiate(prefab, prefab.transform.parent);
            var t = p.GetComponentInChildren<Text>().text = item;
            p.onClick.AddListener(() =>
            {
                callback?.Invoke(t);
                Surface.surface.RemovePanel<EntityListPanel>();
            });
            buttons.Add(p);
        }
        prefab.gameObject.SetActive(false);
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    public void ChooseEntity(System.Action<string> back)
    {
        this.callback = back;
    }



    public void OnDestroy()
    {

    }
}
