using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public partial class ProgressModel : ObservableObject
    {
        [ObservableProperty]
        private double _percentage;

        [ObservableProperty]
        private string _statusText = string.Empty;
    }
}
