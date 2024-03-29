﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameThing
{
    /// <summary>
    /// Interaction logic for LibraryItemPage.xaml
    /// </summary>
    public partial class LibraryItemPage : Page
    {
        // PROPERTIES

        private MainWindow parent;
        private LibItemViewModel vm;


        // CONSTRUCTORS

        public LibraryItemPage(MainWindow parentWindow, MainViewModel parentViewModel)
        {
            InitializeComponent();
            parent = parentWindow;
            vm = new LibItemViewModel();
            DataContext = vm;

            //TODO: Store original object (or values) for reverting if not saved (may need to clone object)
            vm.SelectedItem = parentViewModel.SelectedItem;
        }


        // METHODS

        private void OpenCoverImageFileDialog()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.FileName = vm.SelectedItem.CoverImagePath;
                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    vm.SelectedItem.CoverImagePath = dialog.FileName;
                    vm.RaiseItemChanged();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // EVENT HANDLERS

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.UpdateLibraryFile();
                parent.HideSubView();
            }
            catch (Exception ex)
            {
                parent.HandleError(ex);
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Revert SelectedItem back to original values
            parent.HideSubView();
        }


        private void BtnCoverImage_Click(object sender, RoutedEventArgs e)
        {
            OpenCoverImageFileDialog();
        }


        private void BtnCoverImageClear_Click(object sender, RoutedEventArgs e)
        {
            vm.SelectedItem.CoverImagePath = null;
            vm.RaiseItemChanged();
        }


        private void TxtTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.OemComma || e.Key == Key.OemSemicolon)
            {
                try
                {
                    vm.SubmitCurrentTag();
                    e.Handled = true;
                    TextBox TxtTags = (TextBox)sender;
                    TxtTags.CaretIndex = TxtTags.Text.Length;
                }
                catch (Exception ex)
                {
                    parent.HandleError(ex);
                }
            }
        }


    }
}
