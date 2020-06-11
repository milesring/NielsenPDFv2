using MSHTML;
using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NielsenPDFv2.Views
{
    /// <summary>
    /// Interaction logic for PDFPreview.xaml
    /// </summary>
    public partial class PDFPreview : Window
    {
        private PDFPreviewViewModel vm;
        public string Filepath { get; set; }
        public PDFPreview(FileObject file)
        {
            InitializeComponent();
            Filepath = file.FilePath;
            var destString = Filepath;
            vm = DataContext as PDFPreviewViewModel;
            if (file.PasswordProtected)
            {

                //currently not supporting passworded previews
                this.Close();
                destString += "#pass=" + file.Password;
            }
            
            webBrowser.Navigate(new Uri(destString));
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
