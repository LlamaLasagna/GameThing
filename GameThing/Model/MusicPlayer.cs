using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace GameThing
{
    public static class MusicPlayer
    {
        // CONSTANTS

        private const double FadeInTime = 5; //Total time (in seconds) for a fade in to take
        private const double FadeOutTime = 1; //Total time (in seconds) for a fade out to take
        private const double FadeIncrement = 0.05; //Amount to change the volume by each fade tick. Volume range is 0 - 1


        // PROPERTIES

        private static MediaPlayer Player;
        private static DispatcherTimer FadeInTimer;
        private static DispatcherTimer FadeOutTimer;
        private static DispatcherTimer DurationTimer;

        public static List<string> AllMusic;
        public static List<string> CurrentPlaylist;
        public static MediaItem CurrentSong;
        public static bool IsLoop = true;
        public static bool IsShuffle = true;

        private static double _Volume = 0.5;
        public static double Volume
        {
            get { return _Volume; }
            set
            {
                _Volume = value;
                if (!IsVolumeFading) Player.Volume = value;
            }
        }

        public static bool IsVolumeFading
        {
            get { return (FadeInTimer != null && FadeInTimer.IsEnabled) || (FadeOutTimer != null && FadeOutTimer.IsEnabled); }
        }

        public static EventHandler OnMusicStarted;
        public static EventHandler OnMusicCompleted;
        public static EventHandler OnPlaylistCompleted;


        // METHODS

        private static void InitPlayer()
        {
            Player = new MediaPlayer();
        }


        private static void FadeInTimer_Elapsed(object sender, EventArgs e)
        {
            Player.Volume += FadeIncrement;
            Player.Volume = Math.Round(Player.Volume, 2); //Tidy up rounding issues
            if (Player.Volume >= Volume)
            {
                FadeInTimer.Stop();
            }
        }


        private static void FadeOutTimer_Elapsed(object sender, EventArgs e)
        {
            Player.Volume -= FadeIncrement;
            Player.Volume = Math.Round(Player.Volume, 2); //Tidy up rounding issues
            if (Player.Volume <= 0)
            {
                FadeOutTimer.Stop();
                Stop();
            }
        }


        private static void DurationTimer_Elapsed(object sender, EventArgs e)
        {
            if (Player.Position >= Player.NaturalDuration)
            {
                //Media has ended
                OnMusicCompleted?.Invoke(null, EventArgs.Empty);
                Next();
            }
        }


        public static void ScanDirectory(string fileDirectory = null)
        {
            //Default value if none was given
            if (fileDirectory == null) fileDirectory = Properties.Settings.Default.MusicFileDirectory;
            //Handle non-existing directory
            if (!Directory.Exists(fileDirectory)) return;
            //Get all files in the directory
            //TODO: Support more extensions than mp3
            string[] musicFiles = Directory.GetFiles(fileDirectory, "*.mp3", SearchOption.AllDirectories);
            AllMusic = musicFiles.ToList();
            CurrentPlaylist = AllMusic;
        }


        public static void ShuffleQueue()
        {
            Random rng = new Random(); //Seeds based on system clock
            CurrentPlaylist = CurrentPlaylist.OrderBy(x => rng.Next()).ToList();
        }


        public static void Play(string filePath)
        {
            //Play the given music
            Player.Open(new Uri(filePath));
            Player.Play();
            CurrentSong = new MediaItem(filePath);
            //Set the duration timer
            if (DurationTimer != null && DurationTimer.IsEnabled)
            {
                DurationTimer.Stop();
            }
            DurationTimer = new DispatcherTimer();
            DurationTimer.Interval = TimeSpan.FromMilliseconds(1000);
            DurationTimer.Tick += DurationTimer_Elapsed;
            DurationTimer.Start();
            //Trigger event
            OnMusicStarted?.Invoke(null, EventArgs.Empty);
        }


        public static void Stop()
        {
            //Stop playing music
            Player.Stop();
            Player.Close();
            CurrentSong = null;
            //Stop the duration timer
            DurationTimer.Stop();
        }


        public static void PlayFadeIn(string filePath)
        {
            Player.Volume = 0;
            Play(filePath);
            //Check if already fading in
            if (FadeInTimer != null && FadeInTimer.IsEnabled) return;
            //Stop fade out timer if it's active
            if (FadeOutTimer != null && FadeOutTimer.IsEnabled)
            {
                FadeOutTimer.Stop();
            }
            //Set fade in timer
            double fadeInterval = (FadeInTime * 1000) * FadeIncrement;
            FadeInTimer = new DispatcherTimer();
            FadeInTimer.Interval = TimeSpan.FromMilliseconds(fadeInterval);
            FadeInTimer.Tick += FadeInTimer_Elapsed;
            FadeInTimer.Start();
        }


        public static void PlayQueue()
        {
            if (CurrentPlaylist == null || CurrentPlaylist.Count == 0) return;
            if (IsShuffle)
            {
                ShuffleQueue();
            }
            if (Player == null)
            {
                InitPlayer();
            }
            string song = CurrentPlaylist.First();
            if (CurrentSong != null && CurrentSong.FilePath == song) return;
            PlayFadeIn(song);
        }


        public static void Next()
        {
            int currentSongIndex = -1;
            if (CurrentSong != null)
            {
                currentSongIndex = CurrentPlaylist.IndexOf(CurrentSong.FilePath);
            }
            currentSongIndex++;
            if (currentSongIndex >= CurrentPlaylist.Count)
            {
                //End of playlist
                if (IsLoop)
                {
                    //Reset to start of playlist
                    currentSongIndex = 0;
                    //Reshuffle if necessary
                    //NOTE: Maybe make this smarter? Reshuffling means potential for the same song twice in a row
                    if (IsShuffle)
                    {
                        ShuffleQueue();
                    }
                }
                else
                {
                    Stop();
                    OnPlaylistCompleted?.Invoke(null, EventArgs.Empty);
                    return;
                }
            }
            string nextSong = CurrentPlaylist[currentSongIndex];
            Play(nextSong);
        }


        public static void StopFadeOut()
        {
            //Check if already fading out
            if (FadeOutTimer != null && FadeOutTimer.IsEnabled) return;
            //Stop fade in timer if it's active
            if (FadeInTimer != null && FadeInTimer.IsEnabled)
            {
                FadeInTimer.Stop();
            }
            //Set fade out timer
            double fadeInterval = (FadeOutTime * 1000) * FadeIncrement;
            FadeOutTimer = new DispatcherTimer();
            FadeOutTimer.Interval = TimeSpan.FromMilliseconds(fadeInterval);
            FadeOutTimer.Tick += FadeOutTimer_Elapsed;
            FadeOutTimer.Start();
        }


    }
}
