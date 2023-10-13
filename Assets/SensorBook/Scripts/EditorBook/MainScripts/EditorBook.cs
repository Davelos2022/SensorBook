using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VolumeBox.Toolbox;
using Cysharp.Threading.Tasks;
using System;

public class EditorBook : Singleton<EditorBook>
{
    [SerializeField] private ScreenshotHandlerPage _screenshotHandler;
    [SerializeField] private UndoControllerComponent _undoControllerComponent;
    [SerializeField] private GameObject _loadingScreen;
    [Space]
    [SerializeField] private TMP_InputField _nameBook;
    [SerializeField] private PagePreview _coverBook;
    [Space]
    [SerializeField] private ErrorHighlighter _errorName;
    [SerializeField] private ErrorHighlighter _errorCover;

    [Header("Link to Prefabs")]
    [Space]
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private Transform _pageParent;
    [Space]
    [SerializeField] private GameObject _pagePreviewPrefab;
    [SerializeField] private ScrollRect _scrollPagesView;
    [Space]
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private GameObject _imagePrefab;

    public UndoControllerComponent UndoControllerComponent => _undoControllerComponent;
    private List<Page> _pages = new List<Page>();
    private List<PagePreview> _pagesPreviews = new List<PagePreview>();

    private int _currentIndexPage;
    private Book _editBook = null;

    private int _minPageCount = 2;
    private bool _coverExist = false;

    private void Awake()
    {
        if (MenuSceneController.Instance.CurrentBook != null)
        {
            _editBook = MenuSceneController.Instance.CurrentBook;
            OpenEditBook(_editBook);
        }
        else
        {
            AddPage();
        }

        _currentIndexPage = 0;
        SetCurrentPage(_currentIndexPage);
    }

    public void LoadinScreen(bool active)
    {
        _loadingScreen.SetActive(active);
    }

    private void OpenEditBook(Book book)
    {
        _nameBook.text = book.NameBook;
        _coverBook.SetImage((Texture2D)book.CoverBook.texture);
        _coverExist = true;

        for (int x = 0; x < book.PagesBook.Count; x++)
        {
            AddPage();
            ResizeAreaPageForTexture(book.PagesBook[x].texture, _pages[x].transform);

            CreateImage(book.PagesBook[x].texture, x);
            _pagesPreviews[x].SetImage(book.PagesBook[x].texture);
        }
    }

    private void ResizeAreaPageForTexture(Texture2D texture, Transform page)
    {
        RectTransform pageRectTransform = page.GetComponent<RectTransform>();

        int textureWidth = texture.width / 2;
        int textureHeight = texture.height / 2;

        pageRectTransform.sizeDelta = new Vector2(textureWidth, textureHeight);
    }

    public void Undo()
    {
        _undoControllerComponent.Undo();
    }

    public void Redo()
    {
        _undoControllerComponent.Redo();
    }

    public async void AddImage()
    {
        string pathToImage = FileManager.SelectImageInBrowser();

        if (!string.IsNullOrWhiteSpace(pathToImage))
        {
            Texture2D image = await FileManager.LoadTextureAsync(pathToImage, false);
            CreateImage(image, _currentIndexPage);
        }
        else
        {
            return;
        }
    }

    private void CreateImage(Texture2D texture, int indexPage)
    {
        GameObject image = Instantiate(_imagePrefab, _pages[indexPage].transform);
        image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        RawImage rawImage = image.GetComponent<RawImage>();
        rawImage.texture = texture;

        rawImage.GetComponent<RawImageAspectPreserver>().SetAspect();
    }

    public void AddText()
    {
        GameObject text = Instantiate(_textPrefab, _pages[_currentIndexPage].transform);
        text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        text.GetComponentInChildren<TMP_InputField>().Select();
    }

    public void ClearPage()
    {
        int countObjectInPage = _pages[_currentIndexPage].transform.childCount;

        if (countObjectInPage > 1)
        {
            _pages[_currentIndexPage].ClearPage();
            _pagesPreviews[_currentIndexPage].ClearPreviewPage();
        }
        else
        {
            return;
        }
    }

    public async void SaveOrExportBook(bool export = false)
    {
        if (!CheckingForCorrectnessData())
            return;

        TakeScreenShotCurrentPage();
        await UniTask.Delay(100);

        LoadinScreen(true);

        try
        {
            RectTransform sizePage = _pages[0].PageRectTransform;
            List<Texture2D> pagesTexture = CreatePagesForBook();
            string pathToBook;

            if (export)
            {
                pathToBook = PdfFileManager.SavePdfFileBrowser(_nameBook.text);

                if (!string.IsNullOrEmpty(pathToBook))
                    await PdfFileManager.SaveBookInPDF(pathToBook, pagesTexture, sizePage);

                //Notifier.Instance.Notify(NotifyType.Success, "Книга экспортирована");
                Debug.Log("Книга экспортирована");
            }
            else
            {
                pathToBook = PdfFileManager._bookPath + _nameBook.text + ".pdf";
                await PdfFileManager.SaveBookInPDF(pathToBook, pagesTexture, sizePage);

                MenuSceneController.Instance.CreateBook(pathToBook);
                _undoControllerComponent.ClearHistory();

                //Notifier.Instance.Notify(NotifyType.Success, "Книга cохранена");
                Debug.Log("Книга сохранена");
                MenuSceneController.Instance.ReturnLibary();
            }
        }
        catch (Exception e)
        {
            //Notifier.Instance.Notify(NotifyType.Error, "Произошла ошибка при сохранение файла");
            Debug.LogWarning($"Failed to save book: {e.Message}");
        }

        LoadinScreen(false);
    }

