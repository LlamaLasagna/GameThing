using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class GameConsole
    {
        //PROPERTIES

        public int Id { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }
        public string ShortTitle { get; set; }
        public string Company { get; set; }
        public string Series { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Generation { get; set; }
        public List<string> FileTypes { get; set; }


        // METHODS

        public override string ToString()
        {
            return Title;
        }


    }
}
