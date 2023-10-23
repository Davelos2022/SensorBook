using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BookModeMenu : MonoBehaviour
{
    [SerializeField] private Transform _panel;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private AutoFlip _autoFlip;
    [Space]
    [SerializeField] private Button _returMenu;
    [SerializeField] private Button _nextPage;
    [SerializeField] private Button _backPage;

    private float _animationDuration = 1f;
    private Vector3 _initialPosition;
    private bool _showMenuPanel;

    private void Start()
    {
        _initialPosition = _panel.position;
    }

    private void OnEnable()
    {
        _returMenu.onClick.AddListener(ReturnMenu);
        _nextPage.onClick.AddListener(NextPageClick);
        _backPage.onClick.AddListener(BackPageClick);
    }

    private void OnDisable()
    {
        _returMenu.onClick.RemoveListener(ReturnMenu);
        _nextPage.onClick.RemoveListener(NextPageClick);
        _backPage.onClick.RemoveListener(BackPageClick);
    }

    public void ShowMenu()
    {
        _showMenuPanel = !_showMenuPanel;

        if (_showMenuPanel)
            _panel.DOMove(_targetPosition.position, _animationDuration);
        else
            _panel.DOMove(_initialPosition, _animationDuration);
    }

    private void NextPageClick()
    {
        _autoFlip.FlipRightPage();
    }

    private void BackPageClick()
    {
        _autoFlip.FlipLeftPage();
    }

    private void ReturnMenu()
    {
        MenuSceneController.Instance.ReturnLibary();
    }
}
