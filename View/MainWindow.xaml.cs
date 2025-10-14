using BackupManager.Helper;
using BackupManager.Model;
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
        private string _destinationFolder = string.Empty;
        private List<DatabaseModel> _selectedDatabases = new();
        private ICollectionView? _databasesView;

        public ObservableCollection<DatabaseModel> Databases { get; set; } = new();
        public ProgressModel BackupProgress { get; set; } = new();

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

            _databasesView = CollectionViewSource.GetDefaultView(Databases);
            _databasesView.Filter = FilterDatabases;
        }

        private void dgDatabases_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgDatabases.SelectedItem is not DatabaseModel database)
                return;

            SelectDatabase(database);
        }

        private void btnSelectDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            _destinationFolder = FolderService.SelectFolder("Selecione a pasta de destino do backup");

            RefreshBackupButtonState();
            UpdateSelectDestinationButtonStyle();
        }

        private void txtDatabaseFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDatabaseFilter.Text))
                tbPlaceholder.Visibility = Visibility.Hidden;
            else
                tbPlaceholder.Visibility = Visibility.Visible;

            _databasesView?.Refresh();
        }

        private async void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;

                var dbService = new DatabaseService(
                    _destinationFolder,
                    chkShrink.IsChecked == true,
                    chkCompact.IsChecked == true
                );

                await dbService.ProcessListDatabasesBackupAsync(_selectedDatabases, BackupProgress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Backup Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                BackupProgress.Reset();
                this.IsEnabled = true;
            }

            MessageBox.Show("Backup concluído.", "Backup Manager", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Private methods

        private void LoadDatabases()
        {
            string sql = "SELECT name FROM sys.databases";
            var dtDatabases = SQL.FillDataTable(sql);

            foreach (DataRow dr in dtDatabases.Rows)
            {
                string databaseName = dr["name"].ToString() ?? string.Empty;
                Databases.Add(new DatabaseModel(databaseName));
            }
        }

        private void RefreshBackupButtonState()
        {
            btnBackup.IsEnabled = !string.IsNullOrWhiteSpace(_destinationFolder) && _selectedDatabases.Count > 0;
        }

        private void UpdateSelectDestinationButtonStyle()
        {
            if (string.IsNullOrWhiteSpace(_destinationFolder))
            {
                btnSelectDestinationFolder.Content = "Selecionar pasta de destino";
                btnSelectDestinationFolder.Foreground = Brushes.Tomato;
            }
            else
            {
                btnSelectDestinationFolder.Content = _destinationFolder;
                btnSelectDestinationFolder.Foreground = Brushes.Green;
            }
        }

        private void SelectDatabase(DatabaseModel database)
        {
            database.IsChecked = !database.IsChecked;

            if (database.IsChecked)
                _selectedDatabases.Add(database);
            else
                _selectedDatabases.Remove(database);

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