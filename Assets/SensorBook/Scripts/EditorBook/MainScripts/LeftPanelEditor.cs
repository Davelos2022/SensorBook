using UnityEngine;
using UnityEngine.UI;
using VolumeBox.Toolbox.UIInformer;

public class LeftPanelEditor : MonoBehaviour
{
    [SerializeField] private Button _backBTN;
    [SerializeField] private Button _undo;
    [SerializeField] private Button _redo;
    [SerializeField] private Button _addText;
    [SerializeField] private Button _addImage;
    [SerializeField] private Button _clearPage;

    private void OnEnable()
    {
        _backBTN.onClick.AddListener(BackCLick);
        _undo.onClick.AddListener(UndoCLick);
        _redo.onClick.AddListener(RedoClick);
        _addText.onClick.AddListener(AddTextClick);
        _addImage.onClick.AddListener(AddImageClick);
        _clearPage.onClick.AddListener(ClearPageClick);
    }

    private void OnDisable()
    {
        _backBTN.onClick.RemoveListener(BackCLick);
        _undo.onClick.RemoveListener(UndoCLick);
        _redo.onClick.RemoveListener(RedoClick);
        _addText.onClick.RemoveListener(AddTextClick);
        _addImage.onClick.RemoveListener(AddImageClick);
        _clearPage.onClick.RemoveListener(ClearPageClick);
    }

    private void BackCLick()
    {
        if (EditorBook.Instance.UndoControllerComponent.GetCountStack())
            Info.Instance.ShowBox("Вы точно хотите выйти, внесенные изменения будут потеряны?", 
                ReturnInLibbary, null, null, "Да, выйти", "Отмена");
        else
            ReturnInLibbary();
    }

    private void UndoCLick()
    {
        EditorBook.Instance.Undo();
    }

    private void RedoClick()
    {
        EditorBook.Instance.Redo();
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
        Info.Instance.ShowBox("Вы действительно хотите очистить страницу?", 
            ClearPage, null, null, "Очистить страницу", "Отмена");
    }

    private void ClearPage()
    {
        EditorBook.Instance.ClearPage();
    }

    private void ReturnInLibbary()
    {
        EditorBook.Instance.UndoControllerComponent.ClearHistory();
        MenuSceneController.Instance.ReturnLibary();
    }
}
