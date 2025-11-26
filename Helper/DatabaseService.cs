using BackupManager.Model;
using System.IO;
using System.IO.Compression;

namespace BackupManager.Helper
{
    /// <summary>
    /// Provides methods to perform SHRINK, BACKUP, and COMPRESSION operations on SQL Server databases.
    /// </summary>
    public class DatabaseService
    {
        private readonly string _destinationFolder;
        private readonly bool _shrinkDatabase;
        private readonly bool _compactDatabase;

        public DatabaseService(string destinationFolder, bool shrinkDatabase, bool compactDatabase)
        {
            _destinationFolder = destinationFolder;
            _shrinkDatabase = shrinkDatabase;
            _compactDatabase = compactDatabase;
        }

        #region Public methods

        /// <summary>
        /// Asynchronously executes the backup process for a list of databases.
        /// </summary>
        /// 
        /// <remarks>
        /// This method should be invoked inside a try/catch block.
        /// </remarks>
        /// 
        /// <param name="databases">
        /// The list of databases to be processed.
        /// </param>
        /// 
        /// <param name="progress">
        /// The progress reporter instance.
        /// </param>
        public async Task ProcessListDatabasesBackupAsync(IEnumerable<DatabaseModel> databases, ProgressModel progress)
        {
            ValidateDestinationFolder();

            foreach (var database in databases)
                await ProcessDatabaseBackupAsync(database, progress);
        }

        #endregion

        #region Private methods

        private async Task ProcessDatabaseBackupAsync(DatabaseModel database, ProgressModel progress)
        {
            await Task.Run(() =>
            {
                ShrinkDatabase(database, progress);
                BackupDatabase(database, progress);
                CompactDatabase(database, progress);

            });
        }

        private void ShrinkDatabase(DatabaseModel database, ProgressModel progress)
        {
            if (!_shrinkDatabase)
                return;

            progress.Report(30, $"Shrinking {database.Name}...");

            string sql = $"DBCC SHRINKDATABASE ({database.Name})";
            SQL.ExecuteNonQuery(sql);
        }

        private void BackupDatabase(DatabaseModel database, ProgressModel progress)
        {
            progress.Report(70, $"Backing up {database.Name}...");

            string sql = @$"
                BACKUP DATABASE [{database.Name}] 
                TO DISK = N'{_destinationFolder}\{database.Name}.bak' 
                WITH FORMAT, INIT, NAME = N'Backup de {database.Name}'";

            SQL.ExecuteNonQuery(sql);
        }

        private void CompactDatabase(DatabaseModel database, ProgressModel progress)
        {
            if (!_compactDatabase)
                return;

            progress.Report(100, $"Compacting {database.Name}...");

            string backupFile = @$"{_destinationFolder}\{database.Name}.bak";
            string zipFile = @$"{_destinationFolder}\{database.Name}.zip";

            using (var zipToOpen = new FileStream(zipFile, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(backupFile, Path.GetFileName(backupFile), CompressionLevel.Optimal);
            }

            File.Delete(backupFile);
        }

        private void ValidateDestinationFolder()
        {
            if (!FolderService.FolderExists(_destinationFolder))
            {
                string message = $"The destination folder:\n{_destinationFolder}\ndoes not exist or could not be found.";
                throw new DirectoryNotFoundException(message);
            }
        }

        #endregion
    }
}
