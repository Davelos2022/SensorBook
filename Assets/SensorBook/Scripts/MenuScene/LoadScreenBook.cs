using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VolumeBox.Toolbox;

public class LoadScreenBook : Singleton<LoadScreenBook>
{
    [SerializeField]
    private GameObject _loadScreen;
    [SerializeField]
    private TextMeshProUGUI _loadText;
    [SerializeField]
    private TextMeshProUGUI _loadPageText;

    public void LoadScreen(bool active, string textLoad = null)
    {
        _loadScreen.SetActive(active);
        _loadText.text = textLoad;
    }

    public void SetTextLoadPage(string loadPageText)
    {
        _loadPageText.text = loadPageText;
    }
}
