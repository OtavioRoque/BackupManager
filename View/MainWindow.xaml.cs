using System.Windows;
using System.Collections.ObjectModel;
using BackupManager.Model;
using System.Data;

namespace BackupManager.View
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<DatabaseModel> Databases { get; set; } = new ObservableCollection<DatabaseModel>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadDatabases();
        }

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

        private void dgDatabases_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgDatabases.SelectedItem is not DatabaseModel database)
                return;

            database.IsChecked = !database.IsChecked;
            dgDatabases.SelectedItem = null;
        }
    }
}