using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DespesasLibrary
{
    public class DbConnect
    {
        private string Server { get; set; }
        private string DatabaseName { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private MySqlConnection Connection { get; set; }

        /// <summary>
        ///     Set the default DB configuration
        /// </summary>
        private void Init()
        {
            const bool isLocal = false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (isLocal)
                // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
            {
                // ReSharper disable once HeuristicUnreachableCode
                Server = "localhost";
                DatabaseName = "despesas_isi";
                UserName = "root";
                Password = "";
            }
#pragma warning restore 162
            // ReSharper disable once RedundantIfElseBlock
            else
                // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
            {
                // ReSharper disable once HeuristicUnreachableCode
                Server = "despesas-isi-mysql.mysql.database.azure.com";
                DatabaseName = "despesas_isi";
                UserName = "despesasisimysql@despesas-isi-mysql";
                Password = "despes@s-1s1-BD";
            }
#pragma warning restore 162
        }

        /// <summary>
        ///     Close DB Connection
        /// </summary>
        public void Close()
        {
            Connection.Close();
        }

        /// <summary>
        ///     Check if the connection to the DB is open, if not it ill be opened
        /// </summary>
        /// <returns>TRUE: Connection is open</returns>
        public bool IsConnectionOpen()
        {
            Init();
            if (Connection != null) return true;
            string connstring = $"Server={Server}; database={DatabaseName}; UID={UserName}; password={Password}";
            Connection = new MySqlConnection(connstring);
            Connection.Open();
            return true;
        }

        /// <summary>
        ///     Test if the user exist
        /// </summary>
        /// <param name="hashUser">Hash of the User to be checked</param>
        /// <returns>
        ///     <para>TRUE: User exists</para>
        ///     <para>FALSE: User doest exist</para>
        /// </returns>
        public bool HasUser(string hashUser)
        {
            const string query = "SELECT emailSha FROM despesas_isi.utilizadores WHERE emailSha = @hashUser;";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@hashUser", hashUser)
            };
            MySqlDataReader reader = ExecSqlWithData(query, parameters);
            try
            {
                if (reader == null) return false;
                if (reader.HasRows)
                {
                    reader.Close();
                    return true;
                }

                reader.Close();
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Run a DB Operation by Type of Data (Expense or User)
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data that will be processed</param>
        /// <param name="id">ID that is used to identify what will be update. 0 used in case of Insert operation</param>
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        public bool RunOperation(int op, ApiData data, string id = "")
        {
            // If is an Expense operation
            return data.GetType() == typeof(Expense)
                ? RunOperationExpense(op, (Expense) data, id)
                : RunOperationUser(op, (User) data);
            // If is an User operation
        }

        /// <summary>
        ///     Execute SQL and return a bool
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Parameters to be added to the SQL query</param>
        /// <returns>
        ///     <para>TRUE: SQL executed successfully</para>
        ///     <para>FALSE: SQL not executed successfully</para>
        /// </returns>
        public bool ExecSqlWithStatus(string query, List<MySqlParameter> parameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                parameters.ForEach(param => { cmd.Parameters.Add(param); });
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Execute SQL and return a data
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Parameters to be added to the SQL query</param>
        /// <returns>MySqlDataReader that allows to read the data retrieved from DB</returns>
        public MySqlDataReader ExecSqlWithData(string query, List<MySqlParameter> parameters)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                parameters.ForEach(param => { cmd.Parameters.Add(param); });
                return cmd.ExecuteReader();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Run operation on Despesas BD table
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data to add to the SQL query</param>
        /// <param name="id">ID of the Expense to be updated (only used on update)</param>
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        private bool RunOperationExpense(int op, Expense data, string id = "")
        {
            /*
             Test if:
                Connection with DB is open,
                User Exist,
                Expense ID Exist,
                Data is filled;
            */

            if (IsConnectionOpen() && HasUser(data.HashUser) && data.IsRequestFilled())
            {
                string query;
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Different query's and parameters per OperationID
                switch (op)
                {
                    case 1: // Insert
                        query =
                            "INSERT INTO despesas_isi.despesas (nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id) VALUES (@nome, @desc, @dataHoraCriacao, @valEuro, @valUsd, @utilizador);";
                        parameters.Add(new MySqlParameter("?utilizador", data.HashUser));
                        break;
                    case 2: // Update
                        // Can this user update this Expense?
                        if (id != "" && data.CanUpdate(id, data.HashUser) && Expense.HasExpense(id))
                        {
                            query =
                                "UPDATE despesas_isi.despesas SET nome = @nome, descricao = @desc,dataHoraCriacao = @dataHoraCriacao, valEur = @valEuro, valUsd = @valUsd WHERE(id = @id);";
                            parameters.Add(new MySqlParameter("?id", id));
                        }
                        else
                        {
                            return false;
                        }

                        break;
                    default: return false;
                }

                // Generic parameters for all ops
                parameters.Add(new MySqlParameter("?nome", data.Nome));
                parameters.Add(new MySqlParameter("?desc", data.Descricao));
                parameters.Add(new MySqlParameter("?dataHoraCriacao", data.DataHoraCriacao));
                parameters.Add(new MySqlParameter("?valEuro", data.ValEur));
                parameters.Add(new MySqlParameter("?valUsd", data.ValUsd));

                // Execute DB Op
                return ExecSqlWithStatus(query, parameters);
            }

            Connection.Close();
            return false;
        }

        /// <summary>
        ///      Run operation on Utilizadores BD table
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data to add to the SQL query</param>
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        private bool RunOperationUser(int op, User data)
        {
            /*
             Test if:
                Connection with DB is open,
                User Exists
                Expense Exists
                Data is filled;
            */

            if (IsConnectionOpen() && data.IsRequestFilled())
            {
                string query;
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Different query's and parameters per OperationID
                switch (op)
                {
                    case 1: // Insert
                        query =
                            "INSERT INTO despesas_isi.utilizadores (emailSha, moedaPadrao) VALUES (@emailSha, @moedaPadrao);";
                        break;
                    case 2: // Update
                        // Can this User update this User (he can only update himself)?
                        if (data.CanUpdate("", data.EmailSha))
                        {
                            query =
                                "UPDATE despesas_isi.utilizadores SET moedaPadrao = @moedaPadrao WHERE (emailSha = @emailSha);";
                        }
                        else
                        {
                            return false;
                        }

                        break;
                    default: return false;
                }

                // Generic parameters for all ops
                parameters.Add(new MySqlParameter("?emailSha", data.EmailSha));
                parameters.Add(new MySqlParameter("?moedaPadrao", data.MoedaPadrao));

                // Execute DB Op
                return ExecSqlWithStatus(query, parameters);
            }

            Connection.Close();
            return false;
        }

        /// <summary>
        ///     Obtém o Id do último registo inserido por um determinado Utilizador numa tabela
        /// </summary>
        /// <param name="nomeTabela">Nome da tabela a obter o Id do último registo</param>
        /// <param name="hashUser">Hash do Utilizador</param>
        /// <returns>Último Id registado na tabela e associado ao respetivo Utilizador</returns>
        public int GetLastId(string nomeTabela, string hashUser)
        {
            string query = "SELECT MAX(id) FROM despesas_isi." + nomeTabela + " WHERE utilizador_id = @hashUser";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@hashUser", hashUser)
            };
            MySqlDataReader reader = ExecSqlWithData(query, parameters);
            try
            {
                if (reader == null) return 0;
                if (reader.HasRows && reader.Read())
                {
                    return reader.GetInt32(0);
                }

                reader.Close();
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }
    }
}