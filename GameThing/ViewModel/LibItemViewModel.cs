using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public class LibItemViewModel : CommonBase
    {
        // PROPERTIES

        private LibraryItem _SelectedItem;
        public LibraryItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                SetProperty(ref _SelectedItem, value);
                if (value.Tags != null && value.Tags.Count > 0)
                {
                    TagsEdit = string.Join(", ", value.Tags);
                    TagsEdit += ", ";
                }
            }
        }


        public List<GameConsole> AllGameConsoles
        {
            get { return LibraryFiler.AllGameConsoles; }
        }


        public bool IsHidden
        {
            get
            {
                if (SelectedItem == null) { return true; }
                return !SelectedItem.IsVisible;
            }
            set
            {
                SelectedItem.IsVisible = !value;
                RaisePropertyChanged("IsHidden");
            }
        }


        private string _TagsEdit;
        public string TagsEdit
        {
            get { return _TagsEdit; }
            set { SetProperty(ref _TagsEdit, value); }
        }


        // METHODS

        private List<string> GetInputTags()
        {
            if (string.IsNullOrWhiteSpace(TagsEdit)) { return null; }

            List<string> tagsList = TagsEdit.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> newTags = new List<string>();
            foreach (string tag in tagsList)
            {
                string tagFormatted = tag.Trim();
                tagFormatted = Tools.TitleCase(tagFormatted);
                //Add tag if it's not empty or a duplicate
                if (!string.IsNullOrWhiteSpace(tagFormatted) && !newTags.Contains(tagFormatted, StringComparer.OrdinalIgnoreCase))
                {
                    newTags.Add(tagFormatted);
                }
            }
            return newTags;
        }


        public void UpdateLibraryFile()
        {
            SelectedItem.Tags = GetInputTags();
            LibraryFiler.SaveLibraryData();
        }


        public void RaiseItemChanged()
        {
            RaisePropertyChanged("SelectedItem");
        }


        public void SubmitCurrentTag()
        {
            if (string.IsNullOrWhiteSpace(TagsEdit)) { return; }
            //Prevent blank tags being added
            int lastTagIndex = TagsEdit.LastIndexOf(',');
            if (lastTagIndex < 0) { lastTagIndex = 0; }
            else { lastTagIndex += 1; }
            string latestTag = TagsEdit.Substring(lastTagIndex).Trim();
            if (latestTag.Trim() == "") { return; }
            //Trim and format the tags
            List<string> newTags = GetInputTags();
            TagsEdit = string.Join(", ", newTags);
            //Add a comma at the end if there's not already one
            if (TagsEdit.Substring(TagsEdit.Length - 1, 1) != ",")
            {
                TagsEdit = TagsEdit + ", ";
            }
        }


    }
}
