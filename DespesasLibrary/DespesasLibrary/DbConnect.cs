using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DespesasLibrary
{
    public class DbConnect
    {
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public MySqlConnection Connection { get; set; }

        /// <summary>
        ///     Set the default DB configuration
        /// </summary>
        public void Init() {
            Server = "localhost";
            DatabaseName = "despesas_isi";
            UserName = "root";
            Password = "";
        }

        /// <summary>
        ///     Close DB Connection
        /// </summary>
        public void Close() {
            Connection.Close();
        }

        /// <summary>
        ///     Check if the connection to the DB is open, if not it ill be opened
        /// </summary>
        /// <returns>
        ///     TRUE: Connection is open
        /// </returns>
        public bool IsConnectionOpen() {
            Init();
            if(Connection == null)
            {
                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
            }
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
        public bool HasUser(string hashUser) {
            string query = "SELECT emailSha FROM despesas_isi.utilizadores WHERE despesas_isi.utilizadores.emailSha = @hashUser;";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@hashUser", hashUser)
            };
            MySqlDataReader reader = ExecSQLWithData(query, parameters);
            try
            {
                if(reader != null)
                {
                    if(reader.HasRows)
                    {
                        reader.Close();
                        return true;
                    }
                    reader.Close();
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Run a DB Operation by OperationID
        ///         1 - Insert
        ///         2 - Update
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data that will be processed</param>
        /// <param name="id">ID that is used to identify what will be update. 0 used in case of Insert operation</param>
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        public bool RunOperation(int op, ApiData data, string id = "") {
            // If is an Expense operation
            if(data.GetType() == typeof(Expense))
            {
                return RunOperationExpense(op, (Expense)data, id);
            }
            // If is an User operation
            return runOperationUser(op, (User)data, id);
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
        public bool ExecSQLWithStatus(string query, List<MySqlParameter> parameters) {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                parameters.ForEach(paramn =>
                {
                    cmd.Parameters.Add(paramn);
                });
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                return true;
            }
            catch(Exception)
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
        public MySqlDataReader ExecSQLWithData(string query, List<MySqlParameter> parameters) {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                parameters.ForEach(paramn =>
                {
                    cmd.Parameters.Add(paramn);
                });
                return cmd.ExecuteReader();
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Run operation on Despesas BD table
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data to add to the SQL query</param>
        /// <param name="id">ID of the Expense to be updated (only used on update)</param>                   TODO: MUDAR PARA INT????
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        private bool RunOperationExpense(int op, Expense data, string id = "") {
            /**
            * Test if:
            *   Connection with DB is open,
            *   User Exist,
            *   Expense ID Exist,                                                                            TODO: NÃO ESTÁ A VERIFICAR ISTO
            *   Data is filled;
            * */
            if(IsConnectionOpen() && HasUser(data.hashUser) && data.IsRequestFilled())
            {
                string query;
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Different query's and parameters per OperationID
                switch(op)
                {
                    case 1: // Insert
                        query = "INSERT INTO despesas_isi.despesas (nome, descricao, valEur, valUsd, utilizador_id) " +
                                "VALUES (@nome, @desc, @valEuro, @valUsd, @utilizador);";
                        parameters.Add(new MySqlParameter("?utilizador", data.hashUser));
                        break;
                    case 2: // Update
                        // Can this user update this Expense?
                        if(id != "" && data.CanUpdate(id, data.hashUser) && data.HasExpense(id))
                        {
                            query = "UPDATE despesas_isi.despesas " +
                                    "SET nome = @nome, descricao = @desc, valEur = @valEuro, valUsd = @valUsd WHERE(id = @id);";
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
                parameters.Add(new MySqlParameter("?nome", data.nome));
                parameters.Add(new MySqlParameter("?desc", data.descricao));
                parameters.Add(new MySqlParameter("?dataHoraCriacao", data.dataHoraCriacao));
                parameters.Add(new MySqlParameter("?valEuro", data.valEuro));
                parameters.Add(new MySqlParameter("?valUsd", data.valUsd));

                // Execute DB Op
                return ExecSQLWithStatus(query, parameters);
            }
            Connection.Close();
            return false;
        }

        /// <summary>
        ///      Run operation on Utilizadores BD table
        /// </summary>
        /// <param name="op">Operation to execute</param>
        /// <param name="data">Data to add to the SQL query</param>
        /// <param name="id">ID of the User to be updated (only used on update)</param>
        /// <returns>
        ///     <para>TRUE: Operation executed successfully</para>
        ///     <para>FALSE: Operation not executed successfully</para>
        /// </returns>
        private bool runOperationUser(int op, User data, string id = "") {
            /**
            * Test if:
            *   Connection with DB is open,
            *   User Exists,                                                                     TODO: NÃO ESTÁ A VERIFICAR ISTO
            *   Expense Exists,                                                                  TODO: NÃO ESTÁ A VERIFICAR ISTO
            *   Data is filled;
            * */
            if(IsConnectionOpen() && data.IsRequestFilled())
            {
                string query;
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Different query's and parameters per OperationID
                switch(op)
                {
                    case 1: // Insert
                        query = "INSERT INTO despesas_isi.utilizadores (emailSha, moedaPadrao) VALUES (@emailSha, @moedaPadrao);";
                        break;
                    case 2: // Update
                        // Can this User update this User (he can only update himself)?
                        if(id != "" && data.CanUpdate("", id))
                        {
                            query = "UPDATE despesas_isi.utilizadores SET emailSha = @emailSha, moedaPadrao = @moedaPadrao WHERE (emailSha = @id);";
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
                parameters.Add(new MySqlParameter("?emailSha", data.emailSha));
                parameters.Add(new MySqlParameter("?moedaPadrao", data.moedaPadrao));

                // Execute DB Op
                return ExecSQLWithStatus(query, parameters);
            }
            Connection.Close();
            return false;
        }
    }
}