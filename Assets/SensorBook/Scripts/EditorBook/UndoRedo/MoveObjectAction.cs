using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectAction : UndoRedoAction
{
    private RectTransform movedObject;
    private Vector3 originalPosition;
    private Vector3 newPosition;

    public MoveObjectAction(RectTransform obj, Vector3 originalPos, Vector3 newPos)
    {
        movedObject = obj;
        originalPosition = originalPos;
        newPosition = newPos;
    }

    public override void Undo()
    {
        movedObject.anchoredPosition = originalPosition;
    }

    public override void Redo()
    {
        movedObject.anchoredPosition = newPosition;
    }
}
