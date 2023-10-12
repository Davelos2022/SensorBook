using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RightPanelEditor : MonoBehaviour
{
    [SerializeField] private Button _exortBookBTN;
    [SerializeField] private Button _saveBookBTN;
    [SerializeField] private Button _addCoverBook;
    [SerializeField] private Button _addPage;


    private void OnEnable()
    {
        _exortBookBTN.onClick.AddListener(ExportBookClick);
        _saveBookBTN.onClick.AddListener(SaveBookClick);
        _addCoverBook.onClick.AddListener(AddCoverBookClick);
        _addPage.onClick.AddListener(AddPageClick);
    }

    private void OnDisable()
    {
        _exortBookBTN.onClick.RemoveListener(ExportBookClick);
        _saveBookBTN.onClick.RemoveListener(SaveBookClick);
        _addCoverBook.onClick.RemoveListener(AddCoverBookClick);
        _addPage.onClick.RemoveListener(AddPageClick);
    }

    private void ExportBookClick()
    {
        EditorBook.Instance.SaveOrExportBook(true);
    }

    private void SaveBookClick()
    {
        EditorBook.Instance.SaveOrExportBook();
    }

    private void AddCoverBookClick()
    {
        EditorBook.Instance.AddCoverBook();
    }

    private void AddPageClick()
    {
        EditorBook.Instance.TakeScreenShotCurrentPage();
        EditorBook.Instance.AddPage();
    }
}
