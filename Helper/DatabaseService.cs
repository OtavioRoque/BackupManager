using BackupManager.Model;
using System.IO;
using System.IO.Compression;

namespace BackupManager.Helper
{
    public class DatabaseService
    {
        private readonly string _destinationFolder;
        private readonly bool _shrinkDatabase;
        private readonly bool _backupDatabase;

        public DatabaseService(string destinationFolder, bool shrinkDatabase, bool backupDatabase)
        {
            _destinationFolder = destinationFolder;
            _shrinkDatabase = shrinkDatabase;
            _backupDatabase = backupDatabase;
        }
    }
}
