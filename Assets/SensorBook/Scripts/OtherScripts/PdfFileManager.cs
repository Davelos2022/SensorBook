using Cysharp.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Paroxe.PdfRenderer;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class PdfFileManager : FileManager
{
    public static string _bookPath = Application.streamingAssetsPath + "/Books/";
    private static string _pathRecovery = _bookPath + "Recovery/";

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

    public static List<Sprite> OpenPdfFile(string pathToPDF)
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
            Debug.LogWarning($"Failed to save book: {e.Message}");
            return null;
        }
    }

    public async static UniTask SaveBookInPDF(string path, List<Texture2D> pagesTexture, RectTransform sizePage)
    {
        await RecoveryImagePage(pagesTexture);
        await CreateDocumentPDF((int)sizePage.rect.width
            , (int)sizePage.rect.height, path);
        await DeleteRecoveryImage();
    }

    private static async UniTask RecoveryImagePage(List<Texture2D> texturePages)
    {
        if (!Directory.Exists(_pathRecovery))
            Directory.CreateDirectory(_pathRecovery);

        for (int x = 0; x < texturePages.Count; x++)
            await SaveTexture(texturePages[x], _pathRecovery + $"{x}.png");
    }

    private static async UniTask CreateDocumentPDF(int width, int height, string pathToSave)
    {
        Rectangle sizeDocPage = new Rectangle(width, height);
        Document document = new Document(sizeDocPage, 0,0,0,0);
        PdfWriter.GetInstance(document, new FileStream($"{pathToSave}", FileMode.Create));

        await AddPageInDocument(document);
    }

    private static async UniTask AddPageInDocument(Document document)
    {
        Paragraph paragraph = CreateCenterAlignedParagraph();
        var info = new DirectoryInfo(_pathRecovery);
        var files = info.GetFiles().Where(x => x.Extension.ToLower() == ".png").
            OrderBy(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();

        document.SetMargins(0, 0, 0, 0);
        document.Open();

        foreach (var page in files)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                var image = LoadAndScaleImage(page, document);
                paragraph.Add(image);
            });
        }

        document.Add(paragraph);
        document.Close();
    }
    private static Paragraph CreateCenterAlignedParagraph()
    {
        Paragraph paragraph = new Paragraph();
        paragraph.Alignment = Element.ALIGN_CENTER;
        paragraph.SpacingAfter = 0;
        paragraph.SpacingBefore = 0;
        return paragraph;
    }

    private static Image LoadAndScaleImage(string imagePath, Document document)
    {
        Image image = Image.GetInstance(imagePath);

        float imageWidth = image.Width;
        float documentWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
        float scaleRatio = documentWidth / imageWidth;
        image.ScalePercent(scaleRatio * 100);

        return image;
    }

    private async static UniTask DeleteRecoveryImage()
    {
        if (Directory.Exists(_pathRecovery))
        {
            DirectoryInfo di = new DirectoryInfo(_pathRecovery);

            foreach (FileInfo file in di.GetFiles())
                await UniTask.RunOnThreadPool(() => file.Delete());

            Directory.Delete(_pathRecovery);
        }
    }

    public static void DeleteFile(string pathToFile)
    {
        if (File.Exists(pathToFile))
            File.Delete(pathToFile);
        else
            return;
    }
}
