using UnityEngine;
using Cysharp.Threading.Tasks;


public class ScreenshotHandler : MonoBehaviour
{
    private GameObject _screenshot;

    private Camera _myCamera;
    private bool _takeScreenshotOnNextFrame;

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

            EditorBook.Instance.SetImagePreview(renderResult);

            RenderTexture.ReleaseTemporary(renderTexture);
            _myCamera.targetTexture = null;
            Destroy(_screenshot);
        }
    }


    public  void TakeScreenshot(GameObject Screen)
    {
        RectTransform sizeScreenShot = Screen.GetComponent<RectTransform>();

        _screenshot = Instantiate(Screen, transform.parent);

        _screenshot.GetComponent<RectTransform>().sizeDelta = sizeScreenShot.sizeDelta;
        _screenshot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        _myCamera.targetTexture = RenderTexture.GetTemporary((int)sizeScreenShot.rect.width, (int)sizeScreenShot.rect.height, 24);
        _takeScreenshotOnNextFrame = true;
    }
}
