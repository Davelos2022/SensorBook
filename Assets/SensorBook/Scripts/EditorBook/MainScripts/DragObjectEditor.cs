using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DragObjectEditor : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Button _deletedObject;

    private Canvas _canvas; 

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    public UnityEvent OnDragEnd;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        _canvas = Object.FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        _deletedObject.onClick.AddListener(DeletedObject);
    }

    private void OnDisable()
    {
        _deletedObject.onClick.RemoveListener(DeletedObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {    
        _rectTransform.SetAsLastSibling();

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    private bool GetInsideScene()
    {
        var localCorners = new Vector3[4];
        _rectTransform.GetWorldCorners(localCorners);
        //var parentWorldRect = _parentTransform.GetWorldRect();
        var parentWorldRect = GetWorldRect(_rectTransform.parent.GetComponent<RectTransform>());
        return localCorners.All(corner => parentWorldRect.Contains(corner));
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        var min = corners[0];
        var max = corners[2];
        var size = max - min;
        return new Rect(min, size);
    }

    public void MoveAround(PointerEventData eventData)
    {
        var anchoredPosition = _rectTransform.anchoredPosition;
        var oldPosition = anchoredPosition;
        var newPosition = anchoredPosition + eventData.delta / _canvas.scaleFactor;
        anchoredPosition = newPosition;

        _rectTransform.anchoredPosition = anchoredPosition;

        if (!GetInsideScene())
        {
            _rectTransform.anchoredPosition = oldPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;
        OnDragEnd.Invoke();
    }

    private void DeletedObject()
    {
        Destroy(this.gameObject);
    }
}
