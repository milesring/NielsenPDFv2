using NielsenPDFv2.ViewModels;
using NielsenPDFv2.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    class OpenSettingsCommand : ICommand
    {
        private EditContract editContractView;
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
            var mainViewModel = parameter as MainViewModel;
            editContractView = new EditContract();
            editContractView.Owner = Application.Current.MainWindow;
            editContractView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mainViewModel.Refresh = editContractView.ShowDialog();

        }
    }
}
