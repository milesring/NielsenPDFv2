using NielsenPDFv2.Commands;
using NielsenPDFv2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        #region Locals
        private ObservableCollection<Contract> contracts;
        private LoadContractsCommand loadContractsCommand;
        private Contract selectedContract;
        private Contract originalContract;
        private AddContractCommand addContractCommand;
        private RemoveContractCommand removeContractCommand;
        private SaveContractCommand saveContractCommand;
        private int selectedIndex;
        private bool? refresh;
        private bool contractEdited;
        private MainViewModel mainViewModel;
        #endregion


        public SettingsViewModel()
        {
            LoadContractsCommand.Execute(this);
        }

        #region Properties
        public ObservableCollection<Contract> Contracts
        {
            get { 
                if(contracts == null)
                {
                    contracts = new ObservableCollection<Contract>();
                }
                return contracts;
            }
            set
            {
                contracts = value;
                OnPropertyChanged(nameof(Contracts));
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
                    OriginalContract = (Contract)selectedContract.Shallowcopy();
                }
                else
                {
                    OriginalContract = null;
                }
                OnPropertyChanged(nameof(SelectedContract));
            }
        }

        public Contract OriginalContract
        {
            get { return originalContract; }
            set
            {
                originalContract = value;
                OnPropertyChanged(nameof(OriginalContract));
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public bool? Refresh
        {
            get { return refresh; }
            set
            {
                refresh = value;
                OnPropertyChanged(nameof(Refresh));
            }
        }

        public bool ContractEdited
        {
            get { return contractEdited; }
            set
            {
                contractEdited = value;
                OnPropertyChanged(nameof(ContractEdited));
            }
        }

        public MainViewModel MainViewModel {
            get
            {
                return mainViewModel;
            }
            set
            {
                mainViewModel = value;
                PDFPreviews = mainViewModel.PDFPreviews;
                Overwrite = mainViewModel.OverwriteFile;

            }
        }

        public bool PDFPreviews
        {
            get { 
                if(MainViewModel == null)
                {
                    return false;
                }
                return MainViewModel.PDFPreviews; 
            }
            set
            {
                MainViewModel.PDFPreviews = value;
                OnPropertyChanged(nameof(PDFPreviews));
            }
        }

        public bool Overwrite
        {
            get
            {
                if(MainViewModel == null)
                {
                    return false;
                }
                return MainViewModel.OverwriteFile;
            }
            set
            {
                MainViewModel.OverwriteFile = value;
                OnPropertyChanged(nameof(Overwrite));
            }
        }
        #endregion

        #region Commands
        public LoadContractsCommand LoadContractsCommand
        {
            get
            {
                if(loadContractsCommand == null)
                {
                    loadContractsCommand = new LoadContractsCommand();  
                }
                return loadContractsCommand;
            }
            set
            {
                loadContractsCommand = value;
                OnPropertyChanged(nameof(LoadContractsCommand));
            }
        }

        public SaveContractCommand SaveContractCommand
        {
            get
            {
                if(saveContractCommand == null)
                {
                    saveContractCommand = new SaveContractCommand();
                }
                return saveContractCommand;
            }
            set
            {
                saveContractCommand = value;
                OnPropertyChanged(nameof(SaveContractCommand));
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
                OnPropertyChanged(nameof(AddContractCommand));
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
            set
            {
                removeContractCommand = value;
                OnPropertyChanged(nameof(RemoveContractCommand));
            }
        }

        #endregion  
    }
}
