using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    /// <summary>
    /// Represents the current progress state, including percentage and status text, and provides methods to update or reset these values.
    /// </summary>
    public partial class ProgressModel : ObservableObject
    {
        [ObservableProperty]
        private double _percentage;

        [ObservableProperty]
        private string _statusText = string.Empty;

        #region Public methods

        /// <summary>
        /// Sets a specific value and status for the progress bar.
        /// </summary>
        public void Report(double percentage, string status)
        {
            Percentage = percentage;
            StatusText = status;
        }

        /// <summary>
        /// Resets the progress bar value and status.
        /// </summary>
        public void Reset()
        {
            Percentage = 0;
            StatusText = string.Empty;
        }

        #endregion
    }
}
