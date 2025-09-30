using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupManager.Model
{
    public partial class ProgressModel : ObservableObject
    {
        [ObservableProperty]
        private double _percentage;

        [ObservableProperty]
        private string _statusText = string.Empty;

        #region Public methods

        /// <summary>
        /// Define um valor e status específico para a progress bar.
        /// </summary>
        public void Report(double percentage, string status)
        {
            Percentage = percentage;
            StatusText = status;
        }

        /// <summary>
        /// Zera o valor e o status da progress bar.
        /// </summary>
        public void Reset()
        {
            Percentage = 0;
            StatusText = string.Empty;
        }

        #endregion
    }
}