    private List<Texture2D> CreatePagesForBook()
    {
        List<Texture2D> pages = new List<Texture2D>();

        _pagesPreviews.Insert(0, _coverBook);

        for (int x = 0; x < _pagesPreviews.Count; x++)
        {
            if (_pagesPreviews[x].ImageBox.texture != null)
                pages.Add((Texture2D)_pagesPreviews[x].ImageBox.texture);
        }

        return pages;
    }

    private bool CheckingForCorrectnessData()
    {
        if (_pages.Count < _minPageCount)
        {
            Debug.Log("Должно быть минимум две страницы!");
            //Notifier.Instance.Notify(NotifyType.Error, "Должно быть минимум две страницы!");
            return false;
        }
        else if (string.IsNullOrWhiteSpace(_nameBook.text) && !_coverExist)
        {
            Debug.Log("Заполните обязательные поля");
            //Notifier.Instance.Notify(NotifyType.Error, "Заполните обязательные поля");
            _errorName.Highlight();
            _errorCover.Highlight();
            return false;
        }
        else if (string.IsNullOrWhiteSpace(_nameBook.text))
        {
            Debug.Log("Отсутствует название книги");
            //Notifier.Instance.Notify(NotifyType.Error, "Отсутсвует название книги");
            _errorName.Highlight();
            return false;
        }
        else if (!_coverExist)
        {
            Debug.Log("Отсутствует обложка");
            //Notifier.Instance.Notify(NotifyType.Error, "Отсутсвует обложка");
            _errorCover.Highlight();
            return false;
        }


        if (MenuSceneController.Instance.DoesBookExists(_nameBook.text) && _editBook == null)
        {
            return false;
        }
        else if (_editBook)
        {
            MenuSceneController.Instance.DeletedBook(_editBook);
        }

        return true;
    }

    public async void AddCoverBook()
    {
        string pathToImage = FileManager.SelectImageInBrowser();

        if (!string.IsNullOrWhiteSpace(pathToImage))
        {
            Texture2D cover = await FileManager.LoadTextureAsync(pathToImage, false);
            _coverBook.SetImage(cover);
            _coverExist = true;
        }
        else
        {
            return;
        }
    }

    public void AddPage()
    {
        GameObject pagePreviewObj = Instantiate(_pagePreviewPrefab, _scrollPagesView.content);
        PagePreview pagePreview = pagePreviewObj.GetComponent<PagePreview>();

        _pagesPreviews.Add(pagePreview);
        pagePreview.SetNumberPage(_pagesPreviews.Count - 1);

        GameObject pageObj = Instantiate(_pagePrefab, _pageParent);
        Page page = pageObj.GetComponent<Page>();

        _pages.Add(page);
        _pages[_pages.Count - 1].SetNumberPage(_pagesPreviews.Count - 1);
        _pages[_pages.Count - 1].gameObject.SetActive(false);
    }

    public void DeletedPage(int indexPage)
    {
        Destroy(_pages[indexPage]);
        Destroy(_pagesPreviews[indexPage].gameObject);

        _pages.RemoveAt(indexPage);
        _pagesPreviews.RemoveAt(indexPage);

        RefeshPages();
    }

    public void TakeScreenShotCurrentPage()
    {
        if (_undoControllerComponent.GetCountStack())
        {
            _screenshotHandler.TakeScreenshot(_pages[_currentIndexPage].gameObject, _currentIndexPage);
            _undoControllerComponent.ClearHistory();
        }
        else
        {
            return;
        }
    }

    public void SetImagePreviewPage(Texture2D texture, int indexPage)
    {
        _pagesPreviews[indexPage].SetImage(texture);
    }

    public void SetCurrentPage(int indexPage)
    {
        _pages[_currentIndexPage].gameObject.SetActive(false);
        _pagesPreviews[_currentIndexPage].SelectedPage(false);

        _pages[indexPage].gameObject.SetActive(true);
        _pagesPreviews[indexPage].SelectedPage(true);

        _currentIndexPage = indexPage;
    }

    public void SwapPages(int beforeIndex, int afterIndex)
    {
        PagePreview pageInPanel = _pagesPreviews[beforeIndex];
        _pagesPreviews[beforeIndex] = _pagesPreviews[afterIndex];
        _pagesPreviews[afterIndex] = pageInPanel;

        Page page = _pages[beforeIndex];
        _pages[beforeIndex] = _pages[afterIndex];
        _pages[afterIndex] = page;

        _pagesPreviews[beforeIndex].SetNumberPage(beforeIndex);
        _pagesPreviews[afterIndex].SetNumberPage(afterIndex);

        _pages[beforeIndex].SetNumberPage(beforeIndex);
        _pages[afterIndex].SetNumberPage(afterIndex);

        SetCurrentPage(afterIndex);
    }

    private void RefeshPages()
    {
        for (int x = 0; x < _pages.Count; x++)
        {
            _pages[x].SetNumberPage(x);
            _pagesPreviews[x].SetNumberPage(x);
        }
    }
}
