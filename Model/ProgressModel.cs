using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public partial class ProgressModel : ObservableObject
    {
        [ObservableProperty]
        private double _percentage;

        [ObservableProperty]
        private string _statusText = string.Empty;

        public void Report(double percentage, string status)
        {
            Percentage = percentage;
            StatusText = status;
        }

        public void Reset()
        {
            Percentage = 0;
            StatusText = string.Empty;
        }
    }
}
