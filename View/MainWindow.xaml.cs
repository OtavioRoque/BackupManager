using System.Windows;
using System.Collections.ObjectModel;
using BackupManager.Model;
using System.Data;
using Microsoft.Win32;

namespace BackupManager.View
{
    public partial class MainWindow : Window
    {
        private string? destinationFolder;

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

            database.IsChecked = !database.IsChecked;
            dgDatabases.SelectedItem = null;
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
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
            btnBackup.IsEnabled = !string.IsNullOrWhiteSpace(destinationFolder);
        }

        #endregion
    }
}