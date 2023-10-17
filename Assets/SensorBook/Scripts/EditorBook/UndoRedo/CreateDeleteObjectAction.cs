using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDeleteObjectAction : UndoRedoAction
{
    private GameObject _createdObject;
    private bool _deleted;
    public CreateDeleteObjectAction(GameObject obj, bool deleted = false)
    {
        _createdObject = obj;
        _deleted = deleted;
    }

    public override void Undo()
    {
        _createdObject.SetActive(_deleted);
    }

    public override void Redo()
    {
        _createdObject.SetActive(!_deleted);
    }
}

