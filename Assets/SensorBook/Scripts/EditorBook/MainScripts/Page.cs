using UnityEngine;
using TMPro;

public class Page : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _numberPage;

    private RectTransform _pageRectTransform;
    public RectTransform PageRectTransform => _pageRectTransform;

    private void Start()
    {
        _pageRectTransform = GetComponent<RectTransform>();
    }

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
}
