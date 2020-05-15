using NielsenPDFv2.ViewModels;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class RemoveContractCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            if (viewModel.Contracts.Count < 1)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            var lastIndex = viewModel.SelectedIndex;
            App.Database.DeleteContractAsync(viewModel.SelectedContract).Wait();
            viewModel.Refresh = true;
            viewModel.SelectedIndex = lastIndex - 1;
            viewModel.LoadContractsCommand.Execute(viewModel);

        }
    }
}
