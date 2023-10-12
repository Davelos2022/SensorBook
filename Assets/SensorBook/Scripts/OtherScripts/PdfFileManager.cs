using Cysharp.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Paroxe.PdfRenderer;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class PdfFileManager : FileManager
{
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

    public static List<Sprite> OpenPDFfile(string pathToPDF)
    {
        try
        {
            PDFDocument pdfDocument = new PDFDocument(pathToPDF, "");
            List<Sprite> pdfPages = new List<Sprite>();

            PDFRenderer.RenderSettings m_RenderSettings = new PDFRenderer.RenderSettings();
            m_RenderSettings.optimizeTextForLCDDisplay = true;

            Texture2D tex;
            int countPage = pdfDocument.GetPageCount();

            for (int x = 1; x < countPage; x++)
            {
                PDFPage page = pdfDocument.GetPage(x);
                int pageWidth = pdfDocument.GetPageWidth(x) * 2;
                int pageHeight = pdfDocument.GetPageHeight(x) * 2;

                tex = pdfDocument.Renderer.RenderPageToTexture
                    (page, pageWidth, pageHeight, null, m_RenderSettings);

                tex.filterMode = FilterMode.Bilinear;
                tex.anisoLevel = 10;
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

    public async static UniTask<bool> SaveBookInPDF(string path, List<Texture2D> pagesTexture, Texture2D coverTex, RectTransform sizePage, bool export = false)
    {
        try
        {

            //Path recovery
            string pathRecovery = _bookPath + "Recovery/";

            if (!Directory.Exists(pathRecovery))
                Directory.CreateDirectory(pathRecovery);

            //Create recovery Image
            pagesTexture.Insert(0, coverTex);

            for (int x = 0; x < pagesTexture.Count; x++)
            {
                var tex = pagesTexture[x];
                var texBytes = tex.EncodeToPNG();
                await SaveTexture(tex, pathRecovery + $"{x}.png");
            }

            //Save PDF
            Rectangle sizeDocPage = new Rectangle(sizePage.rect.width, sizePage.rect.height);
            var doc = new Document(sizeDocPage);
            PdfWriter.GetInstance(doc, new FileStream($"{path}", FileMode.Create));

            //PDF proccesing
            doc.Open();
            var info = new DirectoryInfo(pathRecovery);
            var files = info.GetFiles().Where(x => x.Extension.ToLower() == ".png").OrderBy(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();

            foreach (var page in files)
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    var image = iTextSharp.text.Image.GetInstance(page);
                    image.Alignment = Element.ALIGN_CENTER -60;

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

                Directory.Delete(pathRecovery);
            }

            //Message info
            if (export)
            {
                //Notifier.Instance.Notify(NotifyType.Success, "Книга экспортирована");
                Debug.Log("Книга экспортирована");
            }
            else
            {
                //Notifier.Instance.Notify(NotifyType.Success, "Книга сохранена");
                Debug.Log("Книга сохранена");
            }

            return true;
        }
        catch (Exception e)
        {
            //Notifier.Instance.Notify(NotifyType.Error, "Произошла ошибка при экспорте файла");
            Debug.LogWarning($"Failed to save book: {e.Message}");
            return false;
        }
    }

    public static void DeletedFile(string pathToFile)
    {
        if (File.Exists(pathToFile))
            File.Delete(pathToFile);
        else
            return;
    }


}
