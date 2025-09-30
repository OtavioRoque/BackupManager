using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public partial class DatabaseModel : ObservableObject
    {
        public string Name { get; }

        [ObservableProperty]
        private bool _isChecked;

        public DatabaseModel(string databaseName)
        {
            Name = databaseName;
        }
    }
}
