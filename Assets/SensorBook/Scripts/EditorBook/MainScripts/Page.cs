using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Page : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numberPage;
    [SerializeField] private RectTransform _pageRectTransform;
    [SerializeField] private Image _imageBox;
    public RectTransform PageRectTransform => _pageRectTransform;

    public void SetNumberPage(int indexPage)
    {
        int numberPage = indexPage;
        _numberPage.text = $"{numberPage + 1}";

        transform.SetSiblingIndex(indexPage);
    }

    public void ClearPage()
    {
        for (int x = 1; x < transform.childCount; x++)
        {
            Destroy(transform.GetChild(x).gameObject);
        }
    }

    public void DeActiveElemetnsForScreen()
    {
        _imageBox.sprite = null;
        _numberPage.gameObject.SetActive(false);
    }
}
