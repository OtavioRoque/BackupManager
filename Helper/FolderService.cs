using Microsoft.Win32;
using System.IO;

namespace BackupManager.Helper
{
    public static class FolderService
    {
        public static string SelectFolder(string title)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = title
            };

            dialog.ShowDialog();

            return dialog.FolderName;
        }

        public static bool FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }
    }
}
