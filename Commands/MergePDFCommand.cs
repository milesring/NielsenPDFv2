using NielsenPDFv2.ViewModels;
using System;
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

        public void Execute(object parameter)
        {
            MainViewModel viewModel = parameter as MainViewModel;
            viewModel.MergePDFs();
        }
        #endregion
    }
}
