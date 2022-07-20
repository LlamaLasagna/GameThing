using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for LibraryItemCover.xaml
    /// </summary>
    public partial class LibraryItemCover : UserControl
    {
        // CONSTRUCTORS

        public LibraryItemCover()
        {
            InitializeComponent();
        }


        // METHODS

        private void ShowPlaceholder()
        {
            CoverImg.Visibility = Visibility.Hidden;
            CoverPlaceholder.Visibility = Visibility.Visible;
        }


        private void ShowImage()
        {
            CoverPlaceholder.Visibility = Visibility.Hidden;
            CoverImg.Visibility = Visibility.Visible;
        }


        private void CheckCoverImage()
        {
            try
            {
                LibraryItem item = (LibraryItem)DataContext;
                if (item == null || string.IsNullOrWhiteSpace(item.CoverImagePath) || !File.Exists(item.CoverImagePath))
                {
                    ShowPlaceholder();
                }
                else
                {
                    ShowImage();
                }
            }
            catch (Exception)
            {
                //Don't worry about it
            }
        }


        // EVENT HANDLERS

        private void DataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckCoverImage();
        }


        private void CoverImg_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ShowPlaceholder();
        }


    }
}
