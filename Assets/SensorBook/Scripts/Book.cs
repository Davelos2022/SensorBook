using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameBookTMP;
    [SerializeField]
    private RawImage _coverBook;
    [SerializeField]
    private Button _bookBtn;
    [SerializeField]
    private Button _favoriteBookBTN;
    [Space]
    [Header("Admin Panel")]
    [SerializeField]
    private Button _deletedBTN;
    [SerializeField]
    private Button _exportBTN;
    [SerializeField]
    private Button _editBookBTN;


    private bool _favoriteBook;
    private string _pathToPDF;
    private DateTime _dateTime; public DateTime DataTimeBook => _dateTime;

    private void OnEnable()
    {
        _bookBtn.onClick.AddListener(Book_Click);
        _favoriteBookBTN.onClick.AddListener(FavoriteBook_CLick);
        _deletedBTN.onClick.AddListener(DeletedBook_Click);
        _exportBTN.onClick.AddListener(ExportBook_Click);
        _editBookBTN.onClick.AddListener(ExportBook_Click);
    }

    private void OnDisable()
    {
        _bookBtn.onClick.RemoveListener(Book_Click);
        _favoriteBookBTN.onClick.RemoveListener(FavoriteBook_CLick);
        _deletedBTN.onClick.RemoveListener(DeletedBook_Click);
        _exportBTN.onClick.RemoveListener(ExportBook_Click);
        _editBookBTN.onClick.RemoveListener(ExportBook_Click);
    }


    public void SetParameters(string NameBook, Texture2D CoverTexture, string PathToBook, DateTime DateTime)
    {
        _nameBookTMP.text = NameBook;
        _coverBook.texture = CoverTexture;
        _pathToPDF = PathToBook;
        _dateTime = DateTime;
    }

    private void Book_Click()
    {
        MenuSceneController.Instance.BookMode(_pathToPDF);
    }

    private void FavoriteBook_CLick()
    {
        _favoriteBook = !_favoriteBook;
    }

    private void DeletedBook_Click()
    {

    }

    private void ExportBook_Click()
    {

    }

    public void AdminPanel(bool active)
    {
        _deletedBTN.gameObject.SetActive(active);
        _editBookBTN.gameObject.SetActive(active);
        _exportBTN.gameObject.SetActive(active);
    }
}
