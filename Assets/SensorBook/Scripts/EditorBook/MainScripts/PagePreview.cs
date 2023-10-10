using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PagePreview : MonoBehaviour
{
    [SerializeField] private Button _pageBTN;
    [SerializeField] private Button _deletedBTN;
    [Space]
    [SerializeField] private Image _imageBox;
    [SerializeField] private GameObject _selectedPanel;
    [SerializeField] private TextMeshProUGUI _textPage;

    private int _indexPage;

    private void OnEnable()
    {
        _pageBTN.onClick.AddListener(ClickPage);
        _deletedBTN.onClick.AddListener(DeletedPage);
    }

    private void OnDisable()
    {
        _pageBTN.onClick.RemoveListener(ClickPage);
        _deletedBTN.onClick.RemoveListener(DeletedPage);
    }

    public void SetImage(Sprite sprite)
    {
        _imageBox.sprite = sprite;
        _imageBox.preserveAspect = true;
    }

    public void SetNumberPage(int index)
    {
        _indexPage = index;
        _textPage.text = $"{_indexPage + 1}";

        transform.SetSiblingIndex(index);
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
        EditorBook.Instance.SetCurrentPage(_indexPage);
    }
}
