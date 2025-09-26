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

        /// <summary>
        /// Executa de forma assíncrona o processo de backup para uma lista de bancos de dados.
        /// </summary>
        /// <remarks>
        /// Chamar esse método dentro de um try/catch.
        /// </remarks>
        /// <param name="databases">A lista de bancos de dados a serem processados.</param>
        /// <param name="progress">O objeto para reportar o andamento.</param>
        public async Task ProcessListDatabasesBackupAsync(List<DatabaseModel> databases, ProgressModel progress)
        {
            ValidadeDestinationFolder();

            foreach (var database in databases)
                await ProcessDatabaseBackupAsync(database, progress);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Executa de forma assíncrona o processo de backup do banco de dados, fazendo shrink e compactação se configurado.
        /// </summary>
        /// <param name="database">O banco de dados a ser processado.</param>
        /// <param name="progress">O objeto para reportar o andamento.</param>
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

            progress.ReportProgress(30, $"Shrinking {database.Name}...");

            string sql = $"DBCC SHRINKDATABASE ({database.Name})";
            DB.ExecuteNonQuery(sql);
        }

        private void BackupDatabase(DatabaseModel database, ProgressModel progress)
        {
            progress.ReportProgress(70, $"Backing up {database.Name}...");

            string sql = @$"
                BACKUP DATABASE [{database.Name}] 
                TO DISK = N'{_destinationFolder}\{database.Name}.bak' 
                WITH FORMAT, INIT, NAME = N'Backup de {database.Name}'";

            DB.ExecuteNonQuery(sql);
        }

        private void CompactDatabase(DatabaseModel database, ProgressModel progress)
        {
            if (!_compactDatabase)
                return;

            progress.ReportProgress(100, $"Compacting {database.Name}...");

            string backupFile = @$"{_destinationFolder}\{database.Name}.bak";
            string zipFile = @$"{_destinationFolder}\{database.Name}.zip";

            using (var zipToOpen = new FileStream(zipFile, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(backupFile, Path.GetFileName(backupFile), CompressionLevel.Optimal);
            }

            DeleteBackupFile(backupFile);
        }

        /// <summary>
        /// Apaga o .bak após a compactação pois não vai ser mais necessário.
        /// </summary>
        private void DeleteBackupFile(string backupFile)
        {
            if (File.Exists(backupFile))
                File.Delete(backupFile);
        }

        private void ValidadeDestinationFolder()
        {
            if (!FolderService.FolderExists(_destinationFolder))
            {
                string message = $"A pasta destino:\n{_destinationFolder}\nnão existe ou não foi encontrada.";
                throw new DirectoryNotFoundException(message);
            }
        }

        #endregion
    }
}
