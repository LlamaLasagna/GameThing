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
    /// Interaction logic for CollectionPage.xaml
    /// </summary>
    public partial class CollectionPage : Page
    {
        // PROPERTIES

        private MainWindow parent;
        private MainViewModel vm;


        // CONSTRUCTOR

        public CollectionPage(MainWindow parentWindow, MainViewModel parentViewModel)
        {
            InitializeComponent();
            parent = parentWindow;
            vm = parentViewModel;
            DataContext = vm;
        }


        // EVENT HANDLERS

        private void BtnViewCollection_Click(object sender, RoutedEventArgs e)
        {
            parent.ShowLibraryPage();
        }


    }
}
