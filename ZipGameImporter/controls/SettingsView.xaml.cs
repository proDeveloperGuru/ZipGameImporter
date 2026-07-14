using Playnite.SDK.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            PluginSettingsViewModel vm =
                DataContext as PluginSettingsViewModel;

            if (vm != null)
            {
                vm.Settings.Categories =
                    vm.Categories
                    .Where(x => x.IsSelected)
                    .Select(x => x.Category)
                    .ToList();

                vm.OnPropertyChanged("SelectedCategoriesText");
            }
        }

        private void CategoriesComboBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (combo != null)
            {
                combo.IsDropDownOpen = true;
            }
        }
    }
}
