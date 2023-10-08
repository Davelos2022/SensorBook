using UnityEngine;
using System.Collections.Generic;

public class ScreenshotHandler : MonoBehaviour
{
    [SerializeField] private Vector2 _sizeScreen;

    private static ScreenshotHandler Instance;

    private GameObject screenshot;
    private static int _currentIndexPage;

    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;
    private bool coverScreen;

    private void Awake()
    {
        Instance = this;
        myCamera = gameObject.GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, true);
            Rect rect = new Rect(0, 0, renderResult.width, renderResult.height);
            renderResult.ReadPixels(rect, 0, 0);
            renderResult.Apply();

            Sprite screenShoot = Sprite.Create(renderResult, new Rect(0, 0, renderResult.width, renderResult.height), new Vector2(renderResult.width / 2, renderResult.height / 2));
            //FileHandler.AddPage_InBook(screenShoot, _currentIndexPage);

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
            Destroy(screenshot);
        }

        if (coverScreen)
        {
            coverScreen = false;

            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, true);
            Rect rect = new Rect(0, 0, renderResult.width, renderResult.height);
            renderResult.ReadPixels(rect, 0, 0);
            renderResult.Apply();

            Sprite screenShoot = Sprite.Create(renderResult, new Rect(0, 0, renderResult.width, renderResult.height), new Vector2(renderResult.width / 2, renderResult.height / 2));
            //FileHandler.coverBook = screenShoot;
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
            Destroy(screenshot);
        }
    }

    private void TakeScreenshot(GameObject Screen)
    {
        screenshot = Instantiate(Screen, transform.parent);

        screenshot.GetComponent<RectTransform>().sizeDelta = _sizeScreen;
        screenshot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        myCamera.targetTexture = RenderTexture.GetTemporary((int)_sizeScreen.x, (int)_sizeScreen.y, 24);
        takeScreenshotOnNextFrame = true;
    }

    public static void TakeScreenshot_Static(GameObject Screen, int indexPage)
    {
        _currentIndexPage = indexPage;
        Instance.TakeScreenshot(Screen);
    }

    private void TakeScreenshotCover(GameObject Screen)
    {
        screenshot = Instantiate(Screen, transform.parent);

        screenshot.GetComponent<RectTransform>().sizeDelta = _sizeScreen;
        screenshot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        myCamera.targetTexture = RenderTexture.GetTemporary((int)_sizeScreen.x, (int)_sizeScreen.y, 24);
        coverScreen = true;
    }

    public static void TakeScreenshot_StaticCover(GameObject Screen)
    {
        Instance.TakeScreenshotCover(Screen);
    }
}
