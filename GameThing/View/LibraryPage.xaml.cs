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
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        // PROPERTIES

        private MainWindow parent;
        private MainViewModel vm;


        // CONSTRUCTORS

        public LibraryPage(MainWindow parentWindow, MainViewModel parentViewModel)
        {
            InitializeComponent();
            parent = parentWindow;
            vm = parentViewModel;
            DataContext = vm;

            vm.FilterSelectedCollection();
        }


        // METHODS


        private void RunSelectedItem()
        {
            //Run the selected Library Item in the corresponding Emulator/Application
            try
            {
                vm.CurrentProcess = vm.GetSelectedItemProcess();
                vm.CurrentProcess.OnUpdate += ProcessUpdate;
                vm.CurrentProcess.OnProcessEnd += ProcessEnded;
                vm.CurrentProcess.Run();

                parent.ShowPlayOverlay();
            }
            catch (Exception ex)
            {
                parent.HandleError(ex, "Failed running game. View error log for more information.");
            }
        }


        // EVENT HANDLERS

        private void ProcessUpdate(object sender, EventArgs e)
        {
            vm.UpdateCurrentItemPlayTime();
        }


        private void ProcessEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                parent.HidePlayOverlay();
            });

            try
            {
                vm.UpdateCurrentItemPlayTime();
            }
            catch (Exception ex)
            {
                parent.HandleError(ex, "Error updating play time.");
            }
            List<string> processErrors = vm.CurrentProcess.Errors;
            if (processErrors != null && processErrors.Count > 0)
            {
                int errorCount = processErrors.Count;
                string errorMessage = processErrors.Last();
                string promptMessage = "There was an error while running the process.";
                if (errorCount > 1)
                {
                    promptMessage = $"There were {errorCount} errors while running the process.";
                }
                MessageBox.Show(promptMessage + "\r\n\r\nLast error message:\r\n" + errorMessage, "Process Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            RunSelectedItem();
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            parent.ShowLibraryItemPage();
        }


    }
}
