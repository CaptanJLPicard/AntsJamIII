using UnityEngine;

public class GameUIScripts : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;

    [SerializeField] private Vector2 bigSize = new Vector2(1920f, 1080f);
    [SerializeField] private Vector2 smallSize = new Vector2(400f, 225f);

    private void Start()
    {
        if (targetRect != null)
            targetRect.sizeDelta = smallSize;
    }

    private void Update()
    {
        if (targetRect == null) return;

        if (Input.GetKey(KeyCode.Tab))
        {
            targetRect.sizeDelta = bigSize;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            targetRect.sizeDelta = smallSize;
        }
    }
}
