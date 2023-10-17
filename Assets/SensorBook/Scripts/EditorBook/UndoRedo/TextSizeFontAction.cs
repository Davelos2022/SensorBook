using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSizeFontAction : UndoRedoAction
{
    private TextMeshProUGUI _text;
    private TMP_Dropdown tMP_Dropdown; 
    private float _oldSize;
    private float _newSize;


    public TextSizeFontAction(TextMeshProUGUI text, float oldSize, float newSize, TMP_Dropdown tMP)
    {
        _text = text;
        _oldSize = oldSize;
        _newSize = newSize;
        tMP_Dropdown = tMP;
    }

    public override void Undo()
    {
        _text.fontSize = _oldSize;
        tMP_Dropdown.captionText.text = _oldSize.ToString();
    }

    public override void Redo()
    {
        _text.fontSize = _newSize;
        tMP_Dropdown.captionText.text = _newSize.ToString();
    }
}
