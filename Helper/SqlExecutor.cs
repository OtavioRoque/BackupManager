using System.Data;
using BackupManager.Config;
using Microsoft.Data.SqlClient;

#pragma warning disable CS8601
#pragma warning disable CS8603

namespace BackupManager.Helper
{
    /// <summary>
    /// Contains utility methods for accessing the database, helping you avoid manually creating a SqlConnection.
    /// </summary>
    /// 
    /// <remarks>
    /// Use with the SQL.MethodName() alias.
    /// </remarks>
    public static class SqlExecutor
    {
        private static readonly string _connectionString = ConfigLoader.GetConnectionString();

        /// <summary>
        /// Reads a database table using an SQL query.
        /// </summary>
        /// 
        /// <param name="sql">
        /// The SQL query to be executed.
        /// </param>
        /// 
        /// <returns>
        /// A DataTable containing the query results.
        /// </returns>
        public static DataTable FillDataTable(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(sql, conn);
                using var adapter = new SqlDataAdapter(cmd);

                var tabela = new DataTable();
                adapter.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw ThrowSqlCommandException(sql, ex);
            }
        }

        /// <summary>
        /// Executes a SQL command such as INSERT, UPDATE, DELETE, or any statement that does not return a result set.
        /// </summary>
        /// 
        /// <param name="sql">
        /// The SQL command to execute.
        /// </param>
        /// 
        /// <returns>
        /// The number of rows affected by the command.
        /// </returns>
        public static int ExecuteNonQuery(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(sql, conn);

                cmd.CommandTimeout = 0;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ThrowSqlCommandException(sql, ex);
            }
        }

        private static Exception ThrowSqlCommandException(string comandoSql, Exception exception)
        {
            string message = $"Error executing SQL command: {comandoSql}. Details: {exception.Message}";
            return new Exception(message, exception);
        }
    }
}
