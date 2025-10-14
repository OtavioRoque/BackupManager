using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace BackupManager.Helper
{
    /// <summary>
    /// Contém métodos para operações relacionadas a pastas.
    /// </summary>
    public static class FolderService
    {
        /// <summary>
        /// Mostra uma caixa de diálogo para o usuário selecionar uma pasta.
        /// </summary>
        /// <returns>
        /// O caminho da pasta selecionada ou null se o usuário cancelar.
        /// </returns>
        /// <param name="title">O título da caixa de diálogo.</param>
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
        /// Verifica se uma pasta existe.
        /// </summary>
        /// <returns>
        /// True se a pasta existir, false caso contrário.
        /// </returns>
        /// <param name="folderPath">O caminho da pasta a ser verificada.</param>
        public static bool FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        /// <summary>
        /// Abre uma pasta no Explorer do Windows.
        /// </summary>
        /// <param name="folderPath">Caminho da pasta que vai ser aberta.</param>
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
