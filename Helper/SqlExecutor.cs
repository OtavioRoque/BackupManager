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
    /// Usar com o alias SQL.NomeMetodo().
    /// </remarks>
    public static class SqlExecutor
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
            string mensagem = $"Erro ao executar comando SQL: {comandoSql}. Detalhes: {exception.Message}";
            return new Exception(mensagem, exception);
        }
    }
}
