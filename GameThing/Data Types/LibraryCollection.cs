using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class LibraryCollection
    {
        // PROPERTIES

        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public bool IsGenerated { get; set; }
        public string SearchString { get; set; }
        public List<string> Tags { get; set; }
        public List<int> GameConsoleIds { get; set; }
        public int MinPlayers { get; set; }
        public double MinHours { get; set; }
        public string SortField { get; set; }


        // CONSTRUCTORS

        public LibraryCollection(GameConsole console)
        {
            Title = console.Title;
            IsVisible = true;
            IsGenerated = true;
            GameConsoleIds = new List<int>() { console.Id };
            MinPlayers = 0;
            MinHours = 0;
            SortField = "Title";
        }


        public LibraryCollection(string title)
        {
            Title = title;
            IsVisible = true;
            IsGenerated = false;
            MinPlayers = 1;
            MinHours = 0;
            SortField = "Title";
        }


        // METHODS

        public override string ToString()
        {
            return Title;
        }


    }
}
