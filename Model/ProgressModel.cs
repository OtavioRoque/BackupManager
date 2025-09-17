using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public class ProgressModel : ObservableObject
    {
        private string? _statusText;
        public string StatusText
        {
            get => _statusText ?? string.Empty;
            set => SetProperty(ref _statusText, value);
        }

        private double _percentage;
        public double Percentage
        {
            get => _percentage;
            set => SetProperty(ref _percentage, value);
        }
    }
}
