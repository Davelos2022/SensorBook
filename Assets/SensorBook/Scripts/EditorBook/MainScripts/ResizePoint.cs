using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizePoint : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Vector2 normalizedPoint;
    [SerializeField] private bool cornerPoint;

    private bool _dragging;
    public bool Dragging => _dragging;
    public bool CornerPoint => cornerPoint;
    public Vector2 NormalizedPoint => normalizedPoint;
    public PointerEventData EventData => _data;

    private RectResizer _resizer;
    private PointerEventData _data;

    private Vector2 _screenRatio
    {
        get
        {
            Vector2 ratio;
            ratio.x = Screen.width / Screen.currentResolution.width;
            ratio.y = Screen.height / Screen.currentResolution.height;
            return ratio;
        }
    }



    public void Initialize(RectResizer resizer)
    {
        _resizer = resizer;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        _resizer.DragStartHandle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //(transform as RectTransform).anchoredPosition += eventData.delta / _screenRatio;

        _data = eventData;
        _resizer.DragHandle(this);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        _resizer.DragEndHandle(eventData);
        EditorBook.Instance.TakeScreenShotCurrentPage();
    }
}
