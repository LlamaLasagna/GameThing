using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class ConsolesViewModel : CommonBase
    {
        // PROPERTIES

        public List<GameRunSettings> ConsolesList { get; set; }

        private GameRunSettings _SelectedConsole;
        public GameRunSettings SelectedConsole
        {
            get { return _SelectedConsole; }
            set { SetProperty(ref _SelectedConsole, value); }
        }


        // CONSTRUCTORS

        public ConsolesViewModel()
        {
            ConsolesList = LibraryFiler.AllRunSettings;
        }


        // METHODS

        public void SetSelectedRunnerPath(string newRunnerFilePath)
        {
            SelectedConsole.GameRunnerPath = newRunnerFilePath;
            SaveGameRunSettings();
        }


        public void SaveGameRunSettings()
        {
            //Save settings to file
            LibraryFiler.SetRunSettings(ConsolesList);
        }


    }
}
