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
    private GameObject _bookPrefab;
    [SerializeField]
    private Transform _parentBook;
    [SerializeField]
    private GameObject _imageIfNullBook;
    [SerializeField]
    private GameObject _fadeMenu;

    public enum _sortMode { sortDate, sortAz, sortFavorite };
    private List<Book> _collectionBook = new List<Book>();  //<--- ARRAY!!!! Why? if I need to controll collection (replenish it and delete it book)

    private Book _currentBook; public Book CurrentBook => _currentBook;
    private _sortMode _currentSortMode; public _sortMode SortMode => _currentSortMode;

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
            SortBook(_currentSortMode);

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

    /*not good*/ // -  and now?
    public void SortBook(_sortMode sortMode)
    {
        _currentSortMode = sortMode;

        switch (sortMode)
        {
            case _sortMode.sortAz:
                _collectionBook =_collectionBook.
                    OrderBy(x => x.NameBook).ToList();
                ShowBook();
                break;
            case _sortMode.sortDate:
                _collectionBook =_collectionBook.
                    OrderByDescending(x => x.DataTimeBook.Ticks).ToList();
                ShowBook();
                break;
            case _sortMode.sortFavorite:
                ShowBook(true);
                break;
        }
    }

    /*not good*/ // -  and now?
    private void ShowBook(bool favorite = false)
    {
        for (int x = 0; x < _collectionBook.Count; x++)
        {
            _collectionBook[x].transform.SetSiblingIndex(x);

            if (favorite && !_collectionBook[x].FavoriteBook)
                _collectionBook[x].gameObject.SetActive(false);
            else
                _collectionBook[x].gameObject.SetActive(true);
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
