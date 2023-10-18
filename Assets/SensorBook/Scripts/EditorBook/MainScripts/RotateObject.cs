using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float rotationSpeed;

    private Vector2 pivotPosition;
    private Vector2 initialMousePosition;
    private float speed = 0.3f;

    private Quaternion _startRotation;
    private void OnEnable()
    {
        pivotPosition = rectTransform.pivot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialMousePosition = eventData.pressPosition;
        _startRotation = rectTransform.rotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragVector = eventData.position - initialMousePosition;
        float rotationAngle = -Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;

        rectTransform.pivot = pivotPosition;
        rectTransform.rotation = _startRotation * Quaternion.Euler(0f, 0f, rotationAngle * rotationSpeed * speed);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SaveStepRotation();
        EditorBook.Instance.TakeScreenShotCurrentPage();
    }

    private void SaveStepRotation()
    {
        UndoRedoSystem.Instance.AddAction(new RotationObjectAction
            (rectTransform, _startRotation, rectTransform.rotation));
    }
}

