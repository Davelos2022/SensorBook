using System.Collections.Generic;
using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using VolumeBox.Toolbox;



public class MenuSceneController : Singleton<MenuSceneController>
{
    [SerializeField]
    private GameObject _bookPrefab;
    [SerializeField]
    private Transform _parentBook;
    [SerializeField]
    private GameObject _imageIfNullBook;
    [SerializeField]
    private GameObject _fadeMenu;

    private Scene _loaderScene;

    private List<Book> _collectionBook = new List<Book>();  //<--- ARRAY!!!! Why? if I need to controll collection (replenish it and delete it book) 
    private Book _currentBook;

    public enum SortMode { sortDate, sortAz }
    private SortMode _currentSortState;
    private bool _favoriteShow = false;

    private bool _adminStatus = false;
    public Action _adminOn;
    public Action _adminOff;

    public Book CurrentBook => _currentBook;
    public bool FavoriteShowNow => _favoriteShow;
    public bool AdminStatus => _adminStatus;

    private void OnEnable()
    {
        LoadBook_in_Libary();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F5))
            AdminPanel();
    }

    private void AdminPanel()
    {
        _adminStatus = !_adminStatus;

        if (_adminStatus)
            _adminOn.Invoke();
        else
            _adminOff.Invoke();
    }

    public async void AddNewBook_in_Libary()
    {
        string path = FileManager.SelectPdfFileBrowser();

        if (!string.IsNullOrWhiteSpace(path))
        {
            if (DoesBookExists(Path.GetFileNameWithoutExtension(path)))
                return;

            string newPath = FileManager._bookPath + Path.GetFileName(path);
            await FileManager.CopyFileAsync(path, newPath);

            CreateBook(newPath);
            SortBook(_currentSortState);

            //Notifier.Instance.Notify(NotifyType.Success, " нига добавлена");
            Debug.Log(" нига добавлена");
        }
        else
        {
            return;
        }
    }

    public async void ExportBook(Book book)
    {
        string path = FileManager.SavePdfFileBrowser(book.NameBook);

        if (path.Length > 0)
        {
            await FileManager.ExportBookIn_PDF(path, book);
        }
        else
        {
            return;
        }
    }

    public void LoadBook_in_Libary()
    {
        string[] allPathBook = FileManager.GetCountBookFiles();

        if (allPathBook.Length > 0)
        {
            for (int x = 0; x < allPathBook.Length; x++)
                CreateBook(allPathBook[x]);
        }
        else
        {
            return;
        }
    }

    private bool DoesBookExists(string nameBook)
    {
        for (int x = 0; x < _collectionBook.Count; x++)
        {
            if (_collectionBook[x].NameBook == nameBook)
            {
                //Notifier.Instance.Notify(NotifyType.Error, " нига с таким названием уже имеетс€!");
                Debug.Log(" нига с таким названием уже имеетс€!");
                return true;
            }
        }

        return false;
    }

    public void CreateBook(string pathToBook)
    {
        PDFDocument bookPDF = new PDFDocument(pathToBook, "");
        GameObject bookObj = Instantiate(_bookPrefab, _parentBook);

        Book newBook = bookObj.GetComponent<Book>();
        Texture2D coverBook = bookPDF.Renderer.RenderPageToTexture(bookPDF.GetPage(0));

        newBook.SetupPreviewBook(pathToBook, coverBook);
        _collectionBook.Add(newBook);

        if (_imageIfNullBook.activeSelf)
            _imageIfNullBook.SetActive(false);
    }

    public void SortBook(SortMode sortMode)
    {
        _currentSortState = sortMode;

        switch (sortMode)
        {
            case SortMode.sortAz:
                _collectionBook = _collectionBook.
                    OrderBy(x => x.NameBook).ToList();
                break;
            case SortMode.sortDate:
                _collectionBook = _collectionBook.
                    OrderByDescending(x => x.DataTimeBook.Ticks).ToList();
                break;
        }

        foreach (var book in _collectionBook)
        {
            book.transform.SetSiblingIndex(_collectionBook.
                IndexOf(book));
        }
    }

    public void ShowFavoriteBook()
    {
        _favoriteShow = true;

        foreach (var book in _collectionBook)
        {
            book.gameObject.SetActive(book.FavoriteBook);
        }
    }

    public void ShowAllBook()
    {
        _favoriteShow = false;

        foreach (var book in _collectionBook)
        {
            book.gameObject.SetActive(true);
        }
    }

    public void DeletedBook(Book book)
    {
        _collectionBook.Remove(book);
        FileManager.DeletedFile(book.PathToPDF);

        Destroy(book.gameObject);

        if (_collectionBook.Count <= 0)
            _imageIfNullBook.SetActive(true);
    }

    public void EditorBook(Book editBook = null)
    {
        if (editBook != null)
            _currentBook = editBook;

        StartCoroutine(LoadSceneAsync("SensorBook_EditorBook"));
    }

    public void BookMode(Book book)
    {
        _currentBook = book;
        StartCoroutine(LoadSceneAsync("SensorBook_BookMode"));
    }

    public void ReturnLibary()
    {
        if (_currentBook != null)
            _currentBook = null;

        _fadeMenu.SetActive(true);
        SceneManager.UnloadSceneAsync(_loaderScene);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _fadeMenu.SetActive(false);
        AsyncOperation asyncLoad = SceneManager.
            LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        _loaderScene = SceneManager.GetSceneByName(sceneName);
    }
}
