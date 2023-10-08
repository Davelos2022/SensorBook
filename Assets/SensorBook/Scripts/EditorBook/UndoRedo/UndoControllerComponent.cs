
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoControllerComponent : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Z))
            Undo();

        if (Input.GetKeyUp(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Y))
            Redo();
    }


    [ContextMenu("Undo")]
    public void Undo()
    {
        UndoController.Undo();
    }

    [ContextMenu("Redo")]
    public void Redo()
    {
        UndoController.Redo();
    }

    public void ClearHistory()
    {
        UndoController.ClearHistory();
    }
}
