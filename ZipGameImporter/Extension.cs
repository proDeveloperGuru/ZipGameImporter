using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using ZipGameImporter.Controls;
using ZipGameImporter.ViewModels;

namespace ZipGameImporter
{
    public class Extension : GenericPlugin
    {
        private ImportOption Settings;
        private const string MenuName = "Import ZIP Games";
        private IPlayniteAPI api;

        public override Guid Id =>
            Guid.Parse("7f3e3e6d-4f5a-4f1b-a8d5-8c8c4a2e1abc");


        public Extension(IPlayniteAPI api)
            : base(api)
        {
            this.api = api;
            Properties = new GenericPluginProperties
            {
                HasSettings = false,
            };
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(
            GetMainMenuItemsArgs args)
        {
            yield return new MainMenuItem
            {
                Description = MenuName,
                MenuSection = "Tools",
                Action = OpenImportDialog
            };
        }

        private void OpenImportDialog(MainMenuItemActionArgs args)
        {
            var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMinimizeButton = false,
                ShowMaximizeButton = false
            });

            var viewModel = new ImportDialogViewModel(this, api ,window.Close);

            window.Title = "Import Games";
            window.Content = new ImportDialog
            {
                DataContext = viewModel,
            };

            window.ShowDialog();
        }

        
    }
}
