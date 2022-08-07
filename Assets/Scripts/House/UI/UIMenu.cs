using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UIMenu : MonoBehaviour, IPointerDownHandler
    {
        public Transform Tr { get; set; }

        public RectTransform content;
        public Button prefab;

        Queue<Button> buttons;
        public static UIMenu instance;
        private void Awake()
        {
            Tr = transform;
            instance = this;
            prefab.gameObject.SetActive(false);
            buttons = new Queue<Button>();
            for (int i = 0; i < 5; i++)
            {
                buttons.Enqueue(Instantiate(prefab, prefab.transform.parent));
            }
        }

        public void OnDestroy()
        {
            instance = null;
        }

        public void Start()
        {

        }

        public async Task<int> Show(Vector2 pos, params string[] items)
        {
            var btns = Tr.GetComponentsInChildren<Button>(false);
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].onClick.RemoveAllListeners();
                btns[i].gameObject.SetActive(false);
                buttons.Enqueue(btns[i]);
            }
            transform.gameObject.SetActive(true);
            if (pos.x < Screen.width * 0.5f)
            {
                if (pos.y < Screen.height * 0.5f)
                {
                    content.pivot = Vector2.zero;
                    pos += Vector2.up * 50;
                }
                else
                {
                    content.pivot = Vector2.up;
                    pos -= Vector2.up * 50;
                }
            }
            else
            {
                if (pos.y < Screen.height * 0.5f)
                {
                    content.pivot = Vector2.right;
                    pos += Vector2.up * 50;
                }
                else
                {
                    content.pivot = Vector2.one;
                    pos -= Vector2.up * 50;
                }
            }
            if ((pos.y - items.Length * 30) < 0)
            {
                pos.y += items.Length * 30 - pos.y;
            }
            if (pos.x < 0)
            {
                pos.x = 0;
            }
            if (pos.x + 220 > Screen.width)
            {
                pos.x = Screen.width - 200;
            }
            content.transform.position = pos;
            int res = -1;
            for (int i = 0; i < items.Length; i++)
            {
                Button btn = null;
                if (buttons.Count > 0)
                    btn = buttons.Dequeue();
                else
                    btn = Instantiate(prefab, prefab.transform.parent);
                btn.gameObject.SetActive(true);
                btn.transform.SetAsLastSibling();
                var n = i;
                btn.onClick.AddListener(() =>
                {
                    res = n;

                });
                btn.GetComponentInChildren<Text>().text = items[i];
            }
            while (gameObject && res < 0)
                await new WaitForFixedUpdate();
            transform.gameObject.SetActive(false);
            return res;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(content, eventData.position))
            {
                if (content.gameObject.activeSelf)
                {
                    var btns = Tr.GetComponentsInChildren<Button>(false);
                    for (int i = 0; i < btns.Length; i++)
                    {
                        btns[i].onClick.RemoveAllListeners();
                        btns[i].gameObject.SetActive(false);
                        buttons.Enqueue(btns[i]);
                    }
                    transform.gameObject.SetActive(false);
                }
            }
        }
    }
}