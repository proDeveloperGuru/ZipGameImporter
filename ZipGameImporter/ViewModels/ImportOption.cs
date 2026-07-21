using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ZipGameImporter.ViewModels
{
    public class ImportOption : ObservableObject
    {
        private readonly Extension plugin;

        private Guid? sourceId;
        private string incomingFolder;
        private string gamesFolder;
        private bool   hidden;

        public ICommand BrowseIncomingCommand { get; }

        public ICommand BrowseGamesCommand { get; }

        public ObservableCollection<GameSource> Sources { get; }

        public ObservableCollection<CategorySelection> Categories { get; }
        public ObservableCollection<Category> SelectedCategories { get; set; }
        public string SelectedCategoriesText { get { return string.Join(",", SelectedCategories.Select(c => c.Name)); } }

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

        public Guid? SourceId
        {
            get => sourceId;
            set
            {
                sourceId = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveCommand { get; }

        public ImportOption(Extension plugin, ObservableCollection<GameSource> sources, ObservableCollection<CategorySelection> categories, Action<ImportOption> removeAction)
        {
            this.plugin = plugin;

            IncomingFolder = @"D:\Incoming";
            GamesFolder = @"D:\Games";
            BrowseIncomingCommand = new RelayCommand(BrowseIncoming);
            BrowseGamesCommand = new RelayCommand(BrowseGames);
            Sources = sources;
            Categories = categories;
            RemoveCommand = new RelayCommand(() => removeAction(this));
        }

        private void BrowseIncoming()
        {
            var folder =
                plugin.PlayniteApi.Dialogs
                    .SelectFolder(
                        "Select ZIP folder");


            if (!string.IsNullOrEmpty(folder))
            {
                IncomingFolder = folder;
            }
        }



        private void BrowseGames()
        {
            var folder =
                plugin.PlayniteApi.Dialogs
                    .SelectFolder(
                        "Select games folder");


            if (!string.IsNullOrEmpty(folder))
            {
                GamesFolder = folder;
            }
        }
    }
}
