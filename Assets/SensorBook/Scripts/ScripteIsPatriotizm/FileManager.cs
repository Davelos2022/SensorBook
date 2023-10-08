using System;
using System.IO;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SFB;
using UnityEngine;
using VolumeBox.Toolbox;
using VolumeBox.Toolbox.UIInformer;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using UnityEngine.Networking;
using Paroxe.PdfRenderer;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;

public abstract class FileManager : Singleton<FileManager>
{
    public static string DataPath => Application.streamingAssetsPath;

    public static Texture2D LowerTexture(Texture2D tex, int maxSize)
    {
        int destWidth;
        int destHeight;

        if (tex.width > tex.height)
        {
            float asp = (float)tex.height / (float)maxSize;
            destWidth = (int)((float)tex.width / asp);
            destHeight = (int)((float)tex.height / asp);
        }
        else if (tex.width == tex.height)
        {
            float asp = (float)tex.width / (float)maxSize;
            destWidth = (int)((float)tex.width / asp);
            destHeight = (int)((float)tex.height / asp);
        }
        else
        {
            destWidth = maxSize;
            destHeight = maxSize;
        }

        var newTex = new RenderTexture(destWidth, destHeight, 0);
        Graphics.Blit(tex, newTex);
        RenderTexture.active = newTex;
        var finalTex = new Texture2D(destWidth, destHeight);
        finalTex.ReadPixels(new Rect(0, 0, newTex.width, newTex.height), 0, 0);
        finalTex.Apply();

        RenderTexture.active = null;
        Destroy(newTex);
        Resources.UnloadUnusedAssets();

        return finalTex;
    }

    public static string GetUID()
    {
        var rnd = new System.Random((int)DateTime.Now.ToFileTimeUtc());
        return rnd.Next().ToString("x");
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

    public static Sprite CreateSprite(Texture2D texture, SpriteMeshType meshType = SpriteMeshType.Tight)
    {
        try
        {
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100, 0, meshType);
            return sprite;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public static Texture2D BytesToTexture(byte[] bytes)
    {
        Texture2D tex = new Texture2D(0, 0);

        tex.LoadImage(bytes);

        return tex;
    }

    //Method with package, possible error in other modules. Use only on MAP!!!!
    public static async UniTask<Texture2D> LoadTextureAsyncPackage(string path, bool useSavePath = true, CancellationToken token = default)
    {
        if (useSavePath)
        {
            path = DataPath + "/" + path;
        }

        try
        {
            var imageData = await File.ReadAllBytesAsync(path);

            var ext = Path.GetExtension(path);

            AsyncImageLoader.FreeImage.Format imgFormat = AsyncImageLoader.FreeImage.Format.FIF_JPEG;

            switch (ext)
            {
                case ".jpg":
                    imgFormat = AsyncImageLoader.FreeImage.Format.FIF_JPEG;
                    break;
                case ".jpeg":
                    imgFormat = AsyncImageLoader.FreeImage.Format.FIF_JPEG;
                    break;
                case ".png":
                    imgFormat = AsyncImageLoader.FreeImage.Format.FIF_PNG;
                    break;
                default:
                    imgFormat = AsyncImageLoader.FreeImage.Format.FIF_JPEG;
                    break;
            }

            var loaderSettings = new AsyncImageLoader.LoaderSettings() { format = imgFormat, generateMipmap = true, autoMipmapCount = true };
            return await AsyncImageLoader.CreateFromImageAsync(imageData, loaderSettings);
        }
        catch (Exception e)
        {
            Debug.LogWarning("ERROR: " + e.Message);
            return null;
        }
    }

    public static async UniTask<Texture2D> LoadTextureAsync(string path, bool useSavePath = true, CancellationToken token = default)
    {
        if (useSavePath)
        {
            path = DataPath + "/" + path;
        }

        path = "file://" + path;

        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path, false);

        try
        {
            await uwr.SendWebRequest().WithCancellation(token);

            if (token.IsCancellationRequested)
            {
                uwr.Dispose();
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message + " " + path);
            uwr.Dispose();
            return null;
        }

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            uwr.Dispose();
            return null;
        }
        else
        {
            var tex = DownloadHandlerTexture.GetContent(uwr);
            Texture2D texture = new Texture2D(tex.width, tex.height, tex.format, true);
            texture.LoadImage(uwr.downloadHandler.data);
            uwr.Dispose();
            return texture;
        }
    }

