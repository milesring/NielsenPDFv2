using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NielsenPDFv2.Tools
{
    public static class PDFTools
    {

        public static int GetTotalPages(string filePath)
        {
            var pdfReader = new PdfReader(filePath);
            var doc = new PdfDocument(pdfReader);
            int numPages = doc.GetNumberOfPages();
            doc.Close();
            pdfReader.Close();
            return numPages;
        }
    }
}
