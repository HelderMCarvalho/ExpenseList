using System;
using System.Collections.Generic;
using DespesasLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace DespesasREST.Controllers
{
    [Route("[controller]/{hashUser}")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        /// <summary>
        ///     Get all Expenses by User
        /// </summary>
        /// <returns>List of Expenses</returns>
        [HttpGet("[action]")]
        public ActionResult<List<Expense>> GetAll(string hashUser)
        {
            DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();

            // Check if connection is opened
            if (!dbConnect.IsConnectionOpen()) return res;

            if (!dbConnect.HasUser(hashUser)) return new StatusCodeResult(401);

            const string query =
                "SELECT id, nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id FROM despesas_isi.despesas WHERE utilizador_id = @hashUser;";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new("@hashUser", hashUser)
            };
            MySqlDataReader reader = dbConnect.ExecSqlWithData(query, parameters);
            try
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        res.Add(new Expense(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                            reader.GetDateTime(3),
                            reader.GetDecimal(4), reader.GetDecimal(5), reader.GetString(6)));
                    }
                }

                reader?.Close();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }

            return res;
        }

        /// <summary>
        ///     Delete an Expense by ID
        /// </summary>
        /// <param name="hashUser">Hashed email of the User</param>
        /// <param name="id">ID of the Expense to be deleted</param>
        /// <returns>
        ///     <para>TRUE: Expense deleted successfully</para>
        ///     <para>FALSE: Expense not deleted successfully</para>
        /// </returns>
        [HttpDelete("[action]/{id}")]
        public ActionResult<bool> Delete(string hashUser, string id)
        {
            DbConnect dbConnect = new DbConnect();

            // Check if connection is opened
            if (!dbConnect.IsConnectionOpen()) return new StatusCodeResult(500);

            // Check if User or Expense exist
            if (!dbConnect.HasUser(hashUser) || !Expense.HasExpense(id)) return new StatusCodeResult(401);

            const string query = "DELETE FROM despesas_isi.despesas WHERE (id = @id);";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new("@id", id)
            };
            return dbConnect.ExecSqlWithStatus(query, parameters) ? Ok() : new StatusCodeResult(500);
        }

        /// <summary>
        ///     Verifies if the User exists
        /// </summary>
        /// <param name="hashUser">Hash os the User to look for</param>
        /// <returns>
        ///     <para>TRUE: The User exists</para>
        ///     <para>FALSE: The User doesn't exist</para>
        /// </returns>
        [HttpGet("[action]")]
        public ActionResult<bool> HasUser(string hashUser)
        {
            DbConnect dbConnect = new DbConnect();

            // Check if connection is opened
            if (!dbConnect.IsConnectionOpen()) return new StatusCodeResult(500);

            return dbConnect.HasUser(hashUser);
        }

        /// <summary>
        ///     Gets the last Id of a specific table but the Id is bound the a specific User
        /// </summary>
        /// <param name="nomeTabela">Name of the table to get the last Id</param>
        /// <param name="hashUser">Hash of the user</param>
        /// <returns>The last Id of that table bound to that User</returns>
        [HttpGet("[action]/{nomeTabela}")]
        public ActionResult<int> GetLastId(string nomeTabela, string hashUser)
        {
            DbConnect dbConnect = new DbConnect();

            // Check if connection is opened
            if (!dbConnect.IsConnectionOpen()) return new StatusCodeResult(500);

            return dbConnect.GetLastId(nomeTabela, hashUser);
        }
    }
}