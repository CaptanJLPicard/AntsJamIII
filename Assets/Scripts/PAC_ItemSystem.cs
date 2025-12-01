using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PAC_ItemSystem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private int index;
    private PAC_CraftSystem craftSystem;
    private Image image;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup cGroup;

    private void Awake()
    {
        craftSystem = FindAnyObjectByType<PAC_CraftSystem>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        cGroup = GetComponent<CanvasGroup>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject draggedObject = Instantiate(gameObject, transform.position, Quaternion.identity);
        draggedObject.transform.SetParent(transform.parent);
        draggedObject.name = gameObject.name + "_Dragged";

        PAC_ItemSystem draggedScript = draggedObject.GetComponent<PAC_ItemSystem>();
        eventData.pointerDrag = draggedObject;
        draggedScript.cGroup.alpha = 0.6f;
        draggedScript.cGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null &&
            eventData.pointerEnter.gameObject.CompareTag("Boiler") &&
            craftSystem.currentObjects.Count < 8)
        {
            string cleanName = gameObject.name.Replace("_Dragged", "");

            bool alreadyExists = false;
            foreach (var item in craftSystem.currentObjects)
            {
                string itemCleanName = item.name.Replace("_Dragged", "");
                if (itemCleanName == cleanName)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                Debug.Log("Bu item zaten var, eklenmiyor.");
                Destroy(gameObject);
                return;
            }

            craftSystem.currentObjects.Add(gameObject);
            craftSystem.contentsTxt.text = "Contents: " + craftSystem.currentObjects.Count;
            craftSystem.currentCombination[index] = true;

            cGroup.alpha = 1f;
            cGroup.blocksRaycasts = true;
            StartCoroutine(FadeOut());
        }
        else if (craftSystem.currentObjects.Count >= 8)
        {
            craftSystem.ResetSpeel();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        cGroup.alpha = 0f;
        cGroup.blocksRaycasts = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}