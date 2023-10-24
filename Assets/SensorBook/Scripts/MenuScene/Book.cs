using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VolumeBox.Toolbox.UIInformer;
using Cysharp.Threading.Tasks;

[Serializable]
public class Book : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameBookTMP;
    [SerializeField]
    private RawImage _coverBook;
    [SerializeField]
    private RawImageAspectPreserver _coverImageAspect;
    [SerializeField]
    private Button _bookBTN;
    [SerializeField]
    private Button _favoriteBookBTN;
    [SerializeField]
    private GameObject _selectionFavoriteBTN;
    [Space]
    [Header("Admin Panel")]
    [SerializeField]
    private Button _deletedBTN;
    [SerializeField]
    private Button _exportBTN;
    [SerializeField]
    private Button _editBookBTN;

    private string _pathToPDF;
    private List<Texture2D> _pagesBook = new List<Texture2D>();
    private bool _defaultBook;
    private bool _favoriteBook;
    private DateTime _dateTimeCreation;

    public string NameBook => _nameBookTMP.text;
    public RawImage CoverBook => _coverBook;
    public string PathToPDF => _pathToPDF;
    public List<Texture2D> PagesBook => _pagesBook;
    public bool DefaultBook => _defaultBook;
    public bool Favorite => _favoriteBook;
    public DateTime DataTimeBook => _dateTimeCreation;


    private void Start()
    {
        _bookBTN.onClick.AddListener(ShowBook);
        _favoriteBookBTN.onClick.AddListener(FavoriteBook);
        _deletedBTN.onClick.AddListener(DeletedBook_MessagBox);
        _exportBTN.onClick.AddListener(ExportBook);
        _editBookBTN.onClick.AddListener(EditBook);

        MenuSceneController.Instance._adminOn += ActivateAdminPanel;
        MenuSceneController.Instance._adminOff += DeActiveAdminPanel;
    }

    private void OnDestroy()
    {
        _bookBTN.onClick.RemoveListener(ShowBook);
        _favoriteBookBTN.onClick.RemoveListener(FavoriteBook);
        _deletedBTN.onClick.RemoveListener(DeletedBook_MessagBox);
        _exportBTN.onClick.RemoveListener(ExportBook);
        _editBookBTN.onClick.RemoveListener(EditBook);

        MenuSceneController.Instance._adminOn -= ActivateAdminPanel;
        MenuSceneController.Instance._adminOff -= DeActiveAdminPanel;
    }

    public void SetupPreviewBook(string nameBook, string pathToBook, bool defaultBook, bool favoriteBook)
    {
        if (MenuSceneController.Instance.AdminStatus)
            AdminPanel(true);

        if (favoriteBook)
            FavoriteBook();

        _pathToPDF = pathToBook;
        _nameBookTMP.text = nameBook;
        _defaultBook = defaultBook;
        _coverBook.texture = PdfFileManager.GetCoverBook(pathToBook);
        _dateTimeCreation = PdfFileManager.GetDateCreationBook(pathToBook);

        //_coverImageAspect.SetAspect();
    }

    private void ShowBook()
    {
        MenuSceneController.Instance.BookMode(this);
    }

    private void EditBook()
    {
        MenuSceneController.Instance.EditorBook(this);
    }

    private void ExportBook()
    {
        MenuSceneController.Instance.ExportBook(this);
    }

    private void DeletedBook()
    {
        MenuSceneController.Instance.DeletedBook(this);
    }

    private void DeletedBook_MessagBox()
    {
        Info.Instance.ShowBox($"Вы действительно хотите удалить книгу?",
            DeletedBook, null, null, "Удалить книгу", "Отмена");
    }

    private void FavoriteBook()
    {
        _favoriteBook = !_favoriteBook;
        _selectionFavoriteBTN.SetActive(_favoriteBook);

        if (MenuSceneController.Instance.FavoriteShowNow)
            MenuSceneController.Instance.ShowFavoriteBook();
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
        if (!_defaultBook)
        {
            _deletedBTN.gameObject.SetActive(active);
            //_editBookBTN.gameObject.SetActive(active);
            //_exportBTN.gameObject.SetActive(active);
        }
    }

    public async UniTask LoadPages()
    {
        _pagesBook = await PdfFileManager.OpenPdfFile(_pathToPDF);
    }

    public void ClearPages()
    {
        foreach (var pageTexture in _pagesBook)
            Destroy(pageTexture);

        _pagesBook.Clear();
    }
}
