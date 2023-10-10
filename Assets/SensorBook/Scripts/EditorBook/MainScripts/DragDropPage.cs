using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropPage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] 
    private float _distance = 15f;


    private RectTransform _pageTransform;
    private CanvasGroup _canvasGroup;

    private Vector2 _startPosition;
    private ScrollRect _scrollRect;
    private GridLayoutGroup _gridLayoutGroup;

    private int currentIndex;
    void Start()
    {
        _pageTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startPosition = _pageTransform.anchoredPosition;
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        currentIndex = _pageTransform.transform.GetSiblingIndex();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _pageTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        for (int i = 0; i < _scrollRect.content.childCount; i++)
        {
            if (_scrollRect.content.GetChild(i) != transform)
            {
                int closestIndex;
                float distance = Vector3.Distance(_pageTransform.anchoredPosition, _scrollRect.content.GetChild(i).GetComponent<RectTransform>().anchoredPosition);

                if (distance < _distance)
                {
                    closestIndex = i;
                    EditorBook.Instance.SwapPages(currentIndex, closestIndex);

                    break;
                }


            }
            else
            {
                _pageTransform.anchoredPosition = _startPosition;
                break;
            }
        }


    }
}
