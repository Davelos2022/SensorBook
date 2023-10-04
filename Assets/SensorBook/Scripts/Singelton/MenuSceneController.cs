using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Paroxe.PdfRenderer;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSceneController : Singleton<MenuSceneController>
{
    [SerializeField]
    private GameObject _bookPrefabs;

    private string _bookPath = Application.streamingAssetsPath + "/Books/";
    private string _pdfpath = null;


    private List<Book> _collectionBook = new List<Book>();

    public void AddPDF_in_Libarry()
    {

    }
    public void LoadBook_in_Libary()
    {

    }

    public void CreateBook(GameObject prefab, Transform parrent)
    {

    }

    public void EditorBook(string pathBook = null)
    {
        if (!string.IsNullOrWhiteSpace(pathBook))
            _pdfpath = pathBook;

        StartCoroutine(LoadSceneAsync("SensorBook_EditorBook"));
    }

    public void BookMode(string pathBook)
    {
        _pdfpath = pathBook;
        StartCoroutine(LoadSceneAsync("SensorBook_BookMode"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
