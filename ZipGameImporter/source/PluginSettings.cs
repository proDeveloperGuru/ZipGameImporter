using System.Collections.Generic;

namespace ZipGameImporter
{
    internal class PluginSettings : ObservableObject
    {
        private string incomingFolder;
        private string gamesFolder;

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

        public PluginSettings()
        {
            IncomingFolder = @"D:\Incoming";
            GamesFolder = @"D:\Games";
        }
    }
}
