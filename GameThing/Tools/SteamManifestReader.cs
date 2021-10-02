using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class SteamManifestReader
    {
        // PROPERTIES

        private Dictionary<string, object> ManifestData;


        // CONSTRUCTORS

        public SteamManifestReader(string filePath)
        {
            //Read data lines from file
            string[] manifestLines = File.ReadAllLines(filePath);
            //Initialise data object
            ManifestData = new Dictionary<string, object>();
            //Parse lines
            //NOTE: This is quick and dirty since I only need a few base properties.
            //May need some sort of recursive function if all data was needed
            foreach (string line in manifestLines)
            {
                string[] lineParts = line.Split(new string[] { "\t\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lineParts.Count() >= 2)
                {
                    char[] trimChars = new char[] { '\t', '"' };
                    string lineKey = lineParts[0].Trim(trimChars);
                    string lineValue = lineParts[1].Trim(trimChars);
                    if (!ManifestData.ContainsKey(lineKey))
                    {
                        ManifestData.Add(lineKey, lineValue);
                    }
                }
            }
        }


        // METHODS

        public object GetValue(string propertyName)
        {
            if (ManifestData == null || !ManifestData.ContainsKey(propertyName)) return null;
            return ManifestData[propertyName];
        }


    }
}
