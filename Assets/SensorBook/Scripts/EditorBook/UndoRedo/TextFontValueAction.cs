using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFontValueAction : UndoRedoAction
{
    private TextMeshProUGUI _curetnText;
    private TMP_Dropdown _tMP_Dropdown;
    private TMP_FontAsset _oldFont;
    private TMP_FontAsset _newFont; 

    public TextFontValueAction(TextMeshProUGUI curetnText, TMP_FontAsset oldFont, TMP_FontAsset newFont, TMP_Dropdown tMP)
    {
        _curetnText = curetnText;
        _oldFont = oldFont;
        _newFont = newFont;
        _tMP_Dropdown = tMP;
    }   

    public override void Undo()
    {
        _curetnText.font = _oldFont;
        _tMP_Dropdown.captionText.text = _oldFont.ToString().Substring(0, 7) + "...";
    }

    public override void Redo()
    {
        _curetnText.font = _newFont;
        _tMP_Dropdown.captionText.text = _newFont.ToString().Substring(0, 7) + "...";
    }

}
