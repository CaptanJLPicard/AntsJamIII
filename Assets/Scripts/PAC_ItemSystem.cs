using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PAC_ItemSystem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Item Identity")]
    [SerializeField] private int index;
    [SerializeField] private string itemName;

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

        if (string.IsNullOrEmpty(itemName))
            itemName = gameObject.name;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"OnPointerDown: {itemName}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject draggedObject = Instantiate(gameObject, transform.position, Quaternion.identity);
        draggedObject.transform.SetParent(canvas.transform);
        draggedObject.name = itemName + "_Dragged";

        PAC_ItemSystem draggedScript = draggedObject.GetComponent<PAC_ItemSystem>();
        draggedScript.index = this.index;
        draggedScript.itemName = this.itemName;

        eventData.pointerDrag = draggedObject;

        draggedScript.cGroup.alpha = 0.7f;
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
            string cleanName = itemName.Replace("_Dragged", "");

            bool alreadyExists = false;
            foreach (var item in craftSystem.currentObjects)
            {
                if (item == null) continue;

                PAC_ItemSystem itemScript = item.GetComponent<PAC_ItemSystem>();
                if (itemScript != null && itemScript.GetItemName().Replace("_Dragged", "") == cleanName)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                Debug.Log("Item already exists!");
                Destroy(gameObject);
                return;
            }

            craftSystem.currentObjects.Add(gameObject);
            craftSystem.contentsTxt.text = "Contents: " + craftSystem.currentObjects.Count;
            craftSystem.currentCombination[index] = true;

            craftSystem.OnItemAdded(index, itemName);

            cGroup.alpha = 1f;
            cGroup.blocksRaycasts = true;

            StartCoroutine(FadeOut());
        }
        else if (craftSystem.currentObjects.Count >= 8)
        {
            Debug.Log("Cauldron full!");
            craftSystem.ResetSpell();
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

    public string GetItemName() => itemName;
    public int GetIndex() => index;
}