using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Paroxe.PdfRenderer;

public static class FileHandler
{
    private static string _bookPath = Application.streamingAssetsPath + "/Books/";

    public static string _pdfpath = null;
    public static List<Sprite> _booksPages = new List<Sprite>();


    public static string[] GetCountFiles()
    {
        var checkFormats = new[] { ".pdf" };

        var countFiles = Directory
            .GetFiles(_bookPath)
            .Where(file => checkFormats.Any(file.ToLower().EndsWith))
            .ToArray();

        return countFiles;
    }

    public static DateTime DateCreation(string path)
    {
        var info = new FileInfo(path);
        //var xx = info.GetFiles().Where(x => x.Extension.ToLower() == ".pdf").OrderByDescending(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();

        var dateFile = info.CreationTime;

        return dateFile;
    }

    public static List<Sprite> OpenPDF_file(bool cover = false)
    {
        PDFDocument pdfDocument = new PDFDocument(_pdfpath, "");
        List<Sprite> pdfPages = new List<Sprite>();

        int countPage = pdfDocument.GetPageCount();

        for (int x = 0; x < countPage; x++)
            pdfPages.Add(LoadPDF(_pdfpath, x));

        if (!cover)
            pdfPages.RemoveAt(0);

        return pdfPages;
    }

    public static void CheckDirectoory()
    {
        if (Directory.Exists(_bookPath))
            return;
        else
            Directory.CreateDirectory(_bookPath);
    }
    public static Sprite LoadPDF(string pdfPath, int pageNumber = 0)
    {
        PDFDocument pdfDocument = new PDFDocument(pdfPath, "");

        if (pdfDocument.IsValid)
        {
            PDFRenderer renderer = new PDFRenderer();
            Texture2D texture = renderer.RenderPageToTexture(pdfDocument.GetPage(pageNumber));

            texture.filterMode = FilterMode.Bilinear;
            texture.anisoLevel = 8;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width, texture.height));
        }
        else
        {
            return null;
        }
    }

    public static bool CheckBook(string nameBook)
    {
        string path = _bookPath + nameBook + ".pdf";

        if (File.Exists(path))
            return true;
        else
            return false;
    }

    public static bool CheckExportBook(string pathBook)
    {
        string path = pathBook + ".pdf";

        if (File.Exists(path))
            return true;
        else
            return false;

    }


    public static string OpenFileDialog(bool pdf = false)
    {
        FileDialogMsg ofn = new FileDialogMsg();
        ofn.structSize = Marshal.SizeOf(ofn);

        if (pdf)
            ofn.filter = "PDF Files\0*.pdf;\0\0";
        else
            ofn.filter = "Image Files\0*.jpg;*.png\0\0";

        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ofn.title = "Open image";
        ofn.defExt = "PNG";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DllTest.GetOpenFileName(ofn))
            return ofn.file;

        return "";
    }
    public static string SaveFileDialog(string nameBook)
    {
        SaveFileDlg sfd = new SaveFileDlg();
        sfd.structSize = Marshal.SizeOf(sfd);
        sfd.filter = "PDF(*.pdf)\0*.pdf";
        sfd.file = new string(new char[256]);
        sfd.maxFile = sfd.file.Length;
        sfd.fileTitle = new string(new char[64]);
        sfd.maxFileTitle = sfd.fileTitle.Length;
        sfd.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        sfd.title = "Save Book";
        sfd.defExt = "PDF";
        sfd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DllTest.GetSaveFileName(sfd))
            return sfd.file;

        return "";
    }
}


