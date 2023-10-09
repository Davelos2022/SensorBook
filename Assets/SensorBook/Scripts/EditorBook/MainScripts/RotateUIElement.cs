using UnityEngine;
using UnityEngine.EventSystems;

public class RotateUIElement : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform rectTransform;

    private Vector2 pivotPosition;
    private Vector2 initialMousePosition;
    private Vector2 initialUIPosition;

    private void OnEnable()
    {
        pivotPosition = rectTransform.pivot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialMousePosition = eventData.position;
        initialUIPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float rotationSpeed = 5f; 

        float mouseY = eventData.position.y - initialMousePosition.y;

        float rotationAngle = -mouseY * rotationSpeed;

        rectTransform.pivot = pivotPosition; 
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}

