using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NielsenPDFv2.Commands
{
    public class LoadContractsCommand : ICommand
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
            LoadItems(parameter);
        }

        private async void LoadItems(object parameter)
        {
            var viewModel = parameter as SettingsViewModel;
            var selectedIndex = viewModel.SelectedIndex;
            var items = await App.Database.GetContractsAsync();
            viewModel.Contracts.Clear();
            foreach(var item in items)
            {
                viewModel.Contracts.Add(item);
            }
            viewModel.SelectedIndex = selectedIndex;
            
        }
    }
}
