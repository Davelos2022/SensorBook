using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UndoController
{
    private static Stack<UndoInfo> _undoStack = new Stack<UndoInfo>();
    private static Stack<UndoInfo> _redoStack = new Stack<UndoInfo>();

    public static event Action ClearEvent;
    private static bool _cleared;

    public static void AddRecord(UndoRecorder recorder, object state)
    {
        var info = new UndoInfo { state = state, recorder = recorder };

        _cleared = false;
        _redoStack.Clear();

        _undoStack.Push(info);
    }

    public static void ClearHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    public static void Undo()
    {
        if(_cleared || _undoStack.Count <= 0)
        {
            ClearEvent?.Invoke();
            return;
        }

        if(_redoStack.Count <= 0)
        {
            _redoStack.Push(_undoStack.Pop());
        }

        if(_undoStack.Count <= 0)
        {
            _cleared = true;
            ClearEvent?.Invoke();
            return;
        }

        var info = _undoStack.Pop();
        _redoStack.Push(info);

        info.recorder.RestoreState(info.state);
    }

    public static void Redo()
    {
        if (_redoStack.Count <= 0) return;

        _cleared = false;

        var info = _redoStack.Pop();

        _undoStack.Push(info);

        info.recorder.RestoreState(info.state);
    }
}

[Serializable]
public class UndoInfo
{
    public UndoRecorder recorder;
    public object state;
}
