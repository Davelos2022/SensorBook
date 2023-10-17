using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeTextValueAction : UndoRedoAction
{
    private TextMeshProUGUI _text;
    string _oldValue;
    string _newValue;

    public ChangeTextValueAction(TextMeshProUGUI text, string oldValue, string newValue)
    {
        _text = text;
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override void Undo()
    {
        _text.text = _oldValue;
    }


    public override void Redo()
    {
        _text.text = _newValue;
    }
}
