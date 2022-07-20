using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class LibraryItem
    {
        // PROPERTIES

        public string FilePath { get; set; }
        public long FileSize { get; set; }
        [JsonIgnore]
        public string FileExtension { get; set; }
        [JsonIgnore]
        public bool FileExists { get; set; }

        public string Title { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public List<string> Tags { get; set; }
        public bool IsVisible { get; set; }
        public string CoverImagePath { get; set; }

        public DateTime? FirstPlayed { get; set; }
        public DateTime? LastPlayed { get; set; }
        public double HoursPlayed { get; set; }
        public int PlayCount { get; set; }

        public int? GameConsoleId { get; set; }
        [JsonIgnore]
        public GameConsole GameConsoleInfo { get; set; }
        [JsonIgnore]
        public string GameConsoleTitle
        {
            get
            {
                if (GameConsoleInfo == null) return "";
                return GameConsoleInfo.Title;
            }
        }

        [JsonIgnore]
        public string NumPlayers
        {
            get
            {
                if (MinPlayers <= 0) { return "Unknown"; }
                if (MinPlayers >= MaxPlayers) { return MinPlayers.ToString(); }
                return MinPlayers + " to " + MaxPlayers;
            }
        }
        [JsonIgnore]
        public string PlayTime
        {
            get
            {
                if (HoursPlayed == 0) { return "None"; }
                int FullHours = (int)Math.Floor(HoursPlayed);
                if (FullHours == 0)
                {
                    double MinsPlayed = HoursPlayed * 60;
                    int RoundMins = Tools.RoundToNearest(MinsPlayed, 5);
                    return $"About {RoundMins} Minutes";
                }
                else
                {
                    string output = $"About {FullHours} Hour";
                    if (FullHours > 1) output += "s";
                    return output;
                }
            }
        }
        [JsonIgnore]
        public string LastPlayedString
        {
            get
            {
                if (LastPlayed == null) return "";
                string formattedDate = Tools.DateFromNow(LastPlayed);
                return "Last Played " + formattedDate;
            }
        }
        [JsonIgnore]
        public string TagsSimple
        {
            get
            {
                if (Tags == null) { return "None"; }
                return string.Join(", ", Tags);
            }
        }


        // CONSTRUCTORS

        public LibraryItem()
        {
            //Set default values
            FileExists = true;
            MinPlayers = 0;
            MaxPlayers = 0;
            IsVisible = true;
            HoursPlayed = 0;
            PlayCount = 0;
        }


        // METHODS

        public override string ToString()
        {
            return Title;
        }


        public bool MatchesCollection(LibraryCollection collection)
        {
            //Check this item's console is in the collection
            if (collection.GameConsoleIds != null && collection.GameConsoleIds.Count > 0 && !collection.GameConsoleIds.Contains(GameConsoleId ?? 0))
            {
                return false;
            }
            //Check that this item supports the number of players
            //Treat "Unknown" player count as one player
            int evalMinPlayers = MinPlayers > 0 ? MinPlayers : 1;
            int evalMaxPlayers = MaxPlayers > 0 ? MaxPlayers : 1;
            if (collection.MinPlayers > evalMaxPlayers)
            {
                return false;
            }
            //Check that this item has the required play time
            if (collection.MinHours > 0 && collection.MinHours > HoursPlayed) { return false; }
            //Check that this item has all tags in the collection
            if (collection.Tags != null && collection.Tags.Count > 0)
            {
                foreach (string tag in collection.Tags)
                {
                    if (!Tags.Contains(tag)) { return false; }
                }
            }
            //Check that this item matches the search string
            if (collection.SearchString != null)
            {
                string filterString = collection.SearchString.ToLower();
                if (!string.IsNullOrWhiteSpace(filterString) && !Title.ToLower().Contains(filterString) && !TagsSimple.ToLower().Contains(filterString))
                {
                    return false;
                }
            }
            return true;
        }


        public int CompareTitleTo(LibraryItem compareItem)
        {
            string mySortTitle = Title;
            string compareSortTitle = compareItem.Title;
            //Remove "the" from the start of titles
            if (mySortTitle.Length > 4 && mySortTitle.Substring(0, 4).ToLower() == "the ")
            {
                mySortTitle = mySortTitle.Substring(4);
            }
            if (compareSortTitle.Length > 4 && compareSortTitle.Substring(0, 4).ToLower() == "the ")
            {
                compareSortTitle = compareSortTitle.Substring(4);
            }

            return mySortTitle.CompareTo(compareSortTitle);
        }


        public void AddPlayTime(DateTime startTime, DateTime endTime)
        {
            if (startTime < LastPlayed && LastPlayed < endTime)
            {
                startTime = LastPlayed ?? startTime;
            }
            else
            {
                PlayCount++;
            }
            TimeSpan playTime = endTime - startTime;
            HoursPlayed = Math.Round(HoursPlayed + playTime.TotalHours, 2);
            if (FirstPlayed == null || startTime < FirstPlayed)
            {
                FirstPlayed = startTime;
            }
            if (LastPlayed == null || endTime > LastPlayed)
            {
                LastPlayed = endTime;
            }
        }


    }
}
