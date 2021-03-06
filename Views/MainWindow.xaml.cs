﻿using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using NielsenPDFv2.Models;
using NielsenPDFv2.ViewModels;
using System.Diagnostics;
using System.Windows.Threading;
using iText.IO.Util;

namespace NielsenPDFv2.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer t;
        Point startPoint = new Point();
        int startIndex = -1;
        MainViewModel viewModel;
        PDFPreview pdfPreview;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = (MainViewModel)DataContext;
            ResetTimer();
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

        #region FileMouseMove
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var file = sender as ListViewItem;
            viewModel.HighlightedFile = file.Content as FileObject;
        }
        #endregion

        #region PDFPreview
        void ResetTimer()
        {
            if (t != null)
            {
                t.Tick -= t_Elapsed;
            }
            t = new DispatcherTimer();
            t.Tick += t_Elapsed;
            t.Interval = TimeSpan.FromSeconds(1);
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (viewModel.PDFPreviews)
            {
                t.Start();
            }
        }

        private void ListViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewModel.PDFPreviews || t.IsEnabled)
            {
                t.Stop();
                ResetTimer();
            }
        }

        void t_Elapsed(object sender, EventArgs e)
        {
            if(pdfPreview != null && pdfPreview.IsLoaded)
            {
                return;
            }
            pdfPreview = new PDFPreview(viewModel.HighlightedFile);
            pdfPreview.Owner = Application.Current.MainWindow;
            //offset so the window opens with the mouse positioned in the webbrowser control, due to mouse events not working how they should
            pdfPreview.Left = PointToScreen(Mouse.GetPosition(this)).X-10;
            pdfPreview.Top = PointToScreen(Mouse.GetPosition(this)).Y-10;
            pdfPreview.Show();
        }
        #endregion
    }
}
