using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftPanelEditor : MonoBehaviour
{
    [SerializeField] private Button _backBTN;
    [SerializeField] private Button _undo;
    [SerializeField] private Button _rendo;
    [SerializeField] private Button _addText;
    [SerializeField] private Button _addImage;
    [SerializeField] private Button _clearPage;

    private void OnEnable()
    {
        _backBTN.onClick.AddListener(BackCLick);
        _undo.onClick.AddListener(UndoCLick);
        _rendo.onClick.AddListener(RendoClick);
        _addText.onClick.AddListener(AddTextClick);
        _addImage.onClick.AddListener(AddImageClick);
        _clearPage.onClick.AddListener(ClearPageClick);
    }

    private void OnDisable()
    {
        _backBTN.onClick.RemoveListener(BackCLick);
        _undo.onClick.RemoveListener(UndoCLick);
        _rendo.onClick.RemoveListener(RendoClick);
        _addText.onClick.RemoveListener(AddTextClick);
        _addImage.onClick.RemoveListener(AddImageClick);
        _clearPage.onClick.RemoveListener(ClearPageClick);
    }

    private void BackCLick()
    {
        MenuSceneController.Instance.ReturnLibary();
    }

    private void UndoCLick()
    {
        EditorBook.Instance.Undo();
    }

    private void RendoClick()
    {
        EditorBook.Instance.Rendo();
    }

    private void AddTextClick()
    {
        EditorBook.Instance.AddText();
    }

    private void AddImageClick()
    {
        EditorBook.Instance.AddImage();
    }

    private void ClearPageClick()
    {
        EditorBook.Instance.ClearPage();
    }
}
