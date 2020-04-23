using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    class SaveContractCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var viewModel = parameter as MainViewModel;
            if(viewModel.SelectedContract == null)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as MainViewModel;
            viewModel.SaveContract();
        }
    }
}
