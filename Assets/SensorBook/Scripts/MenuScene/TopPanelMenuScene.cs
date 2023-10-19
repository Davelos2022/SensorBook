using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopPanelMenuScene : MonoBehaviour
{
    [SerializeField]
    private Button _createBook;
    [SerializeField]
    private Button _addNewBook;
    [SerializeField]
    private TMP_Dropdown _sortBook;

    private void OnEnable()
    {
        _createBook.onClick.AddListener(ClickCreateBTN);
        _addNewBook.onClick.AddListener(ClickAddNewBookBTN);
        _sortBook.onValueChanged.AddListener(OnSizeValueChanged);

        MenuSceneController.Instance._adminOn += ActivateAdminPanel;
        MenuSceneController.Instance._adminOff += DeActivateAdminPanel;
    }

    private void OnDisable()
    {
        _createBook.onClick.RemoveListener(ClickCreateBTN);
        _addNewBook.onClick.RemoveListener(ClickAddNewBookBTN);
        _sortBook.onValueChanged.RemoveListener(OnSizeValueChanged);

        MenuSceneController.Instance._adminOn -= ActivateAdminPanel;
        MenuSceneController.Instance._adminOff -= DeActivateAdminPanel;
    }

    private void ClickCreateBTN()
    {
        MenuSceneController.Instance.EditorBook(null);
    }

    private void ClickAddNewBookBTN()
    {
        MenuSceneController.Instance.AddNewBook_in_Libary();
    }

    private void OnSizeValueChanged(int value)
    {
        if (value == 0)
            MenuSceneController.Instance.SortBook
                (MenuSceneController.SortMode.sortAz);
        if (value == 1)
            MenuSceneController.Instance.SortBook
                (MenuSceneController.SortMode.sortDate);
    }

    private void ActivateAdminPanel()
    {
        AdminPanel(true);
    }

    private void DeActivateAdminPanel()
    {
        AdminPanel(false);
    }
    private void AdminPanel(bool active)
    {
        //_createBook.gameObject.SetActive(active);
        _addNewBook.gameObject.SetActive(active);
    }
}
