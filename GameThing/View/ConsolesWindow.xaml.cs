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
    /// Interaction logic for ConsolesWindow.xaml
    /// </summary>
    public partial class ConsolesWindow : Window
    {
        // PROPERTIES

        private ConsolesViewModel vm;


        // CONSTRUCTORS

        public ConsolesWindow()
        {
            try
            {
                vm = new ConsolesViewModel();
                DataContext = vm;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                MessageBox.Show(ex.Message, "Failed Loading Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // METHODS

        private void OpenRunnerFileDialog()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.FileName = vm.SelectedConsole.GameRunnerPath;
                bool? dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    vm.SetSelectedRunnerPath(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //EVENT HANDLERS

        private void BtnRunnerFilePath_Click(object sender, RoutedEventArgs e)
        {
            OpenRunnerFileDialog();
        }


        private void TxtArgs_LostFocus(object sender, RoutedEventArgs e)
        {
            vm.SaveGameRunSettings();
        }


    }
}
