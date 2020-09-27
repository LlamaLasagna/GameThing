using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameThing
{
    public class MediaItem
    {
        // PROPERTIES

        public TagLib.File TagFile;
        public TagLib.Tag TagInfo
        {
            get
            {
                if (TagFile == null) return null;
                return TagFile.Tag;
            }
        }
        public TagLib.IPicture TagMainImage;

        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public BitmapImage ImageData { get; set; }


        // CONSTRUCTORS

        public MediaItem(string filePath)
        {
            FilePath = filePath;

            try
            {
                TagFile = TagLib.File.Create(filePath);
                Title = TagInfo.Title;
                Album = TagInfo.Album;
                SetImage();
            }
            catch (Exception)
            {
                Title = "";
            }
            //Set defaults if something went wrong
            if (string.IsNullOrWhiteSpace(Title))
            {
                Title = Path.GetFileNameWithoutExtension(FilePath);
                Title = Tools.TitleCase(Title);
            }
        }


        // METHODS

        private void SetImage()
        {
            if (TagInfo == null || TagInfo.Pictures == null || TagInfo.Pictures.Count() <= 0) return;
            //Find the front cover image, or just use the first if there is none
            TagLib.IPicture mainImage = TagInfo.Pictures.First(x => x.Type == TagLib.PictureType.FrontCover);
            if (mainImage == null)
            {
                mainImage = TagInfo.Pictures.First();
            }
            TagMainImage = mainImage;
            //Convert the TagLib picture to a normal image type
            try
            {
                BitmapImage img = new BitmapImage();
                using (MemoryStream mem = new MemoryStream(TagMainImage.Data.Data))
                {
                    img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = mem;
                    img.EndInit();
                }
                img.Freeze();
                ImageData = img;
            }
            catch (Exception)
            {
                ImageData = null;
            }
        }


    }
}
