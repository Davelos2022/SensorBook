using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VolumeBox.Toolbox.UIInformer;
using UnityEngine.EventSystems;

public enum TypePreview
{
    Page,
    Cover
}

public class PagePreview : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private TypePreview _typePreview;
    [SerializeField] private Button _pageBTN;
    [SerializeField] private Button _deletedBTN;
    [Space]
    [SerializeField] private RawImage _imageBox;
    [SerializeField] private RawImageAspectPreserver _rawImageAspect;
    [Space]
    [SerializeField] private GameObject _selectedPanel;
    [SerializeField] private TextMeshProUGUI _textPage;

    private int _indexPage;
    public RawImage ImageBox => _imageBox;
    public int IndexPage => _indexPage;

    private RectTransform _pagePreviewTransform;
    private CanvasGroup _canvasGroup;
    private GridLayoutGroup _gridLayoutGroup;

    void Start()
    {
        if (_typePreview == TypePreview.Page)
        {
            _pagePreviewTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _gridLayoutGroup = GetComponentInParent<GridLayoutGroup>();
        }
    }

    private void OnEnable()
    {
        if (_typePreview == TypePreview.Page)
        {
            _pageBTN.onClick.AddListener(ClickPage);
            _deletedBTN.onClick.AddListener(MessageInfoForDeletedPage);
        }
    }

    private void OnDisable()
    {
        if (_typePreview == TypePreview.Page)
        {
            _pageBTN.onClick.RemoveListener(ClickPage);
            _deletedBTN.onClick.RemoveListener(MessageInfoForDeletedPage);
        }
    }

    public void SetImage(Texture2D texture)
    {
        _imageBox.texture = texture;
        _rawImageAspect.SetAspect();
    }

    public void SetNumberPage(int index)
    {
        _indexPage = index;
        _textPage.text = $"{_indexPage + 1}";

        transform.SetSiblingIndex(index);
        CheckingFirstPage();
    }

    private void CheckingFirstPage()
    {
        if (_indexPage == 0)
        {
            _deletedBTN.gameObject.SetActive(false);
        }
        else if (_indexPage > 0 && !_deletedBTN.gameObject.activeSelf)
        {
            _deletedBTN.gameObject.SetActive(true);
        }
    }

    private void DeletedPage()
    {
        EditorBook.Instance.DeletedPage(_indexPage);
    }

    private void ClickPage()
    {
        EditorBook.Instance.SetCurrentPage(_indexPage);
    }

    public void SelectedPage(bool active)
    {
        _selectedPanel.SetActive(active);
    }

    private void MessageInfoForDeletedPage()
    {
        Info.Instance.ShowBox($"Вы действительно хотите удалить страницу?",
            DeletedPage, null, null, "Удалить", "Отмена");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_typePreview == TypePreview.Page)
        {
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;

            _gridLayoutGroup.enabled = false;
            _pagePreviewTransform.transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_typePreview == TypePreview.Page)
        {
            _pagePreviewTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_typePreview == TypePreview.Page)
        {
            _gridLayoutGroup.enabled = true;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            if (!eventData.pointerEnter.GetComponent<PagePreview>())
                _pagePreviewTransform.transform.SetSiblingIndex(_indexPage);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_typePreview == TypePreview.Page)
        {
            if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<PagePreview>())
            {
                int afterIndex = eventData.pointerDrag.GetComponent<PagePreview>().IndexPage;
                EditorBook.Instance.SwapPages(_indexPage, afterIndex);
            }
        }
    }
}
