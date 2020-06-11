using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using NielsenPDFv2.Commands;
using NielsenPDFv2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NielsenPDFv2.Tools
{
    public static class PDFTools
    {
        private static PasswordDialogCommand passwordDialogCommand = new PasswordDialogCommand();
        public static int GetTotalPages(FileObject file)
        {
            PdfDocument doc = null;
            if (file.PasswordProtected)
            {
                var readerProps = new ReaderProperties();
                readerProps.SetPassword(Encoding.ASCII.GetBytes(file.Password));
                var pdfReader = new PdfReader(file.FilePath, readerProps);
                pdfReader.SetUnethicalReading(true);
                doc = new PdfDocument(pdfReader);
                pdfReader.Close();
            }
            else
            {
                var pdfReader = new PdfReader(file.FilePath);
                doc = new PdfDocument(pdfReader);
                pdfReader.Close();
            }
            int numPages = doc.GetNumberOfPages();
            doc.Close();
            return numPages;
        }

        public static void CheckPDFPassword(FileObject file)
        {
            PdfDocument doc = null;
            try
            {
                var pdfReader = new PdfReader(file.FilePath);
                doc = new PdfDocument(pdfReader);
            }
            catch(iText.Kernel.PdfException e)
            {
                if (e.Message.Contains("password"))
                {
                    //openpopup to enter password
                    var readerProps = new ReaderProperties();
                    passwordDialogCommand.Execute(null);
                    var password = passwordDialogCommand.Password;
                    file.Password = password;
                    file.PasswordProtected = true;
                    //call passwordinputcommand to get password popup.
                    readerProps.SetPassword(Encoding.ASCII.GetBytes(password));
                    var pdfReader = new PdfReader(file.FilePath, readerProps);
                    pdfReader.SetUnethicalReading(true);
                    doc = new PdfDocument(pdfReader);
                    pdfReader.Close();
                }
            }
            doc.Close();
        }
    }
}
