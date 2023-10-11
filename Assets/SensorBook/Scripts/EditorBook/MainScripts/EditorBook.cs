using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using VolumeBox.Toolbox;

public class EditorBook : Singleton<EditorBook>
{
    [SerializeField] private ScreenshotHandler _screenshotHandler;
    [SerializeField] private UndoControllerComponent _undoControllerComponent;
    [Space]
    [SerializeField] private TMP_InputField _nameBook;
    [SerializeField] private RawImage _coverImageBox;
    [Space]
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private Transform _pageParent;
    [Space]
    [SerializeField] private GameObject _pageInScroolViewPrefab;
    [SerializeField] private ScrollRect _scrollPagesView;
    [Space]
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private GameObject _imagePrefab;

    public UndoControllerComponent UndoControllerComponent => _undoControllerComponent;
    private List<Page> _pages = new List<Page>();
    private List<PagePreview> _pagesPreviews = new List<PagePreview>();
    private int _currentIndexPage;


    private void Awake()
    {
        if (MenuSceneController.Instance.CurrentBook != null)
        {
            Book editBook = MenuSceneController.Instance.CurrentBook;
            OpenEditBook(editBook);
        }
        else
        {
            AddPage();
        }

        _currentIndexPage = 0;
        SetCurrentPage(_currentIndexPage);
    }

    private void OpenEditBook(Book book)
    {
        _nameBook.text = book.NameBook;
        _coverImageBox.texture = book.CoverBook.texture;
        _coverImageBox.GetComponent<RawImageAspectPreserver>().SetAspect();

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

        float textureWidth = texture.width / 2;
        float textureHeight = texture.height / 2;

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

    public void SaveBook()
    {
        Book newBook = new Book();

    }

    public void ExportBook()
    {

    }

    public async void AddCoverBook()
    {
        string pathToImage = FileManager.SelectImageInBrowser();

        if (!string.IsNullOrWhiteSpace(pathToImage))
        {
            Texture2D cover = await FileManager.LoadTextureAsync(pathToImage, false);
            _coverImageBox.texture = cover;
            _coverImageBox.GetComponent<RawImageAspectPreserver>().SetAspect();
        }
        else
        {
            return;
        }
    }

    public void AddPage()
    {
        GameObject pagePreviewObj = Instantiate(_pageInScroolViewPrefab, _scrollPagesView.content);
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

    public void SavePage()
    {
        _screenshotHandler.TakeScreenshot(_pages[_currentIndexPage].gameObject);
    }

    public void SetImagePreview(Texture2D texture)
    {
        _pagesPreviews[_currentIndexPage].SetImage(texture);
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
