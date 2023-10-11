using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VolumeBox.Toolbox.UIInformer;

public class PagePreview : MonoBehaviour
{
    [SerializeField] private Button _pageBTN;
    [SerializeField] private Button _deletedBTN;
    [Space]
    [SerializeField] private RawImage _imageBox;
    [SerializeField] private RawImageAspectPreserver _rawImageAspect;
    [Space]
    [SerializeField] private GameObject _selectedPanel;
    [SerializeField] private TextMeshProUGUI _textPage;

    private int _indexPage;

    private void OnEnable()
    {
        _pageBTN.onClick.AddListener(ClickPage);
        _deletedBTN.onClick.AddListener(MessageInfoForDeletedPage);
    }

    private void OnDisable()
    {
        _pageBTN.onClick.RemoveListener(ClickPage);
        _deletedBTN.onClick.RemoveListener(MessageInfoForDeletedPage);
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
    }

    public void ClearPreviewPage()
    {
        SetImage(null);
    }

    private void MessageInfoForDeletedPage()
    {
        Info.Instance.ShowBox($"Вы действительно хотите удалить страницу?", 
            DeletedPage, DeletedPage, null, "Удалить", "Отмена");
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
}
