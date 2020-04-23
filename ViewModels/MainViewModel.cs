using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using NielsenPDFv2.Commands;
using NielsenPDFv2.Models;
using NielsenPDFv2.Tools;

namespace NielsenPDFv2.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {

        #region Locals
        private string title;
        private ObservableCollection<Contract> contracts = new ObservableCollection<Contract>();
        private Contract selectedContract;
        private string workingDirectory = "No Working Directory";
        private ObservableCollection<FileObject> files = new ObservableCollection<FileObject>();
        private List<FileObject> selectedFiles = new List<FileObject>();
        private Utility utility = new Utility();
        private string outputName;
        private DateTime selectedDate = DateTime.Today;
        private MergePDFCommand mergePDFCommand;
        private AddFileCommand addFileCommand;
        private RemoveFileCommand removeFileCommand;
        private BrowseFileCommand browseFileCommand;
        private AddContractCommand addContractCommand;
        private RemoveContractCommand removeContractCommand;
        private SaveContractCommand saveContractCommand;
        private string buildStatus;
        #endregion

        public MainViewModel()
        {
            Title = "Contracts";
            LoadItems();
        }

        #region Public Methods
        public void AddFile(string path)
        {
            Files.Add(new FileObject { FileName = utility.TrimFileName(path), FilePath = path, FileNum = Files.Count + 1 });
        }
        public void RemoveFile(FileObject file)
        {
            _ = Files.Remove(file);
        }

