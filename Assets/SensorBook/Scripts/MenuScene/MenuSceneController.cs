using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;
using VolumeBox.Toolbox;
using Cysharp.Threading.Tasks;

public class MenuSceneController : Singleton<MenuSceneController>
{
    [SerializeField]
    private GameObject _bookPrefab;
    [SerializeField]
    private string[] _nameBookDefault;
    [SerializeField]
    private Transform _parentBook;
    [SerializeField]
    private GameObject _backDrop;
    [SerializeField]
    private GameObject _fadeMenu;

    private List<Book> _collectionBook = new List<Book>();  //<--- ARRAY!!!! Why? if I need to controll collection (replenish it and delete it book) 
    private BookFavorite _bookFavorite;
    private Book _currentBook;

    public enum SortMode { sortDate, sortAz }
    private SortMode _currentSortState;
    private bool _favoriteShow = false;

    private Scene _loaderScene;

    private bool _adminStatus = false;
    public Action _adminOn;
    public Action _adminOff;

    public Book CurrentBook => _currentBook;
    public bool FavoriteShowNow => _favoriteShow;
    public bool AdminStatus => _adminStatus;

    private void Awake()
    {
        if (HasInstance)  // This "if"  - The conflict with the Singleton is due to the Main scene, when it closes, it reverts and creates a scene and launches this method
            LoadBookInLibary();
    }

    private void OnDisable()
    {
        SaveFavoriteBooksToFile();
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

            LoadScreenBook.Instance.LoadScreen(true, "Добавляем книгу в библиотеку...");
            string newPath = PdfFileManager._bookPath + Path.GetFileName(path);
            await FileManager.CopyFileAsync(path, newPath);

            CreateBook(newPath);
            SortBook(_currentSortState);

            LoadScreenBook.Instance.LoadScreen(false);
            Notifier.Instance.Notify(NotifyType.Success, "Книга добавлена");
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

        if (!string.IsNullOrWhiteSpace(path))
        {
            await FileManager.CopyFileAsync(book.PathToPDF, path);
            Notifier.Instance.Notify(NotifyType.Success, "Книга экспортирвоана");
            Debug.Log("Книга экспортирвоана");
        }
        else
        {
            return;
        }
    }

    public void LoadBookInLibary()
    {
        string[] allPathBook = PdfFileManager.GetCountBookFiles();
        LoadFileFavoriteBook();

        _currentSortState = SortMode.sortAz;

        if (allPathBook.Length > 0)
        {
            for (int x = 0; x < allPathBook.Length; x++)
                CreateBook(allPathBook[x]);

            SortBook(_currentSortState);
        }
        else
        {
            return;
        }
    }

    private void LoadFileFavoriteBook()
    {
        string pathToJson = PdfFileManager._bookPath + "BookFavorite.json";

        if (File.Exists(pathToJson))
        {
            string json = File.ReadAllText(pathToJson);
            _bookFavorite = JsonUtility.FromJson<BookFavorite>(json);
        }
        else
        {
            return;
        }
    }

    private void SaveFavoriteBooksToFile()
    {
        if (_bookFavorite != null)
            _bookFavorite.NameBook.Clear();
        else
            _bookFavorite = new BookFavorite();

        foreach (var book in _collectionBook)
        {
            if (book.Favorite)
                _bookFavorite.NameBook.Add(book.NameBook);
        }

        string json = JsonUtility.ToJson(_bookFavorite);
        File.WriteAllText(PdfFileManager._bookPath + "BookFavorite.json", json);
    }

    public bool DoesBookExists(string nameBook)
    {
        for (int x = 0; x < _collectionBook.Count; x++)
        {
            if (_collectionBook[x].NameBook == nameBook)
            {
                Notifier.Instance.Notify(NotifyType.Error, "Книга с таким названием уже существует!");
                Debug.Log("Книга с таким названием уже существует!");
                return true;
            }
        }

        return false;
    }

    private bool CheckDefaultBook(string nameBook)
    {
        if (_nameBookDefault != null)
        {
            for (int x = 0; x < _nameBookDefault.Length; x++)
            {
                if (_nameBookDefault[x].ToLower() == nameBook.ToLower())
                    return true;
            }
        }

        return false;
    }

    private bool CheckFavoriteBook(string NameBook)
    {
        if (_bookFavorite != null)
        {
            foreach (var book in _bookFavorite.NameBook)
            {
                if (book == NameBook)
                    return true;
            }

            return false;
        }
        else
        {
            return false;
        }
    }

    public void CreateBook(string pathToBook)
    {
        string nameBook = Path.GetFileNameWithoutExtension(pathToBook);
        bool defaultBook = CheckDefaultBook(nameBook);
        bool favoriteBook = CheckFavoriteBook(nameBook);

        GameObject bookObj = Instantiate(_bookPrefab, _parentBook);
        Book newBook = bookObj.GetComponent<Book>();
        newBook.SetupPreviewBook(nameBook, pathToBook, defaultBook, favoriteBook);

        _collectionBook.Add(newBook);
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
        int countFovorite = 0;
        _favoriteShow = true;

        foreach (var book in _collectionBook)
        {
            book.gameObject.SetActive(book.Favorite);

            if (book.Favorite)
                countFovorite++;
        }

        if (countFovorite == 0)
            _backDrop.SetActive(true);
    }


    public void ShowAllBook()
    {
        if (_backDrop.activeSelf)
            _backDrop.SetActive(false);

        _favoriteShow = false;

        foreach (var book in _collectionBook)
        {
            book.gameObject.SetActive(true);
        }
    }

    public void DeletedBook(Book book)
    {
        _collectionBook.Remove(book);
        PdfFileManager.DeleteFile(book.PathToPDF);

        Notifier.Instance.Notify(NotifyType.Success, $"Книга {book.NameBook} удалена!");
        Destroy(book.gameObject);
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
        {
            _currentBook.ClearPages();
            _currentBook = null;
        }

        _fadeMenu.SetActive(true);
        SortBook(_currentSortState);

        SceneManager.UnloadSceneAsync(_loaderScene);
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        if (_currentBook != null)
        {
            LoadScreenBook.Instance.LoadScreen(true, "Загрузка страниц книги...");
            await _currentBook.LoadPages();
        }

        AsyncOperation asyncLoad = SceneManager.
            LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
        }

        _fadeMenu.SetActive(false);
        _loaderScene = SceneManager.GetSceneByName(sceneName);
        LoadScreenBook.Instance.LoadScreen(false);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}