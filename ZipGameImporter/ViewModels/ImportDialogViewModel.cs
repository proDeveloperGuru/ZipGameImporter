using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ZipGameImporter.ViewModels
{
    public class ImportDialogViewModel
    {
        private readonly Extension plugin;
        private IPlayniteAPI api;

        public ObservableCollection<ImportOption> Settings { get; }

        public ObservableCollection<GameSource> Sources { get; }

        public ObservableCollection<Category> Categories { get; }

        public ICommand AddImportCommand => new RelayCommand(() =>
        {
            var categories = new ObservableCollection<CategorySelection>(Categories.Select(x => new CategorySelection
            {
                Category = x,
                IsSelected = false
            }));

            Settings.Add(new ImportOption(this.plugin,Sources,categories,RemoveItem));
        });

        private readonly Action closeAction;
        public ICommand CancelCommand => new RelayCommand(() =>
        {
            closeAction?.Invoke();
        });

        public ICommand ImportCommand => new RelayCommand(() =>
        {
            ImportGames();
        });

        private void RemoveItem(ImportOption item)
        {
            Settings.Remove(item);
        }

        public ImportDialogViewModel(Extension plugin, IPlayniteAPI api, Action closeAction)
        {
            this.plugin = plugin;
            this.api = api;
            this.closeAction = closeAction;

            Categories = new ObservableCollection<Category>(plugin.PlayniteApi.Database.Categories);
            Sources = new ObservableCollection<GameSource>(plugin.PlayniteApi.Database.Sources);

            var categories = new ObservableCollection<CategorySelection>(Categories.Select(x => new CategorySelection
            {
                Category = x,
                IsSelected = false
            }));

            Settings = new ObservableCollection<ImportOption>()
            {
                new ImportOption(this.plugin,Sources,categories,RemoveItem),
            };
        }

        private void ImportGames()
        {
            Task.Run(() =>
            {
                try
                {
                    var logger = new Logger();

                    foreach (var option in Settings)
                    {
                        var updater =
                            new PlayniteUpdater(
                                api, logger);

                        var importer =
                            new Importer(
                                updater,
                                action =>
                                {
                                    Application.Current.Dispatcher.Invoke(action);
                                }, logger);

                        importer.ImportFolder(option);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        api.Dialogs.ShowMessage(
                            "ZIP import completed.",
                            "ZIP Game Importer");
                    });

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        api.Dialogs.ShowErrorMessage(
                            ex.Message,
                            "ZIP Game Importer");
                    });
                }
            });

        }
    }
}
