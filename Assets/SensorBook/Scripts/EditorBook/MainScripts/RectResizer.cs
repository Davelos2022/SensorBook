using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RectResizer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<ResizePoint> resizePoints;
    [SerializeField] private RectTransform constraints;
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private RawImage imageBox;
    [Space]
    [SerializeField] private GameObject pointsContainer;
    [SerializeField] private Button _deleted;

    public UnityEvent OnResizeStart;
    public UnityEvent OnResizeEnd;

    private Vector2 _initialRectSize;
    private RectTransform rect;
    private Vector2 _initialDragPoint;

    private bool isActive = false;

    private void OnEnable()
    {
        _deleted.onClick.AddListener(Deleted);
    }

    private void OnDisable()
    {
        _deleted.onClick.RemoveListener(Deleted);
        Activation_DeactivationPanel(false);
    }

    private void Deleted()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete) && this.pointsContainer.activeSelf)
            Deleted();
    }

    private Vector2 _screenRatio
    {
        get
        {
            Vector2 ratio;
            ratio.x = (float)Screen.width / (float)Screen.currentResolution.width;
            ratio.y = (float)Screen.height / (float)Screen.currentResolution.height;
            return ratio;
        }
    }

    private void Awake()
    {
        for (int i = 0; i < resizePoints.Count; i++)
        {
            resizePoints[i].Initialize(this);
        }

        rect = transform as RectTransform;
    }

    public void DragStartHandle(PointerEventData eventData)
    {
        _initialDragPoint = eventData.position;
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

        var posDiff = scaleDiff;

        scaleDiff.x *= point.NormalizedPoint.x;
        scaleDiff.y *= point.NormalizedPoint.y;


        setHeight += scaleDiff.y;
        setWidth += scaleDiff.x;

        var oppositePoint = ((point.NormalizedPoint * 0.5f) * -1) + Vector2.one * 0.5f;

        setHeight = Mathf.Clamp(setHeight, 0, float.MaxValue);
        setWidth = Mathf.Clamp(setWidth, 0, float.MaxValue);

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

        //posDiff.x *= Mathf.Abs(point.NormalizedPoint.x) * targetRect.rect.width > 0 ? 1 : 0;
        //posDiff.y *= Mathf.Abs(point.NormalizedPoint.y) * targetRect.rect.height > 0 ? 1 : 0;

        //targetRect.anchoredPosition += posDiff * 0.5f;

        //ClampWindow();
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
        //content.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        OnResizeEnd.Invoke();

        SetPivot(targetRect, Vector2.one * 0.5f);
    }

    private void ClampWindow()
    {
        var clamped = transform.position;

        clamped.x = Mathf.Clamp(clamped.x, 0, Screen.width);
        clamped.y = Mathf.Clamp(clamped.y, 0, Screen.height);

        transform.position = clamped;
    }


    public Vector2 PointerDeltaToCanvas(Vector2 point)
    {
        return point / _screenRatio;
    }

    private Vector2 PointerPositionToCanvas(Vector2 point)
    {
        point.x -= Screen.width * 0.5f;
        point.y -= Screen.height * 0.5f;
        point /= _screenRatio;

        return point;
    }

    public void SetImage(Texture texture, bool pdf = false)
    {
        imageBox.texture = texture;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isActive = !isActive;
        Activation_DeactivationPanel(isActive);
    }


    public void Activation_DeactivationPanel(bool active)
    {
        this.pointsContainer.SetActive(active);
    }
}