    public static async UniTask<bool> CopyFileAsync(string sourcePath, string destinationPath, bool createCopyIfAlreadyExists = false, CancellationToken token = default)
    {
        if (sourcePath == destinationPath)
        {
            return true;
        }

        if (File.Exists(destinationPath))
        {
            if (createCopyIfAlreadyExists)
            {
                var dir = Path.GetDirectoryName(destinationPath);
                var newFileName = Path.GetFileName(destinationPath) + "_copy";
                var ext = Path.GetExtension(destinationPath);
                destinationPath = Path.Combine(dir, newFileName + ext);
            }
            else
            {
                try
                {
                    File.Delete(destinationPath);
                }
                catch
                {
                    Info.Instance.ShowHint("Произошла ошибка при чтении файла");
                    return false;
                }
            }
        }

        var directoryPath = Path.GetDirectoryName(destinationPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        //SetAllPermissions(Path.GetDirectoryName(sourcePath));
        //SetAllPermissions(Path.GetDirectoryName(destinationPath));

        try
        {
            Stream source = File.Open(sourcePath, FileMode.OpenOrCreate);
            Stream destination = File.Open(destinationPath, FileMode.OpenOrCreate);
            await source.CopyToAsync(destination, token);
            source.Close();
            await source.DisposeAsync();
            destination.Close();
            await destination.DisposeAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public bool CopyFileTo(string filePath, string toPath, bool createCopyIfExists = false)
    {
        if (File.Exists(toPath))
        {
            if (!createCopyIfExists) return true;

            var dir = Path.GetDirectoryName(toPath);
            var newFileName = Path.GetFileName(toPath) + "_copy";
            var ext = Path.GetExtension(toPath);
            toPath = Path.Combine(dir, newFileName + ext);
        }

        //SetAllPermissions(Path.GetDirectoryName(filePath));
        //SetAllPermissions(Path.GetDirectoryName(toPath));

        try
        {
            File.Copy(filePath, toPath);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    public abstract UniTask Save<T>(T data, string path, bool useSavePath = true, CancellationToken token = default) where T : class;

    public abstract UniTask<T> Load<T>(string path, bool useSavePath = true, CancellationToken token = default) where T : class;

    [CanBeNull]
    public static string SelectAudioInBrowser()
    {
        ExtensionFilter[] extensions = {
            new("Sound Files", "mp3", "wav"),
        };

        var audioPaths = StandaloneFileBrowser.OpenFilePanel("Выберите звук", "", extensions, false);

        if (audioPaths.Length > 0)
        {
            return audioPaths[0];
        }
        else
        {
            return null;
        }
    }

    [CanBeNull]
    public static string SelectFolderInBrowser()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Выберите папку", "", false);

        if (paths.Length > 0)
        {
            return paths[0];
        }
        else
        {
            return null;
        }
    }

    [CanBeNull]
    public static string SelectImageInBrowser()
    {
        ExtensionFilter[] extensions = {
            new("Image Files", "jpg", "jpeg", "png"),
        };

        var imagePaths = StandaloneFileBrowser.OpenFilePanel("Выберите картинку", "", extensions, false);

        if (imagePaths.Length > 0)
        {
            return imagePaths[0];
        }
        else
        {
            return null;
        }
    }

    [CanBeNull]
    public static string SelectMediaInBrowser()
    {
        ExtensionFilter[] extensions = {
            new("Media Files", "jpg", "jpeg", "png", "avi", "mp4"),
        };

        var mediaPaths = StandaloneFileBrowser.OpenFilePanel("Выберите медиа файл", "", extensions, false);

        if (mediaPaths.Length > 0)
        {
            return mediaPaths[0];
        }
        else
        {
            return null;
        }
    }

    //Created by Denis

    public static string _bookPath = Application.streamingAssetsPath + "/Books/";

    public static string[] GetCountBookFiles()
    {
        var checkFormats = new[] { ".pdf" };

        var countFiles = Directory
            .GetFiles(_bookPath)
            .Where(file => checkFormats.Any(file.ToLower().EndsWith))
            .ToArray();

        return countFiles;
    }

    public static DateTime GetDateCreation(string path)
    {
        var info = new FileInfo(path);
        var dateFile = info.CreationTime;

        return dateFile;
    }

    public static string SelectPdfFileBrowser()
    {
        ExtensionFilter[] extensions = {
            new("PDF Files", "pdf"),
        };

        var mediaPaths = StandaloneFileBrowser.OpenFilePanel("Выберите pdf файл", "", extensions, false);

        if (mediaPaths.Length > 0)
        {
            return mediaPaths[0];
        }
        else
        {
            return null;
        }
    }

    public static string SavePdfFileBrowser(string nameBook)
    {
        ExtensionFilter[] extensions = {
            new("PDF Files", "pdf"),
        };

        var mediaPaths = StandaloneFileBrowser.SaveFilePanel("Сохранить книгу", "", nameBook, extensions);

        if (mediaPaths.Length > 0)
        {
            return mediaPaths;
        }
        else
        {
            return null;
        }
    }

    public static List<Sprite> OpenPDF_file(string pathToPDF)
    {
        try
        {
            PDFDocument pdfDocument = new PDFDocument(pathToPDF, "");
            List<Sprite> pdfPages = new List<Sprite>();

            Texture2D tex;
            int countPage;
            countPage = pdfDocument.GetPageCount();

            for (int x = 1; x < countPage; x++)
            {
                tex = pdfDocument.Renderer.RenderPageToTexture(pdfDocument.GetPage(x));
                tex.filterMode = FilterMode.Bilinear;
                tex.anisoLevel = 8;

                Sprite sprite = CreateSprite(tex);

                pdfPages.Add(sprite);
            }


            return pdfPages;
        }
        catch (Exception e)
        {
            //Notifier.Instance.Notify(NotifyType.Error, "Произошла ошибка при открытие файла");
            Debug.LogWarning($"Failed to open book: {e.Message}");
            return null;
        }
    }

    public static void DeletedFile(string pathToFile)
    {
        if (File.Exists(pathToFile))
            File.Delete(pathToFile);
        else
            return;
    }

    public async static UniTask<bool> ExportBookIn_PDF(string path, Book book, bool open = false)
    {
        try
        {
            path = path + $"{book.PathToPDF}.pdf";
            string pathRecovery = book.PathToPDF + "_Recovery";

            //Create recovery Image
            for (int x = 0; x < book.PagesBook.Count; x++)
            {
                var tex = book.PagesBook[x].texture;
                var texBytes = tex.EncodeToPNG();
                await SaveTexture(tex, pathRecovery);
            }

            //Save PDF
            RectTransform sizeImage = book.CoverBook.GetComponent<RectTransform>();

            Rectangle sizePage = new Rectangle(sizeImage.rect.width + 100, sizeImage.rect.height + 100);
            var doc = new Document(sizePage);
            PdfWriter.GetInstance(doc, new FileStream($"{path}", FileMode.Create));

            //PDF proccesing
            doc.Open();

            var info = new DirectoryInfo(pathRecovery);
            var files = info.GetFiles().Where(x => x.Extension.ToLower() == ".png").OrderBy(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();

            foreach (var pathInDirectory in files)
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    var image = iTextSharp.text.Image.GetInstance(pathInDirectory);
                    image.Alignment = Element.ALIGN_CENTER;
                    doc.Add(image);
                });
            }

            doc.Close();

            //Clear recovery Iamege

            if (Directory.Exists(pathRecovery))
            {
                DirectoryInfo di = new DirectoryInfo(pathRecovery);

                foreach (FileInfo file in di.GetFiles())
                {
                    await UniTask.RunOnThreadPool(() => file.Delete());
                }


                Directory.Delete(book.PathToPDF + "_Recovery");
            }

            //Copy file and Open 

            if (open)
                Application.OpenURL(path);

            return true;
        }
        catch (Exception e)
        {
            //Notifier.Instance.Notify(NotifyType.Error, "Произошла ошибка при экспорте файла");
            Debug.LogWarning($"Failed to save book: {e.Message}");
            return false;
        }
    }

    //End

    public static async UniTask SaveTexture(Texture2D tex, string savePath, CancellationToken token = default)
    {
        if (tex == null) return;

        byte[] bytes = tex.EncodeToPNG();

        if (bytes == null) return;

        await File.WriteAllBytesAsync(savePath, bytes, token);
    }

    public static bool SelectAudioInBrowser([CanBeNull] out string path)
    {
        path = SelectAudioInBrowser();

        return !(path == null || string.IsNullOrEmpty(path));
    }

    public static bool SelectImageInBrowser([CanBeNull] out string path)
    {
        path = SelectImageInBrowser();
        return !(path == null || string.IsNullOrEmpty(path));
    }

    public static bool CompareFiles(string pathA, string pathB)
    {
        if (pathA.Equals(pathB))
        {
            return true;
        }

        if ((!File.Exists(pathA) || !File.Exists(pathB)))
        {
            return false;
        }

        using (FileStream fs1 = new FileStream(pathA, FileMode.Open), fs2 = new FileStream(pathB, FileMode.Open))
        {
            int c1 = 0;
            int c2 = 0;

            do
            {
                c1 = fs1.ReadByte();
                c2 = fs2.ReadByte();
            }
            while (c1 == c2 && c1 != -1 && c2 != -1);

            return c1 == c2;
        }
    }
}
