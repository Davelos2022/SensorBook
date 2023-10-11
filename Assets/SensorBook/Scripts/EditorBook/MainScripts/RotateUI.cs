using UnityEngine;
using UnityEngine.EventSystems;

public class RotateUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform rectTransform;

    private Vector2 pivotPosition;
    private Vector2 initialMousePosition;

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
        float rotationSpeed = 2f; 

        float mouseX = eventData.position.x - initialMousePosition.x;

        float rotationAngle = -mouseX * rotationSpeed;

        rectTransform.pivot = pivotPosition; 
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}

