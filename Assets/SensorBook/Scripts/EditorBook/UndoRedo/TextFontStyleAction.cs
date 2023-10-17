using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFontStyleAction : UndoRedoAction
{
    private TextMeshProUGUI _textComponent;
    private GameObject _selectedButton;
    private bool _selectedStatus;
    private FontStyles _originalStyle;
    private FontStyles _newStyle;

    public TextFontStyleAction(TextMeshProUGUI text, FontStyles original, FontStyles newStyle, GameObject selected)
    {
        _textComponent = text;
        _originalStyle = original;
        this._newStyle = newStyle;
        _selectedButton = selected;
    }

    public override void Undo()
    {
        _selectedButton.SetActive(!_selectedButton.activeSelf);
        _textComponent.fontStyle = _originalStyle;
    }

    public override void Redo()
    {
        _selectedButton.SetActive(_selectedButton.activeSelf);
        _textComponent.fontStyle = _newStyle;
    }
}
