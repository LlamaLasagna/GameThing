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
using System.Windows.Shapes;

namespace GameThing
{
    /// <summary>
    /// Interaction logic for OverlayMenuWindow.xaml
    /// </summary>
    public partial class OverlayMenuWindow : Window
    {
        // PROPERTIES

        private MainWindow parent;

        public bool IsClosed = false;


        // CONSTRUCTORS

        public OverlayMenuWindow(MainWindow parentWindow)
        {
            InitializeComponent();
            parent = parentWindow;
        }


        // METHODS

        private void CloseOverlay()
        {
            parent.HideMenuOverlay();
        }


        // EVENT HANDLERS

        private void Window_Loaded(object sender, EventArgs e)
        {
            //TODO: Make this actually work
            Focus();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
        }


        private void BtnOverlayClose_Click(object sender, MouseButtonEventArgs e)
        {
            CloseOverlay();
        }


        private void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            CloseOverlay();
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            parent.ExitProcess();
            CloseOverlay();
        }


    }
}
