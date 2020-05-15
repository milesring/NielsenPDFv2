using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class AddContractCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            Contract c = new Contract() { ContractName = "New Contract", ContractNumber = "", LastUsedDirectory = "" };
            App.Database.SaveContractAsync(c).Wait();
            viewModel.Refresh = true;
            viewModel.SelectedIndex = viewModel.Contracts.Count;
            viewModel.LoadContractsCommand.Execute(viewModel);
        }

    }
}
