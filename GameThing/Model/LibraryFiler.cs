using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public static class LibraryFiler
    {
        // CONSTANTS

        private const string ConsolesFilePath = "./Resource Data/consoles.json";
        private const string RunSettingsFilePath = "./Data/run_settings.json";
        private const string LibraryDataFilePath = "./Data/library.json";
        private const int SteamConsoleId = 22;


        // PROPERTIES

        private static List<GameConsole> _AllGameConsoles;
        public static List<GameConsole> AllGameConsoles
        {
            get
            {
                if (_AllGameConsoles == null)
                {
                    _AllGameConsoles = GetAllGameConsoles();
                }
                return _AllGameConsoles;
            }
        }

        private static List<LibraryCollection> _AllCollections;
        public static List<LibraryCollection> AllCollections
        {
            get
            {
                if (_AllCollections == null)
                {
                    _AllCollections = GetAllCollections();
                }
                return _AllCollections;
            }
        }

        private static List<GameRunSettings> _AllRunSettings;
        public static List<GameRunSettings> AllRunSettings
        {
            get
            {
                if (_AllRunSettings == null)
                {
                    _AllRunSettings = GetRunSettings();
                }
                return _AllRunSettings;
            }
        }

        private static List<LibraryItem> _FullLibraryList;
        public static List<LibraryItem> FullLibraryList
        {
            get
            {
                if (_FullLibraryList == null)
                {
                    _FullLibraryList = GetLibraryData();
                }
                return _FullLibraryList;
            }
        }


        // METHODS

        private static List<GameConsole> GetAllGameConsoles()
        {
            //TODO: Check ConsolesFilePath exists
            string allConsolesString = File.ReadAllText(ConsolesFilePath);
            List<GameConsole> allConsoles = Tools.DeserialiseData<List<GameConsole>>(allConsolesString);
            return allConsoles;
        }


        public static GameConsole GetConsoleById(int gameConsoleId)
        {
            GameConsole foundConsole = AllGameConsoles.Find(x => x.Id == gameConsoleId);
            return foundConsole;
        }


        private static List<LibraryCollection> GetAllCollections()
        {
            //TODO: Get collections from a file
            List<LibraryCollection> customCollections = new List<LibraryCollection>();
            List<LibraryCollection> generatedCollections = new List<LibraryCollection>();
            //Add recently played collection
            LibraryCollection recentCollection = new LibraryCollection("Recently Played");
            recentCollection.SortField = "LastPlayed";
            recentCollection.MinHours = 0.001;
            generatedCollections.Add(recentCollection);
            //Generate auto-collections for each console
            foreach (GameConsole console in AllGameConsoles)
            {
                generatedCollections.Add(new LibraryCollection(console));
            }
            //Return both collection lists merged
            generatedCollections.AddRange(customCollections);
            return generatedCollections;
        }


        private static List<LibraryItem> GetLibraryData()
        {
            //Check if Library Data file exists
            if (!File.Exists(LibraryDataFilePath))
            {
                return null;
            }
            //Load and deserialise saved library data
            string libraryDataString = File.ReadAllText(LibraryDataFilePath);
            List<LibraryItem> libraryData = Tools.DeserialiseData<List<LibraryItem>>(libraryDataString);
            //TODO: If deserialisation fails (eg library data is corrupt), make a backup copy of the corrupted file and return null
            //Process the deserialised data
            foreach (LibraryItem item in libraryData)
            {
                item.FileExtension = Path.GetExtension(item.FilePath).ToUpper();
                item.GameConsoleInfo = AllGameConsoles.Find(x => x.Id == item.GameConsoleId);
            }
            return libraryData;
        }


        public static void SaveLibraryData()
        {
            string libraryDataString = Tools.SerialiseData(FullLibraryList);
            string pathDirectory = Path.GetDirectoryName(LibraryDataFilePath);
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            File.WriteAllText(LibraryDataFilePath, libraryDataString);
        }


        private static string GetFileId(string filePath, long fileSize)
        {
            string fileName = Path.GetFileName(filePath);
            return fileName + fileSize;
        }


        public static void ScanDirectory(string fileDirectory = null)
        {
            //Default value if none was given
            if (fileDirectory == null) fileDirectory = Properties.Settings.Default.LibraryFileDirectory;
            //Handle non-existing directory
            if (!Directory.Exists(fileDirectory)) return;
            //Get all files in the directory
            string[] libraryFiles = Directory.GetFiles(fileDirectory, "*", SearchOption.AllDirectories);
            //TODO: Exclude files with extensions not included in the consoles list
            //Check scanned files against existing library data
            List<string> newLibraryFiles = new List<string>();
            if (FullLibraryList == null)
            {
                //No Existing library data. Create new list
                _FullLibraryList = new List<LibraryItem>();
                newLibraryFiles = libraryFiles.ToList();
            }
            else
            {
                //Get scanned files that aren't in the existing library data
                List<string> unmatchedFiles = libraryFiles.Where(x => FullLibraryList.Find(y => y.FilePath == x) == null).ToList();
                //Check for any "new" items that may actually be existing items that have been moved/renamed etc.
                FileInfo newLibFile;
                foreach (string filePath in unmatchedFiles)
                {
                    //TODO: Allow for renamed files, not just moved
                    newLibFile = new FileInfo(filePath);
                    LibraryItem renamedItem = FullLibraryList.Find(x => GetFileId(x.FilePath, x.FileSize) == GetFileId(filePath, newLibFile.Length));
                    if (renamedItem != null)
                    {
                        //Update the file path of the existing item
                        renamedItem.FilePath = filePath;
                    }
                    else
                    {
                        newLibraryFiles.Add(filePath);
                    }
                }
                //Hide library items where the files are missing/deleted
                List<LibraryItem> missingLibraryFiles = FullLibraryList.Where(x => !libraryFiles.Contains(x.FilePath) && x.FileExists).ToList();
                missingLibraryFiles.ForEach(x => x.FileExists = false);
            }
            //Convert new file paths into Library Items
            foreach (string filePath in newLibraryFiles)
            {
                LibraryItem libItem = GetLibraryItem(filePath);
                if (libItem != null)
                {
                    _FullLibraryList.Add(libItem);
                }
            }

            //Add any installed Steam games to the list
            ScanSteamDirectory();

            //Update the library data
            SaveLibraryData();
        }


        private static LibraryItem GetLibraryItem(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToUpper();
            //Get associated game console for file
            IEnumerable<GameConsole> fileTypeMatches = AllGameConsoles.Where(x => x.FileTypes.Contains(fileExtension.Replace(".", ""), StringComparer.OrdinalIgnoreCase));
            GameConsole matchedConsole;
            if (fileTypeMatches == null || fileTypeMatches.Count() <= 0)
            {
                //Unrecognised file type
                matchedConsole = null;
            }
            else if (fileTypeMatches.Count() == 1)
            {
                //Single file type match
                matchedConsole = fileTypeMatches.First();
            }
            else
            {
                //Multiple potential file type matches
                //TODO: Look at other files in this file's directory?
                matchedConsole = fileTypeMatches.First();
            }
            //Ignore unrecognised file type
            if (matchedConsole == null) { return null; }
            //Create Library Item for file
            FileInfo itemFile = new FileInfo(filePath);
            LibraryItem libItem = new LibraryItem()
            {
                FilePath = filePath,
                FileSize = itemFile.Length,
                Title = Path.GetFileNameWithoutExtension(filePath),
                FileExtension = fileExtension,
                GameConsoleId = matchedConsole?.Id,
                GameConsoleInfo = matchedConsole
            };
            return libItem;
        }


        private static void ScanSteamDirectory()
        {
            //Set all existing Library items for Steam apps to hidden
            //(This is so Steam apps that have been uninstalled won't show)
            List<LibraryItem> existingApps = FullLibraryList.Where(x => x.GameConsoleId == SteamConsoleId).ToList();
            existingApps.ForEach(x => x.IsVisible = false);

            string fileDirectory = Properties.Settings.Default.SteamAppsDirectory;
            if (string.IsNullOrEmpty(fileDirectory)) return;
            //Handle non-existing directory
            if (!Directory.Exists(fileDirectory)) return;
            //Get Steam app manifest files in the directory
            string[] manifestFiles = Directory.GetFiles(fileDirectory, "*.acf", SearchOption.TopDirectoryOnly);
            //Get the "console" object for Steam
            GameConsole steamConsole = AllGameConsoles.Find(x => x.Id == SteamConsoleId);
            //Convert app manifest data to Library data
            List<LibraryItem> steamApps = new List<LibraryItem>();
            foreach (string filePath in manifestFiles)
            {
                //TODO: Try catch for if manifest file is invalid?
                SteamManifestReader appManifest = new SteamManifestReader(filePath);
                string appId = appManifest.GetValue("appid").ToString();
                string appTitle = appManifest.GetValue("name").ToString();
                string appSizeStr = appManifest.GetValue("SizeOnDisk").ToString();
                long.TryParse(appSizeStr, out long appSize);
                //Exclude "Steamworks Common Redistributables"
                if (appId == "228980") continue;
                //Create Library Item for Steam app
                LibraryItem libItem = new LibraryItem()
                {
                    FilePath = $"steam://rungameid/{appId}",
                    FileSize = appSize,
                    Title = appTitle,
                    FileExtension = "steam",
                    GameConsoleId = SteamConsoleId,
                    GameConsoleInfo = steamConsole
                };
                steamApps.Add(libItem);
            }

            //Add Steam apps to Library list
            foreach (LibraryItem newItem in steamApps)
            {
                LibraryItem existingItem = FullLibraryList.Find(x => x.FilePath == newItem.FilePath);
                if (existingItem == null)
                {
                    //New Steam app, add to Library list
                    _FullLibraryList.Add(newItem);
                }
                else
                {
                    //This app is already in the Library, make it visible
                    existingItem.IsVisible = true;
                    existingItem.FileExists = true;
                }
            }
        }


        private static List<GameRunSettings> GetRunSettings()
        {
            List<GameRunSettings> allRunSettings = new List<GameRunSettings>();
            if (File.Exists(RunSettingsFilePath))
            {
                //Read saved game run settings from file
                string allRunSettingsJson = File.ReadAllText(RunSettingsFilePath);
                allRunSettings = Tools.DeserialiseData<List<GameRunSettings>>(allRunSettingsJson);
            }
            else
            {
                //Create game run settings
                foreach (GameConsole console in AllGameConsoles)
                {
                    allRunSettings.Add(new GameRunSettings(console));
                }
            }
            return allRunSettings;
        }


        public static void SetRunSettings(List<GameRunSettings> newRunSettings)
        {
            _AllRunSettings = newRunSettings;
            string allRunSettingsJson = Tools.SerialiseData(newRunSettings);
            string pathDirectory = Path.GetDirectoryName(RunSettingsFilePath);
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            File.WriteAllText(RunSettingsFilePath, allRunSettingsJson);
        }


    }
}
