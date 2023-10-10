using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour
{
    [Header("All_btn")]
    [SerializeField] private Button _allBook;
    [SerializeField] private GameObject _selectedAllBook;
    [Header("Favorite_btn")]
    [SerializeField] private Button _favoriteBook;
    [SerializeField] private GameObject _selectedFavoriteBook;

    private void OnEnable()
    {
        _allBook.onClick.AddListener(ClickAll);
        _favoriteBook.onClick.AddListener(ClickFavorite);

        ClickAll();
    }

    private void OnDisable()
    {
        _allBook.onClick.RemoveListener(ClickAll);
        _favoriteBook.onClick.RemoveListener(ClickFavorite);
    }

    private void ClickAll()
    {
        _selectedFavoriteBook.SetActive(false);
        _selectedAllBook.SetActive(true);

        MenuSceneController.Instance.ShowAllBook();
    }

    private void ClickFavorite()
    {
        _selectedFavoriteBook.SetActive(true);
        _selectedAllBook.SetActive(false);

        MenuSceneController.Instance.ShowFavoriteBook();
    }
}
