using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObjectAction : UndoRedoAction
{
    private RectTransform _rectTransform;

    private float _oldWight;
    private float _newWight;

    private float _oldHeght;
    private float _newHeght;

    public ScaleObjectAction(RectTransform rectTransform, float oldWight, float oldHeght, float newWight, float newHeght)
    {
        _rectTransform = rectTransform;

        _oldWight = oldWight;
        _oldHeght = oldHeght;

        _newWight = newWight;
        _newHeght = newHeght;
    }

    public override void Redo()
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _newWight);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _newHeght);
    }

    public override void Undo()
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _oldWight);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _oldHeght);
    }
}
