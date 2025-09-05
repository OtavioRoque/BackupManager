using System.Windows;
using System.Windows.Controls;

#pragma warning disable CS8618

namespace BackupManager.Views.UserControls
{
    public partial class ClearableTextBox : UserControl
    {
        public ClearableTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string _placeHolder;

        public string Placeholder
        {
            get { return _placeHolder; }
            set { _placeHolder = value; }
        }

        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        public event TextChangedEventHandler TextChanged;

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;

            TextChanged?.Invoke(this, e);
        }
    }
}
