using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class GameRunSettings : CommonBase
    {
        // PROPERTIES

        public int GameConsoleId;

        private string _GameRunnerPath;
        public string GameRunnerPath
        {
            get { return _GameRunnerPath; }
            set { SetProperty(ref _GameRunnerPath, value); }
        }

        private string _GameRunnerArgs;
        public string GameRunnerArgs
        {
            get { return _GameRunnerArgs; }
            set { SetProperty(ref _GameRunnerArgs, value); }
        }

        [JsonIgnore]
        private GameConsole _GameConsoleInfo;
        [JsonIgnore]
        public GameConsole GameConsoleInfo
        {
            get
            {
                if (_GameConsoleInfo == null)
                {
                    _GameConsoleInfo = LibraryFiler.GetConsoleById(GameConsoleId);
                }
                return _GameConsoleInfo;
            }
            private set { _GameConsoleInfo = value; }
        }
        [JsonIgnore]
        public string GameConsoleTitle
        {
            get
            {
                if (GameConsoleInfo == null) return "[Invalid Console ID]";
                return GameConsoleInfo.Title;
            }
        }


        // CONSTRUCTORS

        public GameRunSettings(GameConsole consoleInfo)
        {
            GameConsoleInfo = consoleInfo;
            GameConsoleId = consoleInfo.Id;
            GameRunnerPath = null;
        }


        public GameRunSettings()
        {
            //This constructor used for JSON Deserialisation
        }


        // METHODS

        public override string ToString()
        {
            return GameConsoleTitle;
        }


    }
}
