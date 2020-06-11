using NielsenPDFv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NielsenPDFv2.Views
{
    /// <summary>
    /// Interaction logic for EditContract.xaml
    /// </summary>
    public partial class EditContract : Window
    {
        SettingsViewModel vm;
        public EditContract(MainViewModel mainViewModel)
        {
            InitializeComponent();
            vm = (SettingsViewModel)DataContext;
            vm.MainViewModel = mainViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = vm.Refresh;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
