using System;
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
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        // PROPERTIES

        private MainWindow parent;
        private MainViewModel vm;


        // CONSTRUCTORS

        public SearchPage(MainWindow parentWindow, MainViewModel parentViewModel)
        {
            InitializeComponent();
            parent = parentWindow;
            vm = parentViewModel;
            DataContext = vm;

            vm.CreateNewSearch();
            TxtSearchInput.Focus();
        }


        // METHODS

        public void Search()
        {
            vm.SelectedCollection = vm.SearchCollection;
            parent.ShowLibraryPage();
        }


        // EVENT HANDLERS

        private void TxtSearchInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }


        private void BtnSearchSubmit_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }


    }
}
