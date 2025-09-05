using System.Data;
using SqlHelper.Config;
using Microsoft.Data.SqlClient;

#pragma warning disable CS8601
#pragma warning disable CS8603

namespace SqlHelper.Utils
{
    /// <summary>
    /// Contém métodos utilitários para acessar o banco de dados.
    /// São úteis pra não ter que abrir uma SqlConnection manualmente.
    /// </summary>
    /// <remarks>
    /// Usar com o alias DB.NomeMetodo().
    /// </remarks>
    public static class DatabaseAccess
    {
        private static readonly string _connectionString = ConfigLoader.ObterConnectionString("ConexaoLocal");

        /// <summary>
        /// Ler uma tabela do banco de dados usando uma consulta SQL.
        /// </summary>
        /// <remarks>
        /// Usar quando precisar fazer Bindings dos dados lidos.
        /// </remarks>
        /// <returns>
        /// Um DataTable contendo os resultados da consulta SQL.
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
        /// Ler uma tabela do banco de dados usando uma consulta SQL.
        /// </summary>
        /// <remarks>
        /// Usar quando não precisar fazer Bindings dos dados lidos, é mais performático que FillDataTable.
        /// </remarks>
        public static void ExecuteReader(string sql, Action<SqlDataReader> processador)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                processador(reader);
            }
        }

        /// <summary>
        /// Buscar um único valor do banco de dados usando uma consulta SQL.
        /// </summary>
        /// <returns>
        /// Uma string contendo o resultado da consulta SQL.
        /// </returns>
        public static string ExecuteScalar(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(sql, conn);

                conn.Open();
                return cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ThrowSqlCommandException(sql, ex);
            }
        }

        /// <summary>
        /// Executar comandos SQL de INSERT, DELETE, UPDATE, etc.
        /// </summary>
        /// <returns>
        /// 1 se o comando afetou linhas, 0 se não afetou nenhuma linha.
        /// </returns>
        public static int ExecuteNonQuery(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(sql, conn);

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
            string mensagem = $"Erro ao executar comando SQL: {comandoSql}. Detalhes: {exception.Message}";
            return new Exception(mensagem, exception);
        }
    }
}
