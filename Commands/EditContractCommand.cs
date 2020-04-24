using NielsenPDFv2.ViewModels;
using NielsenPDFv2.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    class EditContractCommand : ICommand
    {
        private EditContract editContractView;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var mainViewModel = parameter as MainViewModel;
            if (mainViewModel.SelectedContract != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            var mainViewModel = parameter as MainViewModel;
            editContractView = new EditContract();
            editContractView.DataContext = mainViewModel;
            editContractView.Owner = Application.Current.MainWindow;
            editContractView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editContractView.btn_Close.Click += Btn_Close_Click1;
            editContractView.ShowDialog();
        }

        private void Btn_Close_Click1(object sender, RoutedEventArgs e)
        {
            editContractView.Close();
        }
    }
}
