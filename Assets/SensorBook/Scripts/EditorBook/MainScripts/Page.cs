using UnityEngine;
using TMPro;

public class Page : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numberPage;
    [SerializeField] private RectTransform _pageRectTransform;
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

    public void DeActiveNumberPage()
    {
        _numberPage.gameObject.SetActive(false);
    }
}
