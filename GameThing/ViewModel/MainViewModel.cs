using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace GameThing
{
    public class MainViewModel : CommonBase
    {
        // PROPERTIES

        private List<LibraryCollection> _CollectionList;
        public List<LibraryCollection> CollectionList
        {
            get { return _CollectionList; }
            set { SetProperty(ref _CollectionList, value); }
        }

        private List<LibraryItem> _LibraryList;
        public List<LibraryItem> LibraryList
        {
            get { return _LibraryList; }
            set { SetProperty(ref _LibraryList, value); }
        }

        private LibraryCollection _SelectedCollection;
        public LibraryCollection SelectedCollection
        {
            get { return _SelectedCollection; }
            set { SetProperty(ref _SelectedCollection, value); }
        }

        private LibraryItem _SelectedItem;
        public LibraryItem SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }

        public string BackgroundFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.BackgroundFilePath))
                {
                    return Properties.Settings.Default.BackgroundFilePath;
                }
                return "pack://application:,,,/Resource Assets/background_default.jpg";
            }
        }

        public LibraryCollection SearchCollection { get; set; }

        public MediaItem CurrentSong
        {
            get { return MusicPlayer.CurrentSong; }
        }

        public BitmapImage CurrentSongImage
        {
            get
            {
                BitmapImage retImage = null;
                if (MusicPlayer.CurrentSong != null && MusicPlayer.CurrentSong.ImageData != null)
                {
                    retImage = MusicPlayer.CurrentSong.ImageData;
                    return retImage;
                }
                //Use default image
                Uri imageUri = new Uri("pack://application:,,,/Resource Assets/music_placeholder.jpg");
                retImage = new BitmapImage(imageUri);
                return retImage;
            }
        }

        public ProcessHandler CurrentProcess { get; set; }


        // CONSTRUCTORS

        public MainViewModel()
        {
            //TODO: Scan directory asynchronously
            LibraryFiler.ScanDirectory();

            List<LibraryCollection> FullCollectionList = LibraryFiler.AllCollections;
            CollectionList = new List<LibraryCollection>();
            //Hide empty collections
            foreach (LibraryCollection collection in FullCollectionList)
            {
                IEnumerable<LibraryItem> collectionItems = LibraryFiler.FullLibraryList.Where(x => x.MatchesCollection(collection) && x.IsVisible);
                if (collection.IsVisible && collectionItems.Count() > 0)
                {
                    CollectionList.Add(collection);
                }
            }
        }


        // METHODS

        public ProcessHandler GetSelectedItemProcess()
        {
            if (SelectedItem == null) return null;

            GameRunSettings selectedItemRunSettings = LibraryFiler.AllRunSettings.Find(x => x.GameConsoleId == SelectedItem.GameConsoleId);

            ProcessHandler currentProcess;
            if (SelectedItem.GameConsoleId == null || selectedItemRunSettings == null || selectedItemRunSettings.GameRunnerPath == null)
            {
                //No valid runner found, run directly
                currentProcess = new ProcessHandler(SelectedItem.FilePath);
            }
            else
            {
                //Run library item in the set runner
                currentProcess = new ProcessHandler(selectedItemRunSettings.GameRunnerPath, SelectedItem.FilePath, selectedItemRunSettings.GameRunnerArgs);
            }
            return currentProcess;
        }


        public void UpdateCurrentItemPlayTime()
        {
            //TODO: Don't use SelectedItem? What if selection is changed while process is still running?
            //Only log time if the session lasted more than 5 minutes
            if (CurrentProcess.RunTime != null && CurrentProcess.RunTime.TotalMinutes > 5)
            {
                //The above IF should prevent DateTime.Now actually being used so this is fine
                DateTime startTime = CurrentProcess.TimeStarted ?? DateTime.Now;
                DateTime endTime = CurrentProcess.TimeLastPolled ?? DateTime.Now;
                SelectedItem.AddPlayTime(startTime, endTime);
                LibraryFiler.SaveLibraryData();
            }
        }


        public void FilterSelectedCollection()
        {
            LibraryList = LibraryFiler.FullLibraryList.Where(x => x.MatchesCollection(SelectedCollection)).ToList();
            LibraryList = LibraryList.Where(x => x.IsVisible && x.FileExists).ToList();
            //TODO: Handle empty collections. Display message "No matching items"?
            //Sort the library list
            //TODO: Custom sort field from collection, not gross hard-coding
            if (SelectedCollection.SortField == "LastPlayed")
            {
                LibraryList.Sort((a, b) => {
                    if (a.LastPlayed == b.LastPlayed) return 0;
                    return a.LastPlayed < b.LastPlayed ? 1 : -1;
                });
            }
            else
            {
                //Sort by title
                LibraryList.Sort((a, b) => a.CompareTitleTo(b));
            }
        }


        public void CreateNewSearch()
        {
            SearchCollection = new LibraryCollection("Search");
        }


        public void UpdateMusicProperties()
        {
            RaisePropertyChanged("CurrentSong");
            RaisePropertyChanged("CurrentSongImage");
        }


    }
}
