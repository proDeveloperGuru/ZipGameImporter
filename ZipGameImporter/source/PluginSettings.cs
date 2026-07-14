using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;

namespace ZipGameImporter
{
    internal class PluginSettings : ObservableObject
    {
        private string incomingFolder;
        private string gamesFolder;
        private bool   hidden;
        //ToDo: For robustness it is better to store ID
        private GameSource source;
        private List<Category> categories;

        public string IncomingFolder
        {
            get => incomingFolder;
            set
            {
                incomingFolder = value;
                OnPropertyChanged();
            }
        }


        public string GamesFolder
        {
            get => gamesFolder;
            set
            {
                gamesFolder = value;
                OnPropertyChanged();
            }
        }

        public bool Hidden
        {
            get => hidden;
            set
            {
                hidden = value;
                OnPropertyChanged();
            }
        }

        public GameSource Source
        {
            get => source;
            set
            {
                source = value;
                OnPropertyChanged();
            }
        }

        public List<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCategoriesText
        {
            get
            {
                if (Categories == null)
                    return string.Empty;

                return string.Join(", ",
                    Categories
                        .Select(x => x.Name));
            }
        }

        public PluginSettings()
        {
            IncomingFolder = @"D:\Incoming";
            GamesFolder = @"D:\Games";
            Categories = new List<Category>();
        }
    }
}
