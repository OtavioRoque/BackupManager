using BackupManager.Model;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace BackupManager.Helper
{
    public class DatabaseService
    {
        private readonly string _destinationFolder;
        private readonly bool _shrinkDatabase;
        private readonly bool _compactDatabase;

        public DatabaseService(string destinationFolder, bool shrinkDatabase, bool backupDatabase)
        {
            _destinationFolder = destinationFolder;
            _shrinkDatabase = shrinkDatabase;
            _compactDatabase = backupDatabase;
        }

        #region Public methods

        public async Task<bool> ProcessDatabaseBackupAsync(DatabaseModel database)
        {
            if (!DestinationFolderExists())
                return false;

            try
            {
                await Task.Run(() =>
                {
                    ShrinkDatabase(database);
                    BackupDatabase(database);
                    CompactDatabase(database);

                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private methods

        private void ShrinkDatabase(DatabaseModel database)
        {
            if (!_shrinkDatabase)
                return;

            string sql = $"DBCC SHRINKDATABASE ({database.Name})";
            DB.ExecuteNonQuery(sql);
        }

        private void BackupDatabase(DatabaseModel database)
        {
            string sql = @$"
                BACKUP DATABASE [{database.Name}] 
                TO DISK = N'{_destinationFolder}\{database.Name}.bak' 
                WITH FORMAT, INIT, NAME = N'Backup de {database.Name}'";

            DB.ExecuteNonQuery(sql);
        }

        private void CompactDatabase(DatabaseModel database)
        {
            if (!_compactDatabase)
                return;

            string backupFile = @$"{_destinationFolder}\{database.Name}.bak";
            string zipFile = @$"{_destinationFolder}\{database.Name}.zip";

            using (var zipToOpen = new FileStream(zipFile, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(backupFile, Path.GetFileName(backupFile), CompressionLevel.Optimal);
            }

            DeleteBackupFile(backupFile);
        }

        private bool DestinationFolderExists()
        {
            return Directory.Exists(_destinationFolder);
        }

        /// <summary>
        /// Apaga o .bak após a compactação pois não vai ser mais necessário.
        /// </summary>
        private void DeleteBackupFile(string backupFile)
        {
            if (File.Exists(backupFile))
                File.Delete(backupFile);
        }

        #endregion
    }
}
