using System.Collections.Generic;
using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using VolumeBox.Toolbox;
using Cysharp.Threading.Tasks;
using TMPro;

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
    [Space]
    [SerializeField]
    private GameObject _loadScreen;
    [SerializeField]
    private TextMeshProUGUI _loadText;

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
        string path = PdfFileManager.SelectPdfFileBrowser();

        if (!string.IsNullOrWhiteSpace(path))
        {
            if (DoesBookExists(Path.GetFileNameWithoutExtension(path)))
                return;

            string newPath = PdfFileManager._bookPath + Path.GetFileName(path);
            await FileManager.CopyFileAsync(path, newPath);

            CreateBook(newPath);
            SortBook(_currentSortState);

            //Notifier.Instance.Notify(NotifyType.Success, "Книга добавлена");
            Debug.Log("Книга добавлена");
        }
        else
        {
            return;
        }
    }

    public async void ExportBook(Book book)
    {
        string path = PdfFileManager.SavePdfFileBrowser(book.NameBook);

        if (path.Length > 0)
        {
            await FileManager.CopyFileAsync(book.PathToPDF, path);
            //Notifier.Instance.Notify(NotifyType.Success, "Книга экспортирвоана");
            Debug.Log("Книга экспортирвоана");
        }
        else
        {
            return;
        }
    }

    public void LoadBook_in_Libary()
    {
        string[] allPathBook = PdfFileManager.GetCountBookFiles();

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

    public bool DoesBookExists(string nameBook)
    {
        for (int x = 0; x < _collectionBook.Count; x++)
        {
            if (_collectionBook[x].NameBook == nameBook)
            {
                //Notifier.Instance.Notify(NotifyType.Error, "Книга с таким названием уже имеется!");
                Debug.Log("Книга с таким названием уже имеется!");
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
        PdfFileManager.DeletedFile(book.PathToPDF);

        Destroy(book.gameObject);

        if (_collectionBook.Count <= 0)
            _imageIfNullBook.SetActive(true);
    }

    public async void EditorBook(Book editBook = null)
    {
        if (editBook != null)
            _currentBook = editBook;

        await LoadSceneAsync("SensorBook_EditorBook");
    }

    public async void BookMode(Book book)
    {
        _currentBook = book;
        await LoadSceneAsync("SensorBook_BookMode");
    }

    public void ReturnLibary()
    {
        if (_currentBook != null)
            _currentBook = null;

        _fadeMenu.SetActive(true);
        SortBook(_currentSortState);

        SceneManager.UnloadSceneAsync(_loaderScene);
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        _fadeMenu.SetActive(false);
        AsyncOperation asyncLoad = SceneManager.
            LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
        }

        _loaderScene = SceneManager.GetSceneByName(sceneName);
        LoadScreen(false);
    }

    public void LoadScreen(bool active, string textLoad = null)
    {
        _loadScreen.SetActive(active);
        _loadText.text = textLoad;
    }
}
