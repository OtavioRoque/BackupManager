using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public partial class DatabaseModel : ObservableObject
    {
        private readonly string _name;
        public string Name => _name;

        [ObservableProperty]
        private bool _isChecked;

        public DatabaseModel(string databaseName)
        {
            _name = databaseName;
        }
    }
}
