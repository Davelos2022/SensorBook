using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class Book : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameBookTMP;
    [SerializeField]
    private RawImage _coverBook;
    [SerializeField]
    private Button _bookBtn;
    [SerializeField]
    private Button _favoriteBookBTN;
    [SerializeField]
    private GameObject _selectionFavoriteBTN;
    [SerializeField]
    private GameObject _loadingScreen;
    [Space]
    [Header("Admin Panel")]
    [SerializeField]
    private Button _deletedBTN;
    [SerializeField]
    private Button _exportBTN;
    [SerializeField]
    private Button _editBookBTN;

    private string _pathToPDF;
    private List<Sprite> _pagesBook = new List<Sprite>();
    private bool _favoriteBook;
    private DateTime _dateTimeCreation;
    private enum _stateBook { Show, Edit, Export };

    public RawImage CoverBook => _coverBook;
    public string NameBook => _nameBookTMP.text;
    public string PathToPDF => _pathToPDF;
    public bool FavoriteBook => _favoriteBook;
    public List<Sprite> PagesBook => _pagesBook;
    public DateTime DataTimeBook => _dateTimeCreation;

    private void OnEnable()
    {
        _bookBtn.onClick.AddListener(Book_Click);
        _favoriteBookBTN.onClick.AddListener(FavoriteBook_CLick);
        _deletedBTN.onClick.AddListener(DeletedBook_Click);
        _exportBTN.onClick.AddListener(ExportBook_Click);
        _editBookBTN.onClick.AddListener(EditBook_Click);

        MenuSceneController.Instance._adminOn += ActivateAdminPanel;
        MenuSceneController.Instance._adminOff += DeActiveAdminPanel;
    }

    private void OnDisable()
    {
        _bookBtn.onClick.RemoveListener(Book_Click);
        _favoriteBookBTN.onClick.RemoveListener(FavoriteBook_CLick);
        _deletedBTN.onClick.RemoveListener(DeletedBook_Click);
        _exportBTN.onClick.RemoveListener(ExportBook_Click);
        _editBookBTN.onClick.RemoveListener(EditBook_Click);

        MenuSceneController.Instance._adminOn -= ActivateAdminPanel;
        MenuSceneController.Instance._adminOff -= DeActiveAdminPanel;
    }

    public void SetupPreviewBook(string PathToBook, Texture2D CoverTexture)
    {
        if (MenuSceneController.Instance.AdminStatus)
            AdminPanel(true);

        _nameBookTMP.text = Path.GetFileNameWithoutExtension(PathToBook);
        _coverBook.texture = CoverTexture;
        _pathToPDF = PathToBook;
        _dateTimeCreation = FileManager.
            GetDateCreation(PathToBook);
    }

    private void Book_Click()
    {
        StartCoroutine(LoadPagesBook(_stateBook.Show));
    }

    private void ExportBook_Click()
    {
        StartCoroutine(LoadPagesBook(_stateBook.Export));
    }

    private void EditBook_Click()
    {
        StartCoroutine(LoadPagesBook(_stateBook.Edit));
    }

    private void DeletedBook_Click()
    {
        MenuSceneController.Instance.DeletedBook(this);
    }

    private void FavoriteBook_CLick()
    {
        _favoriteBook = !_favoriteBook;
        _selectionFavoriteBTN.SetActive(_favoriteBook);

        MenuSceneController.Instance.
            SortBook(MenuSceneController.Instance.SortMode);
    }

    private void ActivateAdminPanel()
    {
        AdminPanel(true);
    }

    private void DeActiveAdminPanel()
    {
        AdminPanel(false);
    }

    private void AdminPanel(bool active)
    {
        _deletedBTN.gameObject.SetActive(active);
        _editBookBTN.gameObject.SetActive(active);
        _exportBTN.gameObject.SetActive(active);
    }

    private IEnumerator LoadPagesBook(_stateBook stateBook)
    {
        _loadingScreen.SetActive(true);

        yield return new WaitForSeconds(1f);
        _pagesBook = FileManager.OpenPDF_file(_pathToPDF);

        _loadingScreen.SetActive(false);

        switch (stateBook)
        {
            case _stateBook.Show:
                MenuSceneController.Instance.
                    BookMode(this);
                break;
            case _stateBook.Edit:
                MenuSceneController.Instance.
                    EditorBook(this);
                break;
            case _stateBook.Export:
                MenuSceneController.Instance.
                    ExportBook(this);
                break;
            default:
                break;
        }
    }
}
