using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class MergePDFCommand : ICommand
    {
        #region Locals
        private PasswordDialogCommand passwordDialogCommand;
        #endregion

        #region Properties
        public PasswordDialogCommand PasswordDialogCommand
        {
            get
            {
                if(passwordDialogCommand == null)
                {
                    passwordDialogCommand = new PasswordDialogCommand();
                }
                return passwordDialogCommand;
            }
            set
            {
                passwordDialogCommand = value;
            }
        }
        #endregion


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

            string checkPath = Path.Combine(viewModel.WorkingDirectory, viewModel.OutputName)+".pdf";
            if(viewModel.Files.Any(file=>file.FilePath == checkPath))
            {
                viewModel.BuildStatus = "Error: Name matches file already in list";
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

            bool overwriteFile = false;
            //Check if desired output file already exists
            if(File.Exists(Path.Combine(viewModel.WorkingDirectory, viewModel.OutputName + ".pdf")))
            {
                //ask user to overwrite file
                var result = MessageBox.Show("File exists, overwrite?", "Overwrite existing file", MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.Cancel)
                {
                    return;
                }
                else
                {
                    overwriteFile = true;
                }
            }

            //Copy of real file list to manipulate
            List<FileObject> tempFileList = CopyFileObjectList(viewModel.Files);
            

            //create a temp directory to work in
            string outputPath = Path.Combine(viewModel.WorkingDirectory, "PDFTemp", viewModel.OutputName + ".pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            //copy all files to work in a temp directory.
            CopyTempFiles(viewModel, tempFileList, outputPath);

            PdfDocument pdf = null;
            PdfMerger merger = null;
            PdfDocument doc = null;

            try
            {
                //check user option for encryption
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

                foreach (var file in tempFileList)
                {
                    if (!File.Exists(file.FilePath))
                    {
                        viewModel.BuildStatus = $"Failed: Temporary file({file.FileName}) no longer exists.";
                        pdf.Close();
                        merger.Close();
                        File.Delete(outputPath);
                        return;
                    }
                    try
                    {
                        //check if file is protected
                        if (file.PasswordProtected)
                        {
                            var readerProps = new ReaderProperties();
                            readerProps.SetPassword(Encoding.ASCII.GetBytes(file.Password));
                            var pdfReader = new PdfReader(file.FilePath, readerProps);
                            pdfReader.SetUnethicalReading(true);
                            doc = new PdfDocument(pdfReader);
                        }
                        else
                        {
                            var pdfReader = new PdfReader(file.FilePath);
                            doc = new PdfDocument(pdfReader);
                        }
                    }
                    catch (iText.Kernel.PdfException e)
                    {

                    }
                    //merge document
                    var numPages = doc.GetNumberOfPages();
                    merger.Merge(doc, 1, numPages);

                    doc.Close();

                    //update build progress
                    viewModel.BuildProgress += numPages;
                }
                pdf.Close();

                //move to orig location
                File.Move(outputPath, Path.Combine(viewModel.WorkingDirectory, viewModel.OutputName + ".pdf"), overwriteFile);

                //delete temp files
                foreach (var file in tempFileList)
                {
                    File.Delete(file.FilePath);
                }

                //delete temp dir
                Directory.Delete(Path.GetDirectoryName(outputPath));

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

        private static void CopyTempFiles(MainViewModel viewModel, List<FileObject> tempFileList, string outputPath)
        {
            var parentPath = Path.GetDirectoryName(outputPath);
            for (int i = 0; i < tempFileList.Count; i++)
            {
                //get original file object
                var origFile = viewModel.Files[i];

                //get new temporary path to copy file to
                var tempPath = Path.Combine(parentPath, Path.GetFileName(origFile.FilePath));

                //check if temp file exists, we need the most updated copy
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                //copy file
                File.Copy(origFile.FilePath, tempPath);

                //update temporary file in list
                tempFileList[i].FilePath = tempPath;
            }
        }

        private static List<FileObject> CopyFileObjectList(ObservableCollection<FileObject> original)
        {
            var returnList = new List<FileObject>();
            foreach (var file in original)
            {
                returnList.Add(new FileObject(file));
            }
            return returnList;
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
