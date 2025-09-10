using BackupManager.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BackupManager.View
{
    public partial class MainWindow : Window
    {
        private string? destinationFolder;
        private List<DatabaseModel> selectedDatabases = new List<DatabaseModel>();
        private ICollectionView? databasesView;

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

            txtDatabaseFilter.Focus();

            databasesView = CollectionViewSource.GetDefaultView(Databases);
            databasesView.Filter = FilterDatabases;
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

            dialog.ShowDialog();
            destinationFolder = dialog.FolderName;

            RefreshBackupButtonState();
            UpdateSelectButtonStyle();
        }

        private void txtDatabaseFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDatabaseFilter.Text))
                tbPlaceholder.Visibility = Visibility.Hidden;
            else
                tbPlaceholder.Visibility = Visibility.Visible;

            databasesView?.Refresh();
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

        private void UpdateSelectButtonStyle()
        {
            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                btnSelectDestinationFolder.Content = "Selecionar pasta de destino";
                btnSelectDestinationFolder.Foreground = Brushes.Tomato;
            }
            else
            {
                btnSelectDestinationFolder.Content = destinationFolder;
                btnSelectDestinationFolder.Foreground = Brushes.Green;
            }
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

        private bool FilterDatabases(object obj)
        {
            if (obj is not DatabaseModel database)
                return false;

            if (string.IsNullOrWhiteSpace(txtDatabaseFilter.Text))
                return true;

            return database.Name.Contains(txtDatabaseFilter.Text, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}