using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace BackupManager.Helper
{
    /// <summary>
    /// Contains methods for folder-related operations.
    /// </summary>
    public static class FolderService
    {
        /// <summary>
        /// Displays a dialog box for the user to select a folder.
        /// </summary>
        /// 
        /// <param name="title">
        /// The dialog title.
        /// </param>
        /// 
        /// <returns>
        /// The selected folder path, or null if the user cancels.
        /// </returns>
        public static string SelectFolder(string title)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = title
            };

            dialog.ShowDialog();

            return dialog.FolderName;
        }

        /// <summary>
        /// Checks whether a folder exists.
        /// </summary>
        /// 
        /// <param name="folderPath">
        /// The path of the folder to check.
        /// </param>
        /// 
        /// <returns>
        /// True if the folder exists; otherwise, false.
        /// </returns>
        public static bool FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        /// <summary>
        /// Opens a folder in Windows Explorer.
        /// </summary>
        /// 
        /// <param name="folderPath">
        /// The path of the folder to open.
        /// </param>
        public static void OpenFolder(string folderPath)
        {
            if (!FolderExists(folderPath))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
