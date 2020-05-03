using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    class MergePDFCommand : ICommand
    {
        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            MainViewModel viewModel = parameter as MainViewModel;

            if (viewModel.IsBuilding)
                return false;

            if (viewModel.Files.Count < 1)
                return false;

            if (string.IsNullOrWhiteSpace(viewModel.WorkingDirectory))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(viewModel.OutputName))
            {
                return false;
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            var viewModel = parameter as MainViewModel;
            viewModel.IsBuilding = true;
            await Task.Run(()=>MergePDFs(viewModel));
            viewModel.IsBuilding = false;

        }

        private async Task MergePDFs(MainViewModel viewModel)
        {
            viewModel.BuildStatus = "Merging PDFs...";
            viewModel.BuildProgress = 0;
            string outputPath = viewModel.WorkingDirectory + "\\" + viewModel.OutputName + ".pdf";
            PdfDocument pdf = null;
            PdfMerger merger = null;
            PdfDocument doc = null;
            try
            {
                if (File.Exists(outputPath))
                {
                    if (!viewModel.OverwriteFile)
                    {
                        viewModel.BuildStatus = "Failed: Output PDF already exists.";
                        return;
                    }
                    else
                    {
                        outputPath = Path.Combine(viewModel.WorkingDirectory, "PDFTemp", viewModel.OutputName + ".pdf");
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    }
                }
                if (viewModel.Encrypt)
                {
                    pdf = new PdfDocument(new PdfWriter(outputPath, new WriterProperties()
                        .SetStandardEncryption(
                        Encoding.ASCII.GetBytes(viewModel.PDFPass),
                        null,
                        EncryptionConstants.ALLOW_PRINTING,
                        EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA)
                        ));
                }
                else
                {
                    pdf = new PdfDocument(new PdfWriter(outputPath));
                }
                merger = new PdfMerger(pdf);
                foreach (FileObject file in viewModel.Files)
                {
                    if (!File.Exists(file.FilePath))
                    {
                        viewModel.BuildStatus = $"Failed: {file.FileName} no longer exists and has been removed from the list.";
                        viewModel.Files.Remove(file);
                        pdf.Close();
                        merger.Close();
                        File.Delete(outputPath);
                        return;
                    }
                    try
                    {
                        var pdfReader = new PdfReader(file.FilePath);
                        doc = new PdfDocument(pdfReader);
                    }
                    catch (iText.Kernel.PdfException e)
                    {
                        if (e.Message.Contains("password"))
                        {
                            //openpopup to enter password
                            var readerProps = new ReaderProperties();
                            readerProps.SetPassword(Encoding.ASCII.GetBytes("test"));
                            var pdfReader = new PdfReader(file.FilePath, readerProps);
                            pdfReader.SetUnethicalReading(true);
                            doc = new PdfDocument(pdfReader);
                        }
                    }
                    
                    merger.Merge(doc, 1, doc.GetNumberOfPages());
                    doc.Close();
                    viewModel.BuildProgress++;
                }
                pdf.Close();
                if (viewModel.OverwriteFile && File.Exists(Path.Combine(viewModel.WorkingDirectory, viewModel.OutputName + ".pdf")))
                {
                    File.Move(outputPath, Path.Combine(viewModel.WorkingDirectory, viewModel.OutputName + ".pdf"), true);
                    Directory.Delete(Path.GetDirectoryName(outputPath), true);
                }
                viewModel.BuildStatus = "Success: PDF Successfully Created";
            }
            //CLEAN UP THIS EXCEPTION GARBAGE EVENTUALLY
            catch (iText.IO.IOException e)
            {
                viewModel.BuildStatus = "Failed: Corrupt PDF";
            }
            //catch(iText.Signatures.)
            catch (iText.Kernel.PdfException e)
            {
                viewModel.BuildStatus = "Failed: Corrupt PDF";
            }
            catch (FileNotFoundException e)
            {
                viewModel.BuildStatus = "Failed: A PDF in the list no longer exists";
            }
            catch (IOException e)
            {
                viewModel.BuildStatus = "Failed: Attempting to modify a file in use";
            }
        }
        #endregion

        private bool IsPasswordProtected(string pdf)
        {
            try
            {
                var pdfReader = new PdfReader(pdf);

            }catch (Exception e)
            {

            }
            return false;

        }
    }
}
