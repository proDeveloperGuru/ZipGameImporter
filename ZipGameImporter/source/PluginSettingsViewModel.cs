using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace ZipGameImporter
{
    internal class PluginSettingsViewModel : ObservableObject, ISettings
    {
        private readonly Extension plugin;

        private PluginSettings settings;


        public PluginSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }


        public ICommand BrowseIncomingCommand { get; }

        public ICommand BrowseGamesCommand { get; }


        public PluginSettingsViewModel(
            Extension plugin,
            PluginSettings settings)
        {
            this.plugin = plugin;

            Settings = settings;


            BrowseIncomingCommand =
                new RelayCommand(
                    BrowseIncoming);


            BrowseGamesCommand =
                new RelayCommand(
                    BrowseGames);
        }



        private void BrowseIncoming()
        {
            var folder =
                plugin.PlayniteApi.Dialogs
                    .SelectFolder(
                        "Select ZIP folder");


            if (!string.IsNullOrEmpty(folder))
            {
                Settings.IncomingFolder = folder;
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
                Settings.GamesFolder = folder;
            }
        }



        public void BeginEdit()
        {
        }


        public void CancelEdit()
        {
        }


        public void EndEdit()
        {
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Settings.IncomingFolder))
            {
                errors.Add("ZIP source folder is not selected.");
            }
            else if (!Directory.Exists(Settings.IncomingFolder))
            {
                errors.Add("ZIP source folder does not exist.");
            }


            if (string.IsNullOrWhiteSpace(Settings.GamesFolder))
            {
                errors.Add("Games installation folder is not selected.");
            }
            else
            {
                try
                {
                    if (!Directory.Exists(Settings.GamesFolder))
                    {
                        Directory.CreateDirectory(Settings.GamesFolder);
                    }

                    string testFile =
                        Path.Combine(
                            Settings.GamesFolder,
                            ".zipimporter_test");

                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                catch (Exception ex)
                {
                    errors.Add(
                        $"Games folder is not writable: {ex.Message}");
                }
            }


            return errors.Count == 0;
        }
    }
}
