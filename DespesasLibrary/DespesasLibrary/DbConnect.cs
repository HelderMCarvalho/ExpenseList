using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class DbConnect
{

    public string Server { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public MySqlConnection Connection { get; set; }

    /// <summary>
    /// Check if connection to DB is open
    /// </summary>
    /// <returns>True: Success | False: Failure</returns>
    public bool isConnectionOpen() {
        this.init();
        if(Connection == null)
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password);
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }

        return true;
    }

    /// <summary>
    /// Run a DB Operation by OperationID
    /// 
    ///     1 - Insert Expense
    ///     2 - Update Expense
    /// </summary>
    /// <param name="op">Operation to execute</param>
    /// <param name="expense">Expense that will be processed</param>
    /// <param name="id">Id that is used to identify the expense that will be update. 0 used in case an insert operation</param>
    /// <returns>True: Success | False: Failure</returns>
    public bool runOperation(int op, Expense expense, int id = 0) {

        /**
         * Test if:
         * Conection with DB is open,
         * User Exist,
         * Desp Id Exist,
         * Data is filled;
         * */
        if(isConnectionOpen() && hasUser(expense.hashUser) && expense.isRequestFilled())
        {
            string query;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            // Diferent querys and parameters per OperationID
            switch(op)
            {
                case 1: // Insert
                    query = "INSERT INTO `despesas_isi`.`despesas` (`nome`, `descricao`, `valEur`, `valUsd`, `utilizador_id`) " +
                "VALUES (@nome, @desc, @valEuro, @valUsd, @utilizador);";
                    parameters.Add(new MySqlParameter("?utilizador", expense.hashUser));
                    break;
                case 2: // Update
                    // Can this user update expense?
                    if(id != 0 && expense.canUpdate(id, expense.hashUser) && expense.hasExpense(id))
                    {
                        query = "UPDATE `despesas_isi`.`despesas` " +
                       "SET `nome` = @nome, `descricao` = @desc, `valEur` = @valEuro, `valUsd` = @valUsd WHERE(`id` = @id);";
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
            parameters.Add(new MySqlParameter("?nome", expense.nome));
            parameters.Add(new MySqlParameter("?desc", expense.descricao));
            parameters.Add(new MySqlParameter("?dataHoraCriacao", expense.dataHoraCriacao));
            parameters.Add(new MySqlParameter("?valEuro", expense.valEuro));
            parameters.Add(new MySqlParameter("?valUsd", expense.valUsd));

            // Execute DB Op
            return execOpWithStatus(query, parameters);
        }
        Connection.Close();
        return false;
    }

    /// <summary>
    /// Execute operation and a boolean
    /// </summary>
    /// <param name="query">SQL query</param>
    /// <param name="parameters">Expense data that will be used on DB</param>
    /// <returns>True: Success | False: Failure</returns>
    public bool execOpWithStatus(string query, List<MySqlParameter> parameters) {
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Connection);
            parameters.ForEach(paramn =>
            {
                cmd.Parameters.Add(paramn);
            });
            var reader = cmd.ExecuteReader();
            reader.Close();
            return true;
        }
        catch(Exception e)
        {
            Console.Write(e);
            return false;
        }
    }
    /// <summary>
    /// Execute operation and return data
    /// </summary>
    /// <param name="query">SQL Query</param>
    /// <param name="parameters">Expense data that will be used on DB</param>
    /// <returns>MySqlDataReader that allows to read the data retrieved from DB</returns>
    public MySqlDataReader execOpWithData(string query, List<MySqlParameter> parameters) {
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, Connection);
            parameters.ForEach(paramn =>
            {
                cmd.Parameters.Add(paramn);
            });
            return cmd.ExecuteReader();
        }
        catch(Exception e)
        {
            Console.Write(e);
            return null;
        }
    }

    /// <summary>
    /// Close DB Connection
    /// </summary>
    public void Close() {
        Connection.Close();
    }

    /// <summary>
    /// Set the default DB Config
    /// </summary>
    public void init() {

        this.Server = "localhost";
        this.DatabaseName = "despesas_isi";
        this.UserName = "root";
        this.Password = "";

    }

    /// <summary>
    /// Test if the user exist
    /// </summary>
    /// <param name="hashUser">Unique data that identify only one user</param>
    /// <returns>True: Has User | False: User doest Exist </returns>
    public bool hasUser(string hashUser) {

        string query = "SELECT emailSha FROM despesas_isi.utilizadores WHERE despesas_isi.utilizadores.emailSha = @hashUser;";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        parameters.Add(new MySqlParameter("@hashUser", hashUser));
        var reader = execOpWithData(query, parameters);

        try
        {
            if(reader != null && reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }
        catch(Exception e)
        {
            return false;
        }
    }

}

