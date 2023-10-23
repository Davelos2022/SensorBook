using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] BookModeMenu _bookModeMenu;
    [SerializeField] private float _swipeThreshold = 100f;

    private Vector2 _startPos;
    private Vector2 _endPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _endPos = eventData.position;

        float swipeDistance = Vector2.Distance(_startPos, _endPos);
        if (swipeDistance >= _swipeThreshold)
        {
            Vector2 swipeDirection = (_endPos - _startPos).normalized;
            if (swipeDirection.y < 0 || swipeDirection.y > 0)
            {
                _bookModeMenu.ShowMenu();   
            }
            else if (swipeDirection.x < 0)
            {
                _bookModeMenu.ShowMenu();
            }
        }
        else
        {
            _bookModeMenu.ShowMenu();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
