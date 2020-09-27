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
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        // PROPERTIES

        private SplashViewModel vm;
        private InputListener input;
        private bool IsEnding = false;


        // CONSTRUCTORS

        public SplashWindow()
        {
            try
            {
                InitializeComponent();
                vm = new SplashViewModel();
                DataContext = vm;

                //Setup input listener
                input = new InputListener();
                input.KeyboardUpdated += Input_KeyPressed;

                //If no video is set, skip the splash screen
                if (vm.VideoSource == null)
                {
                    EndSplash();
                }
            }
            catch (Exception ex)
            {
                //TODO: Show controller-friendly error message
                Tools.LogError(ex);
                EndSplash();
            }
        }


        // METHODS

        private void EndSplash()
        {
            if (!IsEnding)
            {
                IsEnding = true;
                MainWindow mainWin = new MainWindow();
                mainWin.Show();
            }
            Close();
        }


        // EVENT HANDLERS

        private void Window_Closed(object sender, EventArgs e)
        {
            input.Dispose();
        }


        private void Input_KeyPressed(object sender, DirectKeyboardEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Skip the splash screen when any key is pressed
                EndSplash();
            });
        }


        private void SplashVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            EndSplash();
        }


    }
}
