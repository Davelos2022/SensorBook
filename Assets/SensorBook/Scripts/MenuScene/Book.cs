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
    private DateTime _dateTime;

    public RawImage CoverBook => _coverBook;
    public string NameBook => _nameBookTMP.text;
    public string PathToPDF => _pathToPDF;
    public bool FavoriteBook => _favoriteBook;
    public List<Sprite> PagesBook => _pagesBook;
    public DateTime DataTimeBook => _dateTime;

    private void OnEnable()
    {
        _bookBtn.onClick.AddListener(Book_Click);
        _favoriteBookBTN.onClick.AddListener(FavoriteBook_CLick);
        _deletedBTN.onClick.AddListener(DeletedBook_Click);
        _exportBTN.onClick.AddListener(ExportBook_Click);
        _editBookBTN.onClick.AddListener(EditBook_Click);

        MenuSceneController.Instance._adminOn += delegate { AdminPanel(true); };
        MenuSceneController.Instance._adminOff += delegate { AdminPanel(false); };
    }

    private void OnDisable()
    {
        _bookBtn.onClick.RemoveListener(Book_Click);
        _favoriteBookBTN.onClick.RemoveListener(FavoriteBook_CLick);
        _deletedBTN.onClick.RemoveListener(DeletedBook_Click);
        _exportBTN.onClick.RemoveListener(ExportBook_Click);
        _editBookBTN.onClick.RemoveListener(EditBook_Click);


        MenuSceneController.Instance._adminOn -= delegate { AdminPanel(true); };
        MenuSceneController.Instance._adminOff -= delegate { AdminPanel(false); };
    }

    public void SetupPreviewBook(string PathToBook, Texture2D CoverTexture)
    {
        if (MenuSceneController.Instance.AdminStatus)
            AdminPanel(true);

        _nameBookTMP.text = Path.GetFileNameWithoutExtension(PathToBook);
        _coverBook.texture = CoverTexture;
        _pathToPDF = PathToBook;
        _dateTime = FileManager.GetDateCreation(PathToBook);
    }

    private void SetupPagesBook()
    {
        _pagesBook = FileManager.OpenPDF_file(_pathToPDF);
    }

    private void Book_Click()
    {
        SetupPagesBook();
        MenuSceneController.Instance.BookMode(this);
    }

    private void DeletedBook_Click()
    {
        MenuSceneController.Instance.DeletedBook(this);
    }

    private void ExportBook_Click()
    {
        //if (_pagesBook.Count <= 0)
        //    SetupPagesBook();

        //MenuSceneController.Instance.ExportBook(this);

        Debug.Log("Пока не работает!");
    }

    private void EditBook_Click()
    {
        Debug.Log("Пока не работает!");
    }
    private void FavoriteBook_CLick()
    {
        _favoriteBook = !_favoriteBook;
        _selectionFavoriteBTN.SetActive(_favoriteBook);

        MenuSceneController.Instance.ShowBook();
    }

    public void AdminPanel(bool active)
    {
        _deletedBTN.gameObject.SetActive(active);
        _editBookBTN.gameObject.SetActive(active);
        _exportBTN.gameObject.SetActive(active);
    }
}
