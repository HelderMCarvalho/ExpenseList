using DespesasLibrary;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
        public ActionResult<List<Expense>> GetAll(string hashUser) {
            DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();
            if(dbConnect.IsConnectionOpen())
            {
                if(!dbConnect.HasUser(hashUser))
                    return new StatusCodeResult(401);

                string query = "SELECT id, nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id FROM despesas_isi.despesas WHERE despesas_isi.despesas.utilizador_id = @hashUser;";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@hashUser", hashUser)
                };
                MySqlDataReader reader = dbConnect.ExecSQLWithData(query, parameters);
                try
                {
                    if(reader != null && reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            res.Add(new Expense(
                            nome: reader.GetString(1),
                            descricao: reader.GetString(2),
                            dataHoraCriacao: reader.GetDateTime(3),
                            valEuro: reader.GetDecimal(4),
                            valUsd: reader.GetDecimal(5),
                            hashUser: reader.GetString(6)));
                        }
                    }
                    reader.Close();
                }
                catch(Exception)
                {
                    return new StatusCodeResult(500);
                }
            }
            return res;
        }

        /// <summary>
        ///     Delete an Expense by ID
        /// </summary>
        /// <param name="id">ID of the Expense to be deleted</param>
        /// <returns>
        ///     <para>TRUE: Expense deleted successfully</para>
        ///     <para>FALSE: Expense not deleted successfully</para>
        /// </returns>
        [HttpDelete("[action]/{id}")]
        public ActionResult<bool> Delete(string hashUser, string id) {
            DbConnect dbConnect = new DbConnect();
            if(dbConnect.IsConnectionOpen())
            {
                // Check if User or Expense exist
                if(!dbConnect.HasUser(hashUser) || !new Expense().HasExpense(id))
                    return new StatusCodeResult(401);

                var query = "DELETE FROM despesas_isi.despesas WHERE (id = @id);";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@id", id)
                };
                if(dbConnect.ExecSQLWithStatus(query, parameters))
                    return Ok();
            }
            return new StatusCodeResult(500);
        }
    }
}