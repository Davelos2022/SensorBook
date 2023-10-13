using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VolumeBox.Toolbox.UIInformer;

public enum TypePreview
{
    Page, 
    Cover
}

public class PagePreview : MonoBehaviour
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

        CheckingForFirstPage();
    }

    private void CheckingForFirstPage()
    {
        if (_indexPage == 0)
            _deletedBTN.gameObject.SetActive(false);
        else if (_indexPage > 0 && !_deletedBTN.gameObject.activeSelf)
            _deletedBTN.gameObject.SetActive(true);
    }

    public void ClearPreviewPage()
    {
        SetImage(null);
    }

    private void DeletedPage()
    {
        EditorBook.Instance.DeletedPage(_indexPage);
    }

    private void ClickPage()
    {
        EditorBook.Instance.TakeScreenShotCurrentPage();
        EditorBook.Instance.SetCurrentPage(_indexPage);
    }

    public void SelectedPage(bool active)
    {
        _selectedPanel.SetActive(active);
    }

    private void MessageInfoForDeletedPage()
    {
        Info.Instance.ShowBox($"Вы действительно хотите удалить страницу?",
            DeletedPage, DeletedPage, null, "Удалить", "Отмена");
    }
}
