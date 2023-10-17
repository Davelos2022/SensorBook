using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObjectAction : UndoRedoAction
{
    private RectTransform _object;
    private Quaternion _originalRotation;
    private Quaternion _newRotation;

    public RotationObjectAction(RectTransform obj, Quaternion oldRotation, Quaternion newRotation)
    {
        _object = obj;
        _originalRotation = oldRotation;
        _newRotation = newRotation;
    }


    public override void Undo()
    {
        _object.rotation = _originalRotation;  
    }

    public override void Redo()
    {
        _object.rotation = _newRotation;
    }
}
