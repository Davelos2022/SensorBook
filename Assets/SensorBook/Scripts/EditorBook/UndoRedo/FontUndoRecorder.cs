using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontUndoRecorder : UndoRecorder
{
    [SerializeField] private TMP_Dropdown fontDropdown;

    private int _initialValue;

    protected override void ClearHandle()
    {
        fontDropdown.value = _initialValue;
    }

    protected override object RecordHandle()
    {
        return fontDropdown.value;
    }

    protected override void RestoreStateHandle(object state)
    {
        fontDropdown.value = (int)state;
    }
}
