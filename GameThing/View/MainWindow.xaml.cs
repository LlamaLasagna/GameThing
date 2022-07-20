using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // CONSTANTS

        private const double MusicStartDelay = 3 * 1000; //Seconds * 1000


        // PROPERTIES

        private MainViewModel vm;
        private TaskTrayIcon trayIcon;
        private OverlayMenuWindow overlayMenu;
        private InputListener input;

        private Timer MusicStartTimer;


        // CONSTRUCTORS

        public MainWindow()
        {
            //Initialise main window and view model
            try
            {
                //TODO: Make the initialisation file directory scan async (on another thread)
                vm = new MainViewModel();
                DataContext = vm;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HandleCriticalError(ex, "Failed launching. View error log for more information.");
            }

            //Initialise task tray icon
            try
            {
                trayIcon = new TaskTrayIcon();
                trayIcon.AddMenuItem("Exit", TrayIcon_Exit);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error initialising task tray icon.");
            }

            //Initialise music player
            try
            {
                MusicPlayer.OnMusicStarted += MusicPlayer_MusicStarted;
                MusicPlayer.ScanDirectory();
                MusicStartDelayed();
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error initialising music player.");
            }

            //Initialise global input listener
            try
            {
                input = new InputListener();
                input.KeyboardUpdated += Input_KeyPressed;
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error initialising input listener.");
            }

            //Register menu actions
            MenuHandler.HomeAction += Menu_Home;

            ShowCollectionPage();

#if DEBUG
            WindowStyle = WindowStyle.SingleBorderWindow;
#endif
        }


        // METHODS

        private void ShowPage(Page nextPage)
        {
            MainPageFrame.Content = nextPage;
            HideSubView();
        }


        private void ShowSubView(Page subViewPage)
        {
            //Set the content of the sub view
            SubViewFrame.Content = subViewPage;
            //Set the sub view title
            SubViewTitle.Content = "";
            if (!string.IsNullOrEmpty(subViewPage.Title))
            {
                SubViewTitle.Content = subViewPage.Title;
            }
            //Show the sub view
            SubView.Visibility = Visibility.Visible;
        }


        public void HideSubView()
        {
            SubView.Visibility = Visibility.Hidden;
            SubViewFrame.Content = null;
        }


        public void ShowCollectionPage()
        {
            try
            {
                CollectionPage nextPage = new CollectionPage(this, vm);
                ShowPage(nextPage);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed displaying collections page. View error log for more information.");
            }
        }


        public void ShowLibraryPage()
        {
            try
            {
                LibraryPage nextPage = new LibraryPage(this, vm);
                ShowPage(nextPage);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed displaying library page. View error log for more information.");
            }
        }


        public void ShowSearchPage()
        {
            try
            {
                SearchPage nextPage = new SearchPage(this, vm);
                ShowPage(nextPage);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed displaying search page. View error log for more information.");
            }
        }


        public void ShowLibraryItemPage()
        {
            try
            {
                LibraryItemPage nextPage = new LibraryItemPage(this, vm);
                ShowSubView(nextPage);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed displaying library item page. View error log for more information.");
            }
        }


        private void ShowSettingsPage()
        {
            try
            {
                SettingsPage nextPage = new SettingsPage(this, vm);
                ShowPage(nextPage);
                //TODO: On page close, re-scan library (if library path has changed?)
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed displaying settings page. View error log for more information.");
            }
        }


        public void ShowPlayOverlay()
        {
            MusicStop();
            PlayOverlay.Visibility = Visibility.Visible;
            //TODO: Disable most keyboard events
            //TODO: Enable menu overlay
        }


        public void HidePlayOverlay()
        {
            PlayOverlay.Visibility = Visibility.Hidden;
            //TODO: Re-enable most keyboard events
            //TODO: Disable menu overlay
            //HideMenuOverlay();
            MusicStartDelayed();
        }


        public void ShowMenuOverlay()
        {
            if (overlayMenu == null || overlayMenu.IsClosed)
            {
                overlayMenu = new OverlayMenuWindow(this);
            }
            overlayMenu.Show();
        }


        public void HideMenuOverlay()
        {
            if (overlayMenu == null || overlayMenu.IsClosed) return;
            overlayMenu.Close();
        }


        public void HandleError(Exception ex, string userMessage = null)
        {
            //Log and display the error message
            try
            {
                Tools.LogError(ex);
            }
            catch (Exception)
            {
                //Error logging error. There is no hope!
            }
            
            if (userMessage == null)
            {
                userMessage = ex.Message;
            }
            //TODO: Use an always-on-top controller-friendly message box
            MessageBox.Show(userMessage, "GameThing Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public void HandleCriticalError(Exception ex, string userMessage = null)
        {
            //Handle the exception and shut down the application
            HandleError(ex, userMessage);
            CloseApplication();
        }


        public void ExitProcess()
        {
            vm.CurrentProcess.EndProcess();
        }


        private void CloseApplication()
        {
            try
            {
                //Stop timers
                if (MusicStartTimer != null && MusicStartTimer.Enabled) MusicStartTimer.Stop();
                //Ensure the task tray icon is disposed before closing
                trayIcon.Close();
                //Ensure the input listener is disposed
                input.Dispose();
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
            }

            //Shut down the entire application (all windows)
            //Application.Current.Shutdown(); //This sometimes gets a null reference exception
            Process.GetCurrentProcess().Kill();
        }


        public void MusicStartDelayed()
        {
            if (MusicStartTimer != null && MusicStartTimer.Enabled)
            {
                MusicStartTimer.Stop();
            }
            MusicStartTimer = new Timer(MusicStartDelay);
            MusicStartTimer.Elapsed += MusicStartTimer_Elapsed;
            MusicStartTimer.Start();
        }


        public void MusicStart()
        {
            MusicPlayer.PlayQueue();
        }


        public void MusicStop()
        {
            MusicPlayer.StopFadeOut();
        }


        public void MusicNext()
        {
            MusicPlayer.Next();
        }


        // EVENT HANDLERS

        private void Window_Closed(object sender, EventArgs e)
        {
            CloseApplication();
        }


        private void TrayIcon_Exit(object sender, EventArgs e)
        {
            //TODO: Ask for confirmation
            CloseApplication();
        }


        private void Menu_Home(object sender, EventArgs e)
        {
            if (PlayOverlay.Visibility == Visibility.Visible)
            {
                if (overlayMenu == null || overlayMenu.IsClosed)
                {
                    ShowMenuOverlay();
                }
                else
                {
                    HideMenuOverlay();
                }
            }
        }


        private void Input_KeyPressed(object sender, DirectKeyboardEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.State.PressedKeys.Contains(SharpDX.DirectInput.Key.Home))
                {
                    MenuHandler.Home();
                }
            });
        }


        private void BtnHomePage_Click(object sender, RoutedEventArgs e)
        {
            ShowCollectionPage();
        }


        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchPage();
        }


        private void BtnDebug_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsPage();
        }


        private void BtnSubViewClose_Click(object sender, MouseButtonEventArgs e)
        {
            HideSubView();
        }


        private void BtnPlayOverlayClose_Click(object sender, MouseButtonEventArgs e)
        {
            HidePlayOverlay();
        }


        private void MusicPlayer_MusicStarted(object sender, EventArgs e)
        {
            vm.UpdateMusicProperties();
        }


        private void MusicStartTimer_Elapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MusicStart();
                MusicStartTimer.Stop();
                MusicStartTimer.Dispose();
            });
        }


        private void BtnSkipSong_Click(object sender, RoutedEventArgs e)
        {
            MusicNext();
        }


    }
}
