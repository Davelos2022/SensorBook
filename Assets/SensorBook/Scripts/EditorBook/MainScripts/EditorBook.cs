using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class EditorBook : Singleton<EditorBook>
{
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

    private List<GameObject> _pages = new List<GameObject>();
    private List<Page> _pagesInScroll = new List<Page>();
    private int _currentIndexPage;

    private void OnEnable()
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
            CreateImage(book.PagesBook[x].texture, x);
            _pagesInScroll[x].SetImage(book.PagesBook[x]);
        }
    }

    public void Undo()
    {

    }

    public void Rendo()
    {

    }

    public async void AddImage()
    {
        string pathToImage = FileManager.SelectImageInBrowser();

        if (pathToImage.Length > 0)
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
        rawImage.SetNativeSize();
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

        if (countObjectInPage > 0)
        {
            for (int x = 0; x < countObjectInPage; x++)
            {
                Destroy(_pages[_currentIndexPage].transform.GetChild(x).gameObject);
            }
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

        if (pathToImage.Length > 0)
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
        GameObject page = Instantiate(_pageInScroolViewPrefab, _scrollPagesView.content);
        Page pageInPanel = page.GetComponent<Page>();

        _pagesInScroll.Add(pageInPanel);
        pageInPanel.SetIndexPage(_pagesInScroll.Count - 1);

        GameObject mainPage = Instantiate(_pagePrefab, _pageParent);
        _pages.Add(mainPage);

        _pages[_pages.Count - 1].SetActive(false);
    }

    public void DeletedPage(int indexPage)
    {
        Destroy(_pages[indexPage]);
        Destroy(_pagesInScroll[indexPage].gameObject);

        _pages.RemoveAt(indexPage);
        _pagesInScroll.RemoveAt(indexPage);

        RefeshPages();
    }

    public void SetCurrentPage(int indexPage)
    {
        _pages[_currentIndexPage].SetActive(false);
        _pages[indexPage].SetActive(true);

        _currentIndexPage = indexPage;
    }

    public void SwapPages(int beforeIndex, int afterIndex)
    {
        Page pageInPanel = _pagesInScroll[beforeIndex];
        _pagesInScroll[beforeIndex] = _pagesInScroll[afterIndex];
        _pagesInScroll[afterIndex] = pageInPanel;

        RefeshPages();
    }

    private void RefeshPages()
    {
        for (int x = 0; x < _pagesInScroll.Count; x++)
        {
            _pagesInScroll[x].SetIndexPage(x);
        }
    }
}
