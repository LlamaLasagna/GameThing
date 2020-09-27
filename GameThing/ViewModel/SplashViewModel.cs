using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class SplashViewModel : CommonBase
    {
        // PROPERTIES

        public Uri VideoSource { get; set; }


        // CONSTRUCTORS

        public SplashViewModel()
        {
            PickVideo();
        }


        // METHODS

        public void PickVideo()
        {
            string fileDirectory = Properties.Settings.Default.SplashFileDirectory;
            if (string.IsNullOrWhiteSpace(fileDirectory) || !Directory.Exists(fileDirectory)) return;
            //TODO: Only media files
            string[] splashVideos = Directory.GetFiles(fileDirectory, "*", SearchOption.AllDirectories);
            Random rng = new Random(); //Seeds based on system clock
            splashVideos = splashVideos.OrderBy(x => rng.Next()).ToArray();
            VideoSource = new Uri(splashVideos.First());
        }


    }
}
