using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public class DatabaseModel : ObservableObject
    {
        private readonly string _name;
        public string Name => _name;

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public DatabaseModel(string databaseName)
        {
            _name = databaseName;
        }
    }
}
