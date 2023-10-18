using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextColorValueAction : UndoRedoAction
{
    private TextMeshProUGUI _text;
    private Color _oldColor;
    private Color _newColor;

    public TextColorValueAction(TextMeshProUGUI text, Color oldColor, Color newColor)
    {
        _text = text;
        _oldColor = oldColor;
        _newColor = newColor;
    }

    public override void Undo()
    {
        _text.color = _oldColor;
    }

    public override void Redo()
    {
        _text.color = _newColor;
    }
}
