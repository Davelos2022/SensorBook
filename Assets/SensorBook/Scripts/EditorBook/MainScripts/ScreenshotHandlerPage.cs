using UnityEngine;
using Cysharp.Threading.Tasks;

public class ScreenshotHandlerPage : MonoBehaviour
{
    private GameObject _screenshot;

    private Camera _myCamera;
    private bool _takeScreenshotOnNextFrame;

    private int _indexPage;

    private void Awake()
    {
        _myCamera = gameObject.GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        if (_takeScreenshotOnNextFrame)
        {
            _takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = _myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, true);
            Rect rect = new Rect(0, 0, renderResult.width, renderResult.height);
            renderResult.ReadPixels(rect, 0, 0);
            renderResult.Apply();

            EditorBook.Instance.SetImagePreviewPage(renderResult, _indexPage);

            RenderTexture.ReleaseTemporary(renderTexture);
            _myCamera.targetTexture = null;
            Destroy(_screenshot);
        }
    }


    public  void TakeScreenshot(GameObject pageObject, int indexPage)
    {
        _indexPage = indexPage;
        _screenshot = Instantiate(pageObject, transform.parent);

        Page page = _screenshot.GetComponent<Page>();
        page.DeActiveElemetnsForScreen();
        page.PageRectTransform.anchoredPosition = Vector2.zero;

        _myCamera.targetTexture = RenderTexture.GetTemporary((int)page.PageRectTransform.rect.width, (int)page.PageRectTransform.rect.height, 0);
        _takeScreenshotOnNextFrame = true;
    }
}
