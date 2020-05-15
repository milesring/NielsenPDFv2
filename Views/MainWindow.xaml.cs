using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;

namespace NielsenPDFv2.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point startPoint = new Point();
        int startIndex = -1;
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = (MainViewModel)DataContext;
        }

        public void cb_Contracts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectedContract = (Contract)cb_Contracts.SelectedItem;
            if(viewModel.SelectedContract == null)
            {
                cb_Contracts.SelectedIndex = 0;
            }
            cb_Contracts.IsDropDownOpen = false;
        }

        private void cb_Contracts_Loaded(object sender, RoutedEventArgs e)
        {
            cb_Contracts.SelectedIndex = 0;
        }

        private void lv_Input_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectedFiles = lv_Input.SelectedItems.Cast<FileObject>().ToList();
        }

        #region DragNDrop
        // All drag and drop taken from
        // https://www.codeproject.com/Articles/1236549/Csharp-WPF-listview-Drag-Drop-a-Custom-Item
        private void lv_Input_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void lv_Input_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                       Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null) return;           // Abort
                                                            // Find the data behind the ListViewItem
                Models.FileObject item = (Models.FileObject)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (item == null) return;                   // Abort
                                                            // Initialize the drag & drop operation
                startIndex = listView.SelectedIndex;
                DataObject dragData = new DataObject("FileObject", item);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void lv_Input_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FileObject") || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void lv_Input_Drop(object sender, DragEventArgs e)
        {
            int index = -1;

            if (e.Data.GetDataPresent("FileObject") && sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null)
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }
                // Find the data behind the ListViewItem
                Models.FileObject item = (Models.FileObject)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                // Move item into observable collection 
                // (this will be automatically reflected to lstView.ItemsSource)
                e.Effects = DragDropEffects.Move;
                index = viewModel.Files.IndexOf(item);
                if (startIndex >= 0 && index >= 0)
                {
                    viewModel.Files.Move(startIndex, index);
                }
                startIndex = -1;        // Done!

                viewModel.RefreshFileNums();
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        #endregion
    }
}