        public void RefreshFileNums()
        {
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].FileNum = i+1;
            }
        }

        public void RemoveContract(Contract c)
        {
            DeleteContract(c);
        }

        public void AddContract()
        {
            AddContractAsync();
        }

        public void SaveContract()
        {
            SaveContractAsync();
        }

        public void MergePDFs()
        {
            BuildStatus = "Merging PDFs...";
            string outputPath = WorkingDirectory + "\\" + OutputName + ".pdf";
            PdfDocument pdf = null;
            PdfMerger merger = null;
            PdfDocument doc = null;
            try
            {
                pdf = new PdfDocument(new PdfWriter(outputPath));
                merger = new PdfMerger(pdf);
                foreach (FileObject file in Files)
                {
                    if (!File.Exists(file.FilePath))
                    {
                        BuildStatus = $"Failed: {file.FileName} no longer exists and has been removed from the list";
                        Files.Remove(file);
                        pdf.Close();
                        merger.Close();
                        File.Delete(outputPath);
                        return;
                    }

                    doc = new PdfDocument(new PdfReader(file.FilePath));
                    merger.Merge(doc, 1, doc.GetNumberOfPages());
                        doc.Close();
                }
                pdf.Close();
                BuildStatus = "Success: PDF Successfully Created";
            }
            catch(iText.IO.IOException e)
            {
                BuildStatus = "Failed: Corrupt PDF";
            }
            catch(iText.Kernel.PdfException e)
            {
                BuildStatus = "Failed: Corrupt PDF";
            }
            catch (FileNotFoundException e)
            {
                BuildStatus = "Failed: A PDF in the list no longer exists";
            }
            catch (IOException e)
            {
                BuildStatus = "Failed: Attempting to modify a file in use";
            }
            
        }
        #endregion

        #region Private Methods
        private async void LoadItems()
        {
            var items = await App.Database.GetContractsAsync();
            Contracts = new ObservableCollection<Contract>(items);
        }

        private async void DeleteContract(Contract c)
        {
            await App.Database.DeleteContractAsync(c);
            LoadItems();
        }

        private async void AddContractAsync()
        {
            Contract c = new Contract() { ContractName = "New Contract", ContractNumber = "000000", LastUsedDirectory = "" };
            await App.Database.SaveContractAsync(c);
            LoadItems();
        }

        private async void SaveContractAsync()
        {
            await App.Database.SaveContractAsync(SelectedContract);
            LoadItems();
        }

        private void CheckForStringShortcuts()
        {
            if (OutputName.Contains("{D}"))
            {
                OutputName = OutputName.Replace("{D}", SelectedDate.ToString("M-dd-yy"));
            }

            if (OutputName.Contains("{C}"))
            {
                OutputName = OutputName.Replace("{C}", SelectedContract.ContractNumber.ToString());
            }

            if (OutputName.Contains("{N}"))
            {
                OutputName = OutputName.Replace("{N}", SelectedContract.ContractName);
            }
        }

        private void ResetBuildStatus()
        {
            BuildStatus = string.Empty;
        }

        private void ResetFilesAndOutput()
        {
            Files.Clear();
            OutputName = string.Empty;
        }
        #endregion

        #region Properties
        public ObservableCollection<Contract> Contracts
        {
            get { return contracts; }
            set
            {
                contracts = value;
                ResetBuildStatus();
                OnPropertyChanged(nameof(Contracts));
            }
        }

        public ObservableCollection<FileObject> Files
        {
            get { return files; }
            set
            {
                files = value;
                ResetBuildStatus();
                OnPropertyChanged(nameof(Files));
            }
        }

        public List<FileObject> SelectedFiles
        {
            get { return selectedFiles; }
            set
            {
                selectedFiles = value;
                OnPropertyChanged(nameof(SelectedFiles));
            }
        }
        public string Title {
            get
            { return title; } 
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            } 
        }
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                if(value == SelectedContract.LastUsedDirectory)
                {
                    workingDirectory = value;
                }
                else
                {
                    workingDirectory = utility.TrimPath(value);
                }
                

                if (SelectedContract != null && 
                    SelectedContract.LastUsedDirectory != workingDirectory)
                {
                    SelectedContract.LastUsedDirectory = workingDirectory;
                    App.Database.SaveContractAsync(SelectedContract);
                }
                ResetBuildStatus();
                OnPropertyChanged(nameof(WorkingDirectory));
            }
        }

        public Contract SelectedContract
        {
            get { return selectedContract; }
            set
            {
                selectedContract = value;
                if (selectedContract != null)
                {
                    WorkingDirectory = selectedContract.LastUsedDirectory;
                }
                ResetBuildStatus();
                ResetFilesAndOutput();
                OnPropertyChanged(nameof(SelectedContract));
            }
        } 

        public string OutputName
        {
            get { return outputName; }
            set
            {
                outputName = value;
                CheckForStringShortcuts();
                OnPropertyChanged(nameof(OutputName));
            }
        }

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public string BuildStatus
        {
            get { return buildStatus; }
            set
            {
                buildStatus = value;
                OnPropertyChanged(nameof(BuildStatus));
            }
        }
        #endregion

        #region Commands
        public MergePDFCommand MergePDFCommand { 
            get
            { 
                if(mergePDFCommand == null)
                {
                    mergePDFCommand = new MergePDFCommand();
                }
                return mergePDFCommand;
            }
            set 
            {
                mergePDFCommand = value;
            }
        }

        public AddFileCommand AddFileCommand
        {
            get
            {
                if(addFileCommand == null)
                {
                    addFileCommand = new AddFileCommand();
                }
                return addFileCommand;
            }
            set
            {
                addFileCommand = value;
            }
        }

        public RemoveFileCommand RemoveFileCommand
        {
            get
            {
                if(removeFileCommand == null)
                {
                    removeFileCommand = new RemoveFileCommand();
                }
                return removeFileCommand;
            }
            set
            {
                removeFileCommand = value;
            }
        }

        public BrowseFileCommand BrowseFileCommand
        {
            get
            {
                if(browseFileCommand == null)
                {
                    browseFileCommand = new BrowseFileCommand();
                }
                return browseFileCommand;
            }
            set
            {
                browseFileCommand = value;
            }
        }

        public AddContractCommand AddContractCommand
        {
            get
            {
                if(addContractCommand == null)
                {
                    addContractCommand = new AddContractCommand();
                }
                return addContractCommand;
            }
            set
            {
                addContractCommand = value;
            }
        }

        public RemoveContractCommand RemoveContractCommand
        {
            get
            {
                if(removeContractCommand == null)
                {
                    removeContractCommand = new RemoveContractCommand();
                }
                return removeContractCommand;
            }
            set { removeContractCommand = value; }
        }

        public SaveContractCommand SaveContractCommand
        {
            get
            {
                if (saveContractCommand == null)
                {
                    saveContractCommand = new SaveContractCommand();
                }
                return saveContractCommand;
            }
            set
            {
                saveContractCommand = value;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
