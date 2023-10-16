using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float rotationSpeed;

    private Vector2 pivotPosition;
    private Vector2 initialMousePosition;
    private float speed = 0.2f;
    private void OnEnable()
    {
        pivotPosition = rectTransform.pivot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float mouseX = eventData.position.x - initialMousePosition.x;

        float rotationAngle = mouseX * rotationSpeed * speed;

        rectTransform.pivot = pivotPosition; 
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, -rotationAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EditorBook.Instance.TakeScreenShotCurrentPage();
    }
}

