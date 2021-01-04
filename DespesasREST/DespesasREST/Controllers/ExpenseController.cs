using System;
using System.Collections.Generic;
using DespesasLibrary;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace DespesasREST.Controllers
{
    [Route("[controller]/{hashUser}")]
    [ApiController]
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
                "SELECT id, nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id FROM despesas_isi.despesas WHERE despesas_isi.despesas.utilizador_id = @hashUser;";
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
                        res.Add(new Expense(reader.GetString(0),reader.GetString(1), reader.GetString(2), reader.GetDateTime(3),
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
            if (!dbConnect.HasUser(hashUser) || !new Expense().HasExpense(id)) return new StatusCodeResult(401);

            const string query = "DELETE FROM despesas_isi.despesas WHERE (id = @id);";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new("@id", id)
            };
            return dbConnect.ExecSqlWithStatus(query, parameters) ? Ok() : new StatusCodeResult(500);
        }
    }
}