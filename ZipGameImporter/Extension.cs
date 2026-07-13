using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;


namespace ZipGameImporter
{
    public class Extension : GenericPlugin
    {
        private PluginSettings Settings;
        private PluginSettingsViewModel settingsViewModel;
        private const string MenuName = "Import ZIP Games";


        public override Guid Id =>
            Guid.Parse("7f3e3e6d-4f5a-4f1b-a8d5-8c8c4a2e1abc");


        public Extension(IPlayniteAPI api)
            : base(api)
        {
            Settings = LoadPluginSettings<PluginSettings>();
            if (Settings == null)
            {
                Settings = new PluginSettings();
            }

            settingsViewModel =
                new PluginSettingsViewModel(
                    this,
                    Settings);

            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(
            GetMainMenuItemsArgs args)
        {
            yield return new MainMenuItem
            {
                Description = MenuName,
                MenuSection = "Tools",
                Action = ImportGames
            };
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SettingsView();
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settingsViewModel;
        }

        private void ImportGames(MainMenuItemActionArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    var updater =
                        new PlayniteUpdater(
                            PlayniteApi);


                    var importer =
                        new Importer(
                            updater,
                            action =>
                            {
                                Application.Current.Dispatcher.Invoke(action);
                            });


                    importer.ImportFolder(
                        Settings.IncomingFolder,
                    Settings.GamesFolder);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PlayniteApi.Dialogs.ShowMessage(
                            "ZIP import completed.",
                            "ZIP Game Importer");
                    });

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PlayniteApi.Dialogs.ShowErrorMessage(
                            ex.Message,
                            "ZIP Game Importer");
                    });
                }
            });
            
        }
    }
}
