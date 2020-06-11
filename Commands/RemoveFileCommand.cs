using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class RemoveFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            MainViewModel viewModel = parameter as MainViewModel;
            if (viewModel.Files.Count < 1)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            MainViewModel viewModel = parameter as MainViewModel;
            foreach(var file in viewModel.SelectedFiles)
            {
                viewModel.RemoveFile(file);
            }
            viewModel.RefreshFileNums();
        }
    }
}
