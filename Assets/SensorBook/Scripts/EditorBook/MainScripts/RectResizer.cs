using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class RectResizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [Space]
    [SerializeField] private _typeReSize _TypeRe;
    [SerializeField] private List<ResizePoint> resizePoints;
    [SerializeField] private RectTransform constraints;
    [SerializeField] private RectTransform targetRect;
    
    public UnityEvent OnResizeStart;
    public UnityEvent OnResizeEnd;

    private Vector2 _initialRectSize;
    private enum _typeReSize { Image, Text}

    private float _minWidth = 100;
    private float _minHeight = 100;
    private float _minTextSize = 20f;
    private float _maxTextSize = 70f;

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

    private void Awake()
    {
        for (int i = 0; i < resizePoints.Count; i++)
        {
            resizePoints[i].Initialize(this);
        }
    }

    public void DragStartHandle(PointerEventData eventData)
    {
        _initialRectSize.x = targetRect.sizeDelta.x;
        _initialRectSize.y = targetRect.sizeDelta.y;

        OnResizeStart.Invoke();
    }

    public void DragHandle(ResizePoint point)
    {
        if (constraints != null)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(constraints, point.EventData.position))
            {
                return;
            }
        }

        var scaleDiff = PointerDeltaToCanvas(point.EventData.delta);

        var setHeight = targetRect.sizeDelta.y;
        var setWidth = targetRect.sizeDelta.x;

        var aspect = targetRect.rect.width / targetRect.rect.height;

        scaleDiff.x *= point.NormalizedPoint.x;
        scaleDiff.y *= point.NormalizedPoint.y;

        setHeight += scaleDiff.y;
        setWidth += scaleDiff.x;

        var oppositePoint = targetRect.pivot;

        if (_TypeRe == _typeReSize.Text && scaleDiff.x != 0 && scaleDiff.y != 0)
        {
            _text.fontSize += scaleDiff.y / 5f;
            _text.fontSize = Mathf.Clamp(_text.fontSize, _minTextSize, _maxTextSize);
        }

        setHeight = Mathf.Clamp(setHeight, _minHeight, float.MaxValue);
        setWidth = Mathf.Clamp(setWidth, _minWidth, float.MaxValue);

        if (point.CornerPoint)
        {
            if (Mathf.Abs(point.EventData.delta.x) > Mathf.Abs(point.EventData.delta.y))
            {
                setHeight = setWidth / aspect;
            }
            else
            {
                setWidth = setHeight * aspect;
            }
        }

        SetPivot(targetRect, oppositePoint);

        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, setWidth);
        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setHeight);


    }

    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    public void DragEndHandle(PointerEventData eventData)
    {
        OnResizeEnd.Invoke();

        SetPivot(targetRect, Vector2.one * 0.5f);
    }

    public Vector2 PointerDeltaToCanvas(Vector2 point)
    {
        return point / _screenRatio;
    }
}
