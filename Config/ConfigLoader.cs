using System.Configuration;
using System.IO;

namespace BackupManager.Config
{
    public static class ConfigLoader
    {
        public static string GetConnectionString()
        {
            string localConfigPath = "Config/App.Local.config";
            string connectionName = "DefaultConnection";

            if (!File.Exists(localConfigPath))
            {
                string msg = $"O arquivo de configuração local '{localConfigPath}' não foi encontrado." +
                    Environment.NewLine + "Leia o README.md da pasta Config para mais informações.";

                throw new FileNotFoundException(msg);
            }

            var map = new ExeConfigurationFileMap { ExeConfigFilename = localConfigPath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var connectionString = config.ConnectionStrings.ConnectionStrings[connectionName];

            if (connectionString == null)
            {
                string msg = $"A connection string '{connectionName}' não foi encontrada no arquivo '{localConfigPath}'." +
                    Environment.NewLine + $"Certifique-se que App.Local.config tenha o name definido como '{connectionName}'.";

                throw new ConfigurationErrorsException(msg);
            }

            return connectionString.ConnectionString;
        }
    }
}
