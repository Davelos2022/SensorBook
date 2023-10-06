using System.Collections.Generic;
using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System;

public class MenuSceneController : Singleton<MenuSceneController>
{
    [SerializeField]
    private GameObject _bookPrefabs;
    [SerializeField]
    private Transform _parentBook;
    [SerializeField]
    private GameObject _imageIfNullBook;
    [SerializeField]
    private GameObject _loadingBar;

    private List<Book> _collectionBook = new List<Book>();  //<--- ARRAY!!!!
    private Book _currentBook; public Book CurrentBook => _currentBook;
    private bool _favoriteMode; public bool FavoriteMode { set { _favoriteMode = value; } }
    private bool _sortDate; public bool SortDate { set { _sortDate = value; } }

    private bool _adminStatus = false; public bool AdminStatus => _adminStatus;
    public Action _adminOn;
    public Action _adminOff;

    private Scene _loaderScene;

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
            SortBook();
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
                Debug.Log(" нига с таким названием уже имеетс€!");
                return true;
            }
        }

        return false;
    }

    public void CreateBook(string pathToBook)
    {
        PDFDocument bookPDF = new PDFDocument(pathToBook, "");
        GameObject bookObj = Instantiate(_bookPrefabs, _parentBook);

        Book newBook = bookObj.GetComponent<Book>();
        Texture2D coverBook = bookPDF.Renderer.RenderPageToTexture(bookPDF.GetPage(0));

        newBook.SetupPreviewBook(pathToBook, coverBook);
        _collectionBook.Add(newBook);

        if (_collectionBook.Count > 0)
            _imageIfNullBook.SetActive(false);
    }

    /*not good*/
    public void SortBook()
    {
        if (_sortDate)
            _collectionBook = _collectionBook.OrderByDescending(x => x.DataTimeBook.Ticks).ToList();
        else
            _collectionBook = _collectionBook.OrderBy(x => x.NameBook).ToList();

        for (int x = 0; x < _collectionBook.Count; x++)
        {
            _collectionBook[x].transform.SetSiblingIndex(x);
        }
    }

    /*not good*/
    public void ShowBook()
    {
        for (int x = 0; x < _collectionBook.Count; x++)
        {
            if (_favoriteMode)
            {
                if (!_collectionBook[x].FavoriteBook)
                    _collectionBook[x].gameObject.SetActive(false);
            }
            else
            {
                _collectionBook[x].gameObject.SetActive(true);
            }
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

        SceneManager.UnloadSceneAsync(_loaderScene);
    }

    public void LoadingBar(bool active)
    {
        _loadingBar.SetActive(active);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        _loaderScene = SceneManager.GetSceneByName(sceneName);
        LoadingBar(false);
    }
}
