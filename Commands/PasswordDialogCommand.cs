using NielsenPDFv2.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class PasswordDialogCommand : ICommand
    {
        public string Password { get; set; }
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
            PasswordInput passwordInput = new PasswordInput();
            passwordInput.Owner = Application.Current.MainWindow;
            passwordInput.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            passwordInput.ShowDialog();
            Password = passwordInput.Password;
        }
    }
}
