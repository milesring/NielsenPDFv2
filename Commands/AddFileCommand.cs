using Microsoft.Win32;
using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class AddFileCommand : ICommand
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
            MainViewModel viewModel = parameter as MainViewModel;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "PDF documents (*.pdf)|*.pdf";
            if (viewModel.WorkingDirectory != string.Empty)
            {
                openFileDialog.InitialDirectory = viewModel.WorkingDirectory;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    viewModel.AddFile(fileName);
                }
            }
        }
    }
}
