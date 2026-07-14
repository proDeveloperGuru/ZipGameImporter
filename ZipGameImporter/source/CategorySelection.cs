using Playnite.SDK.Models;
using System.Collections.Generic;

namespace ZipGameImporter
{
    public class CategorySelection : ObservableObject
    {
        public Category Category { get; set; }

        public string Name
        {
            get { return Category.Name; }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
