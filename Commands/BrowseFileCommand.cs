using Microsoft.Win32;
using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class BrowseFileCommand : ICommand
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

            if (viewModel.IsBuilding)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as MainViewModel;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.InitialDirectory = viewModel.WorkingDirectory;
            openFileDialog.FileName = "Folder Selection.";


            if (openFileDialog.ShowDialog() == true)
            {
                viewModel.WorkingDirectory = openFileDialog.FileName;
            }
        }
    }
}
