using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using VolumeBox.Toolbox.UIInformer;

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
    private enum _stateBook { Show, Edit };

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
        _dateTimeCreation = PdfFileManager.
            GetDateCreation(PathToBook);

        CheckFavoriteBook(_nameBookTMP.text);
    }

    private void Book_Click()
    {
        StartCoroutine(LoadBook(_stateBook.Show));
    }

    private void ExportBook_Click()
    {
        MenuSceneController.Instance.ExportBook(this);
    }

    private void EditBook_Click()
    {
        StartCoroutine(LoadBook(_stateBook.Edit));
    }

    private void DeletedBook_Click()
    {
        Info.Instance.ShowBox($"Вы действительно хотите удалить книгу?",
            DeletedBook, null, null, "Удалить книгу", "Отмена");
    }

    private void DeletedBook()
    {
        MenuSceneController.Instance.DeletedBook(this);
    }

    private void FavoriteBook_CLick()
    {
        _favoriteBook = !_favoriteBook;
        _selectionFavoriteBTN.SetActive(_favoriteBook);

        SaveAndDeletedFavoriteBook(_favoriteBook, _nameBookTMP.text);

        if (MenuSceneController.Instance.FavoriteShowNow)
            gameObject.SetActive(_favoriteBook);
    }

    private void CheckFavoriteBook(string nameBook)
    {
        if (PlayerPrefs.HasKey($"Favorite_{nameBook}"))
            FavoriteBook_CLick();
        else
            return;
    }

    private void SaveAndDeletedFavoriteBook(bool favorite, string nameBook)
    {
        if (favorite && !PlayerPrefs.HasKey($"Favorite_{nameBook}"))
            PlayerPrefs.SetString($"Favorite_{nameBook}", nameBook);
        else
            PlayerPrefs.DeleteKey($"Favorite_{nameBook}");
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

    private IEnumerator LoadBook(_stateBook stateBook)
    {
        _loadingScreen.SetActive(true);

        yield return new WaitForSeconds(1f);
        _pagesBook = PdfFileManager.OpenPDFfile(_pathToPDF);

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
            default:
                break;
        }
    }
}
