using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class SettingsViewModel : CommonBase
    {
        // PROPERTIES

        public string LibraryFileDirectory
        {
            get { return Properties.Settings.Default.LibraryFileDirectory; }
            set
            {
                Properties.Settings.Default.LibraryFileDirectory = value;
                SaveSettings();
                RaisePropertyChanged("LibraryFileDirectory");
            }
        }

        public string SteamAppsDirectory
        {
            get { return Properties.Settings.Default.SteamAppsDirectory; }
            set
            {
                Properties.Settings.Default.SteamAppsDirectory = value;
                SaveSettings();
                RaisePropertyChanged("SteamAppsDirectory");
            }
        }

        public string MusicFileDirectory
        {
            get { return Properties.Settings.Default.MusicFileDirectory; }
            set
            {
                Properties.Settings.Default.MusicFileDirectory = value;
                SaveSettings();
                RaisePropertyChanged("MusicFileDirectory");
            }
        }

        public string SplashFileDirectory
        {
            get { return Properties.Settings.Default.SplashFileDirectory; }
            set
            {
                Properties.Settings.Default.SplashFileDirectory = value;
                SaveSettings();
                RaisePropertyChanged("SplashFileDirectory");
            }
        }

        public string BackgroundFilePath
        {
            get { return Properties.Settings.Default.BackgroundFilePath; }
            set
            {
                Properties.Settings.Default.BackgroundFilePath = value;
                SaveSettings();
                RaisePropertyChanged("BackgroundFilePath");
            }
        }


        // METHODS

        public void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }


    }
}
