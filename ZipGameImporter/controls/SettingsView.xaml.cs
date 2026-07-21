using Playnite.SDK.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZipGameImporter.ViewModels;

namespace ZipGameImporter
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void CategoryCheckedChanged(
            object sender,
            RoutedEventArgs e)
        {
            ImportOption vm =
                DataContext as ImportOption;

            if (vm != null)
            {
                vm.SelectedCategories = new ObservableCollection<Category>(
                    vm.Categories
                    .Where(x => x.IsSelected)
                    .Select(x => x.Category)
                    .ToList());

                vm.OnPropertyChanged("SelectedCategoriesText");
            }
        }
    }
}
