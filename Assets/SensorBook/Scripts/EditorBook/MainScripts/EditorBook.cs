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

    private List<Page> _pages = new List<Page>();
    private List<PagePreview> _pagesPreviews = new List<PagePreview>(); public List<PagePreview> PagePreviews => _pagesPreviews;

    private Book _editBook = null;
    private int _currentIndexPage;
    private bool _coverExist = false;

    private void Awake()
    {
        _nameBook.onValueChanged.AddListener(OnInputValueChanged);

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

    private void OnInputValueChanged(string value)
    {
        value = value.Replace("/", "").Replace("\\", "").
            Replace(".", "").Replace(",", "");

        _nameBook.text = value;
    }

    private void OpenEditBook(Book book)
    {
        _nameBook.text = book.NameBook;
        _coverBook.SetImage((Texture2D)book.CoverBook.texture);
        _coverExist = true;

        for (int x = 0; x < book.PagesBook.Count; x++)
        {
            AddPage();
            ResizeAreaPageForTexture(book.PagesBook[x], _pages[x].transform);

            CreateImage(book.PagesBook[x], x, true);
            _pagesPreviews[x].SetImage(book.PagesBook[x]);
        }
    }

    private void ResizeAreaPageForTexture(Texture2D texture, Transform page)
    {
        RectTransform pageRectTransform = page.GetComponent<RectTransform>();

        int textureWidth = texture.width / 2;
        int textureHeight = texture.height / 2;

        pageRectTransform.sizeDelta = new Vector2(textureWidth, textureHeight);
    }


    public async void AddImage()
    {
        string pathToImage = FileManager.SelectImageInBrowser();

        if (!string.IsNullOrWhiteSpace(pathToImage))
        {
            Texture2D image = await FileManager.LoadTextureAsync(pathToImage, false);
            CreateImage(image, _currentIndexPage);
            TakeScreenShotCurrentPage();
        }
        else
        {
            return;
        }
    }

    private void CreateImage(Texture2D texture, int indexPage, bool openEdit = false)
    {
        GameObject image = Instantiate(_imagePrefab, _pages[indexPage].transform);
        image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        RawImage rawImage = image.GetComponent<RawImage>();
        rawImage.texture = texture;

        rawImage.GetComponent<RawImageAspectPreserver>().SetAspect();

        if (!openEdit)
            UndoRedoSystem.Instance.AddAction(new CreateDeleteObjectAction(image));

    }

    public void AddText()
    {
        GameObject text = Instantiate(_textPrefab, _pages[_currentIndexPage].transform);
        text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        text.GetComponentInChildren<TMP_InputField>().Select();
        TakeScreenShotCurrentPage();

        UndoRedoSystem.Instance.AddAction(new CreateDeleteObjectAction(text));
    }

    public void ClearPage()
    {
        int countObjectInPage = _pages[_currentIndexPage].transform.childCount;

        if (countObjectInPage > 1)
        {
            _pages[_currentIndexPage].ClearPage();
            _pagesPreviews[_currentIndexPage].SetImage(null);

            UndoRedoSystem.Instance.ClearHistory();
            TakeScreenShotCurrentPage();
        }
        else
        {
            return;
        }
    }

    public void DeletedObject(GameObject gameObject)
    {
        //Destroy(gameObject);
        UndoRedoSystem.Instance.AddAction(new CreateDeleteObjectAction(gameObject,true));
        gameObject.SetActive(false);

        TakeScreenShotCurrentPage();
    }

    public async void SaveOrExportBook(bool export = false)
    {
        if (!CheckingForCorrectnessData())
            return;

        if (MenuSceneController.Instance.DoesBookExists(_nameBook.text) && _editBook == null)
            return;
        else if (_editBook != null)
            MenuSceneController.Instance.DeletedBook(_editBook);


        try
        {
            RectTransform sizePage = _pagePrefab.GetComponent<RectTransform>();
            List<Texture2D> pagesTexture = CreatePagesForBook();
            string pathToBook;

            if (export)
            {
                pathToBook = PdfFileManager.SavePdfFileBrowser(_nameBook.text);

                if (!string.IsNullOrEmpty(pathToBook))
                {
                    await PdfFileManager.SaveBookInPDF(pathToBook, pagesTexture, sizePage);

                    Notifier.Instance.Notify(NotifyType.Success, "Книга экспортирована");
                    Debug.Log("Книга экспортирована");
                }
            }
            else
            {
                pathToBook = PdfFileManager._bookPath + _nameBook.text + ".pdf";
                await PdfFileManager.SaveBookInPDF(pathToBook, pagesTexture, sizePage);

                MenuSceneController.Instance.CreateBook(pathToBook);
                MenuSceneController.Instance.ReturnLibary();

                Notifier.Instance.Notify(NotifyType.Success, "Книга cохранена");
                Debug.Log("Книга сохранена");
            }
        }
        catch (Exception e)
        {
            Notifier.Instance.Notify(NotifyType.Error, "Произошла ошибка при сохранение файла");
            Debug.LogWarning($"Failed to save book: {e.Message}");
        }

    }

    private List<Texture2D> CreatePagesForBook()
    {
        List<Texture2D> pages = new List<Texture2D>();

        pages.Add((Texture2D)_coverBook.ImageBox.texture);

        for (int x = 0; x < _pagesPreviews.Count; x++)
        {
            if (_pagesPreviews[x].ImageBox.texture = null)
                _pagesPreviews[x].SetImage(null);

                pages.Add((Texture2D)_pagesPreviews[x].ImageBox.texture);
        }

        return pages;
    }

    private bool CheckingForCorrectnessData()
    {
        CorrectnessData.Cheeck cheeckInput = CorrectnessData.
            CheckData(_pages.Count, _coverExist, _nameBook.text);

        switch (cheeckInput)
        {
            case CorrectnessData.Cheeck.NotMinPage:
                Debug.Log("Должно быть минимум две страницы!");
                Notifier.Instance.Notify(NotifyType.Error, "Должно быть минимум две страницы!");
                return false;
            case CorrectnessData.Cheeck.NullAll:
                Debug.Log("Заполните обязательные поля");
                Notifier.Instance.Notify(NotifyType.Error, "Заполните обязательные поля");
                _errorName.Highlight();
                _errorCover.Highlight();
                return false;
            case CorrectnessData.Cheeck.NullName:
                Debug.Log("Отсутствует название книги");
                Notifier.Instance.Notify(NotifyType.Error, "Отсутсвует название книги");
                _errorName.Highlight();
                return false;
            case CorrectnessData.Cheeck.NotCover:
                Debug.Log("Отсутствует обложка");
                Notifier.Instance.Notify(NotifyType.Error, "Отсутсвует обложка");
                _errorCover.Highlight();
                return false;
            case CorrectnessData.Cheeck.Completed:
                return true;
            default:
                return false;
        }
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
        //_pages[_pages.Count - 1].SetNumberPage(_pagesPreviews.Count - 1);
        _pages[_pages.Count - 1].gameObject.SetActive(false);
        RefeshPages();
    }

    public void DeletePage(int indexPage)
    {
        Destroy(_pages[indexPage]);
        Destroy(_pagesPreviews[indexPage].gameObject);

        _pages.RemoveAt(indexPage);
        _pagesPreviews.RemoveAt(indexPage);

        RefeshPages();
    }

    public void TakeScreenShotCurrentPage()
    {
        _screenshotHandler.TakeScreenshot(_pages[_currentIndexPage].gameObject, _currentIndexPage);
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
