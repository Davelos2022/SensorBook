using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUndoRecorder : UndoRecorder
{
    [SerializeField] private TextMeshProUGUI inputText;

    private int _initialValue;

    protected override void ClearHandle()
    {
        inputText.text = "fgfgfgf";
    }

    protected override object RecordHandle()
    {
        return inputText.text;
    }


    protected override void RestoreStateHandle(object state)
    {
        inputText.text = (string)state;
    }
}
