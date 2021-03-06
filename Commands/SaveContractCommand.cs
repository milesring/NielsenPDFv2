﻿using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class SaveContractCommand : ICommand
    {
        public SaveContractCommand()
        {

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            if(viewModel.SelectedContract == null)
            {
                return false;
            }
            if(viewModel.SelectedContract.WeakCompare(viewModel.OriginalContract))
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            App.Database.SaveContractAsync(viewModel.SelectedContract).Wait();
            viewModel.Refresh = true;
            viewModel.LoadContractsCommand.Execute(viewModel);
        }
    }
}
