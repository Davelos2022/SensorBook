using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BookModeMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform _panel;
    [SerializeField] private Button _returMenu;

    private float animationDuration = 1f;
    private Transform targetPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = _panel.position;
        targetPosition = transform;
    }

    private void OnEnable()
    {
        _returMenu.onClick.AddListener(ReturnMenu);
    }

    private void OnDisable()
    {
        _returMenu.onClick.RemoveListener(ReturnMenu);
    }

    private void ReturnMenu()
    {
        MenuSceneController.Instance.ReturnLibary();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _panel.DOMove(targetPosition.position, animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _panel.DOMove(initialPosition, animationDuration);
    }
}
