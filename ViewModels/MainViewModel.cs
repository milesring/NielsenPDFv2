using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using iText.IO.Util;
using NielsenPDFv2.Commands;
using NielsenPDFv2.Models;
using NielsenPDFv2.Tools;
using SQLitePCL;

namespace NielsenPDFv2.ViewModels
{
    public class MainViewModel : ViewModel
    {

        #region Locals
        private string title;
        private ObservableCollection<Contract> contracts;
        private Contract selectedContract;
        private int selectedIndex;
        private string workingDirectory = "No Working Directory";
        private ObservableCollection<FileObject> files;
        private List<FileObject> selectedFiles = new List<FileObject>();
        private FileObject highlightedFile;
        private Utility utility = new Utility();
        private string outputName;
        private DateTime selectedDate = DateTime.Today;
        private MergePDFCommand mergePDFCommand;
        private AddFileCommand addFileCommand;
        private RemoveFileCommand removeFileCommand;
        private BrowseFileCommand browseFileCommand;
        private OpenSettingsCommand openSettingsCommand;
        private PasswordDialogCommand passwordDialogCommand;
        private string buildStatus;
        private int buildProgress;
        private bool isBuilding = false;
        private bool overwriteFile;
        private bool encrypt;
        private string pdfPass;
        private int totalPages;
        private bool? refresh;
        private bool pdfPreviews;
        #endregion

        public MainViewModel()
        {
            Title = "Contracts";
            LoadSettings();
            LoadItems();
        }

        #region Public Methods
        public void AddFile(string path)
        {
            var file = new FileObject { FileName = utility.TrimFileName(path), FilePath = path, FileNum = Files.Count + 1};
            PDFTools.CheckPDFPassword(file);
            file.NumPages = PDFTools.GetTotalPages(file);
            TotalPages += file.NumPages;
            Files.Add(file);
        }
        public void RemoveFile(FileObject file)
        {
            TotalPages -= file.NumPages;
            _ = Files.Remove(file);
        }

        public void RefreshFileNums()
        {
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].FileNum = i+1;
            }
        }

        public void RemoveContractAsync(Contract c)
        {
           DeleteContract(c);
        }
        #endregion

        #region Private Methods
        private void LoadSettings()
        {
            var val = Application.Current.Properties[nameof(OverwriteFile)];
            if (val != null)
            {
                OverwriteFile = bool.Parse(val.ToString());
            }
            val = Application.Current.Properties[nameof(PDFPreviews)];
            if(val != null)
            {
                PDFPreviews = bool.Parse(val.ToString());
            }
            
        }
        private async void LoadItems()
        {
            var initialIndex = SelectedIndex;
            List<Contract> items;
            if (selectedIndex == 0)
            {
                items = await App.Database.GetContractsAsync();
            }
            else
            {
                var task = App.Database.GetContractsAsync();
                items = task.Result;
            }
            Contracts = new ObservableCollection<Contract>(items);
            SelectedIndex = initialIndex;
        }

        private async void DeleteContract(Contract c)
        {
            await App.Database.DeleteContractAsync(c);
            LoadItems();
        }

        private void CheckForStringShortcuts()
        {
            if (OutputName.Contains("{D}"))
            {
                OutputName = OutputName.Replace("{D}", SelectedDate.ToString("M-d-yy"));
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

        #region Events
        private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ResetBuildStatus();
        }
        #endregion

        #region Properties
        public ObservableCollection<Contract> Contracts
        {
            get 
            { 
                if(contracts == null)
                {
                    contracts = new ObservableCollection<Contract>();
                }
                return contracts; 
            }
            set
            {
                contracts = value;
                ResetBuildStatus();
                OnPropertyChanged(nameof(Contracts));
            }
        }

        public ObservableCollection<FileObject> Files
        {
            get 
            { 
                if(files == null)
                {
                    files = new ObservableCollection<FileObject>();
                    files.CollectionChanged += Files_CollectionChanged;
                }
                return files; 
            }
            set
            {
                files = value;
                files.CollectionChanged += Files_CollectionChanged;
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

        public FileObject HighlightedFile
        {
            get { return highlightedFile; }
            set
            {
                highlightedFile = value;
                OnPropertyChanged(nameof(HighlightedFile));
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

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { 
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public string OutputName
        {
            get { return outputName; }
            set
            {
                outputName = value;
                CheckForStringShortcuts();
                ResetBuildStatus();
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

        public int BuildProgress
        {
            get { return buildProgress; }
            set
            {
                buildProgress = value;
                OnPropertyChanged(nameof(BuildProgress));
            }
        }

        public bool IsBuilding
        {
            get { return isBuilding; }
            set
            {
                isBuilding = value;
                OnPropertyChanged(nameof(IsBuilding));
            }
        }

        public bool OverwriteFile
        {
            get { return overwriteFile; }
            set
            {
                overwriteFile = value;
                Application.Current.Properties[nameof(OverwriteFile)] = value;
                OnPropertyChanged(nameof(OverwriteFile));
            }
        }

        public bool Encrypt
        {
            get { return encrypt; }
            set
            {
                encrypt = value;
                OnPropertyChanged(nameof(Encrypt));
            }
        }

        public string PDFPass
        {
            get { return pdfPass; }
            set
            {
                pdfPass = value;
                OnPropertyChanged(nameof(PDFPass));
            }
        }

        public int TotalPages
        {
            get { return totalPages; }
            set
            {
                totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public bool? Refresh
        {
            get { return refresh; }
            set
            {
                refresh = value;
                if (refresh == true)
                {
                    LoadItems();
                }
                OnPropertyChanged(nameof(Refresh));
            }
        }

        public bool PDFPreviews
        {
            get { return pdfPreviews; }
            set
            {
                pdfPreviews = value;
                Application.Current.Properties[nameof(PDFPreviews)] = value;
                OnPropertyChanged(nameof(PDFPreviews));
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


        public OpenSettingsCommand OpenSettingsCommand
        {
            get
            {
                if(openSettingsCommand == null)
                {
                    openSettingsCommand = new OpenSettingsCommand();
                }
                return openSettingsCommand;
            }
            set
            {
                openSettingsCommand = value;
                OnPropertyChanged(nameof(OpenSettingsCommand));
            }
        }

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
                OnPropertyChanged(nameof(PasswordDialogCommand));
            }
        }
        #endregion


    }
}
