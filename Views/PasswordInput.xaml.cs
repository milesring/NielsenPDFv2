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
    /// Interaction logic for PasswordInput.xaml
    /// </summary>
    public partial class PasswordInput : Window
    {
        private PasswordViewModel vm;
        public string Password
        {
            get { return vm.Password; }
        }
        public PasswordInput()
        {
            InitializeComponent();
            vm = DataContext as PasswordViewModel;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
