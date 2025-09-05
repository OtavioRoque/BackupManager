using BackupManager.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace BackupManager.View
{
    public partial class MainWindow : Window
    {
        private string? destinationFolder;
        private List<DatabaseModel> selectedDatabases = new List<DatabaseModel>();

        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDatabases();
            RefreshBackupButtonState();
        }

        private void dgDatabases_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgDatabases.SelectedItem is not DatabaseModel database)
                return;

            SelectDatabase(database);
        }

        private void btnSelectDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = "Selecione uma pasta para o backup.",
                Multiselect = false
            };

            if (dialog.ShowDialog() == false)
                return;

            destinationFolder = dialog.FolderName;
            RefreshBackupButtonState();
        }

        private void txtDatabaseFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDatabaseFilter.Text))
                tbPlaceholder.Visibility = Visibility.Hidden;
            else
                tbPlaceholder.Visibility = Visibility.Visible;
        }

        #endregion

        #region Private methods

        private void LoadDatabases()
        {
            string sql = "SELECT name FROM sys.databases";
            var dtDatabases = DB.FillDataTable(sql);

            foreach (DataRow dr in dtDatabases.Rows)
            {
                string databaseName = dr["name"].ToString() ?? string.Empty;
                Databases.Add(new DatabaseModel(databaseName));
            }
        }

        private void RefreshBackupButtonState()
        {
            btnBackup.IsEnabled = !string.IsNullOrWhiteSpace(destinationFolder) && selectedDatabases.Count > 0;
        }

        private void SelectDatabase(DatabaseModel database)
        {
            database.IsChecked = !database.IsChecked;

            if (database.IsChecked)
                selectedDatabases.Add(database);
            else
                selectedDatabases.Remove(database);

            dgDatabases.SelectedItem = null;
            RefreshBackupButtonState();
        }

        #endregion
    }
}