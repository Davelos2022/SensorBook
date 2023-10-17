using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using VolumeBox.Toolbox;

public class UndoRedoSystem : Singleton<UndoRedoSystem>
{
    [SerializeField] private int _countUndo = 10;
    private Stack<UndoRedoAction> undoStack;
    private Stack<UndoRedoAction> redoStack;

    void Start()
    {
        undoStack = new Stack<UndoRedoAction>();
        redoStack = new Stack<UndoRedoAction>();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            UndoRedoAction action = undoStack.Pop();
            action.Undo();

            redoStack.Push(action);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            UndoRedoAction action = redoStack.Pop();
            action.Redo();

            undoStack.Push(action);
        }
    }

    public void AddAction(UndoRedoAction action)
    {
        if (undoStack.Count > _countUndo)
            undoStack.Pop();
 
        undoStack.Push(action);
        redoStack.Clear();
    }

    public void ClearHistory()
    {
        undoStack.Clear();
        redoStack.Clear();
    }

    public int GetCountUndo()
    {
        return undoStack.Count;
    }
}

