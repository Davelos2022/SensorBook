using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BookModeMenu : MonoBehaviour
{
    [SerializeField] private Transform _panel;
    [SerializeField] private BookPro _book;
    [Space]
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _returMenu;
    [SerializeField] private Button _nextPage;
    [SerializeField] private Button _backPage;
    [Space]
    [SerializeField] private TextMeshProUGUI _leftTextPage;
    [SerializeField] private TextMeshProUGUI _rightTextPage;

    private float animationDuration = 1f;
    private Transform targetPosition;
    private Vector3 initialPosition;

    private bool _showMenuPanel;

    private void Start()
    {
        initialPosition = _panel.position;
        targetPosition = transform;
    }

    private void OnEnable()
    {
        _menuButton.onClick.AddListener(Click_Menu);
        _returMenu.onClick.AddListener(ReturnMenu);

        _nextPage.onClick.AddListener(NextPageClick);
        _backPage.onClick.AddListener(BackPageClick);

        RefechInteractableButton();
    }

    private void OnDisable()
    {
        _menuButton.onClick.RemoveListener(Click_Menu);
        _returMenu.onClick.RemoveListener(ReturnMenu);

        _nextPage.onClick.RemoveListener(NextPageClick);
        _backPage.onClick.RemoveListener(BackPageClick);
    }

    private void Click_Menu()
    {
        _showMenuPanel = !_showMenuPanel;

        if (_showMenuPanel)
            _panel.DOMove(targetPosition.position, animationDuration);
        else
            _panel.DOMove(initialPosition, animationDuration);
    }

    private void NextPageClick()
    {
        _book.NavigationBook(+1);
        RefechInteractableButton();
    }

    private void BackPageClick()
    {
        _book.NavigationBook(-1);
        RefechInteractableButton();
    }

    public void RefechInteractableButton()
    {
        if (_book.CurrentPaper == _book.papers.Count)
            _nextPage.interactable = false;
        else
            _nextPage.interactable = true;

        if (_book.CurrentPaper == 0)
            _backPage.interactable = false;
        else
            _backPage.interactable = true;
    }

    public void SetNumberPage(int number)
    {
        _leftTextPage.text = number.ToString();
        _rightTextPage.text = number + 1.ToString();
    }

    private void ReturnMenu()
    {
        MenuSceneController.Instance.ReturnLibary();
    }
}
