using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropPage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] 
    private float closestDistance = 15f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startPosition;
    private ScrollRect scrollRect;

    //private float scrollRectHeight;
    //private float scrollRectWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        scrollRect = GetComponentInParent<ScrollRect>();
        //scrollRectHeight = scrollRect.GetComponent<RectTransform>().rect.height;
        //scrollRectWidth = scrollRect.GetComponent<RectTransform>().rect.width;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;

        //if (scscrollRect.verticalScrollbar != null)
        //{
        //    float minY = -rectTransform.anchoredPosition.y;
        //    float maxY = scrollRectHeight - rectTransform.rect.height - rectTransform.anchoredPosition.y;
        //    float scrollBarSize = scrollRect.verticalScrollbar.value * (scrollRectHeight - scrollRectHeight / scrollRect.content.localScale.y);
        //    float scrollBarOffset = scrollRectHeight - scrollBarSize;

        //    if (minY < scrollBarOffset)
        //    {
        //        float scrollSpeed = Mathf.Clamp01((scrollBarOffset - minY) / scrollBarOffset);
        //        scrollRect.verticalScrollbar.value -= Time.deltaTime * scrollSpeed * scrollRect.scrollSensitivity;
        //    }
        //    else if (maxY < scrollBarOffset)
        //    {
        //        float scrollSpeed = Mathf.Clamp01((scrollBarOffset - maxY) / scrollBarOffset);
        //        scrollRect.verticalScrollbar.value += Time.deltaTime * scrollSpeed * scrollRect.scrollSensitivity;
        //    }
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        int currentIndex = rectTransform.transform.GetSiblingIndex();

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            if (scrollRect.content.GetChild(i) != transform)
            {
                int closestIndex = 0;
                float distance = Vector3.Distance(rectTransform.anchoredPosition, scrollRect.content.GetChild(i).GetComponent<RectTransform>().anchoredPosition);

                if (distance < closestDistance)
                {
                    closestIndex = i;

                    scrollRect.content.GetChild(i).SetSiblingIndex(currentIndex);
                    rectTransform.transform.SetSiblingIndex(closestIndex);
                    EditorBook.Instance.SwapPages(currentIndex, closestIndex);

                    Debug.Log(currentIndex);
                    Debug.Log(closestIndex);

                    break;
                }


            }
            else
            {
                rectTransform.anchoredPosition = startPosition;
                break;
            }
        }
    }
}
