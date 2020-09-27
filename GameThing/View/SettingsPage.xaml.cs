using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        // PROPERTIES

        private MainWindow parent;
        private SettingsViewModel vm;


        // CONSTRUCTORS

        public SettingsPage(MainWindow parentWindow, MainViewModel parentViewModel)
        {
            InitializeComponent();
            parent = parentWindow;
            //vm = parentViewModel;
            vm = new SettingsViewModel();
            DataContext = vm;
        }


        // METHODS

        private string OpenDirectoryFileDialog()
        {
            try
            {
                //Hacky method of doing folder selection in file dialog
                //TODO: Find a better way (NOTE: No FolderBrowserDialog from Windows Forms! Gross!)
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.ValidateNames = false;
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.FileName = "Folder Selection";
                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    string directoryPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    return directoryPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }


        private void OpenLibraryFileDialog()
        {
            string directoryPath = OpenDirectoryFileDialog();
            if (!string.IsNullOrEmpty(directoryPath))
            {
                vm.LibraryFileDirectory = directoryPath;
            }
        }


        private void OpenMusicFileDialog()
        {
            string directoryPath = OpenDirectoryFileDialog();
            if (!string.IsNullOrEmpty(directoryPath))
            {
                vm.MusicFileDirectory = directoryPath;
            }
        }


        private void OpenSplashFileDialog()
        {
            string directoryPath = OpenDirectoryFileDialog();
            if (!string.IsNullOrEmpty(directoryPath))
            {
                vm.SplashFileDirectory = directoryPath;
            }
        }


        private void OpenBackgroundFileDialog()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.FileName = vm.BackgroundFilePath;
                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    vm.BackgroundFilePath = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OpenApplicationDirectory()
        {
            Process.Start(".");
        }


        // EVENT HANDLERS

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult userResp = MessageBox.Show("Really exit GameThing and return to desktop?", "Really Quit?", MessageBoxButton.OKCancel);
            if (userResp == MessageBoxResult.OK || userResp == MessageBoxResult.Yes)
            {
                parent.Close();
            }
        }


        private void BtnShutdown_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult userResp = MessageBox.Show("Really power-off the system?", "Shutdown", MessageBoxButton.OKCancel);
            if (userResp == MessageBoxResult.OK || userResp == MessageBoxResult.Yes)
            {
                Process.Start("shutdown", "/s /t 0");
            }
        }


        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenApplicationDirectory();
        }


        private void BtnLibFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenLibraryFileDialog();
        }


        private void BtnMusicFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenMusicFileDialog();
        }


        private void BtnSplashFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenSplashFileDialog();
        }


        private void BtnBackFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenBackgroundFileDialog();
        }


    }
}
