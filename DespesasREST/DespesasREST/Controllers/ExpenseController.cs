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
        /// Get all Expenses
        /// </summary>
        /// <returns>JSON with all Exposes</returns>
        [HttpGet("[action]")]
        public ActionResult<List<Expense>> getAll(string hashUser) {
            //TODO: GetAllByUser
            DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();
            if(dbConnect.isConnectionOpen())
            {
                if(!dbConnect.hasUser(hashUser))
                    return new StatusCodeResult(401);

                var query = "SELECT id,nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id FROM despesas_isi.despesas WHERE despesas_isi.despesas.utilizador_id = @hashUser;";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@hashUser",hashUser));
                MySqlDataReader reader = dbConnect.execOpWithData(query, parameters);
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
                catch(Exception e)
                {
                    return new StatusCodeResult(500);
                }
            }
            return res;
        }

        /// <summary>
        /// Delete an Expense by ID
        /// </summary>
        /// <param name="id">The expense that will be deleted</param>
        /// <returns>True: Success | False: Error</returns>
        [HttpDelete("[action]/{id}")]
        public ActionResult<bool> delete(string hashUser, int id) {

            DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();
            if(dbConnect.isConnectionOpen())
            {
                // If user/expense invalid
                if(!dbConnect.hasUser(hashUser) || !new Expense().hasExpense(id))
                    return new StatusCodeResult(401);

                var query = "DELETE FROM `despesas_isi`.`despesas` WHERE (`id` = @id);";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@id", id));
                if(dbConnect.execOpWithStatus(query, parameters))
                    return Ok();

            }
            return new StatusCodeResult(500);
        }
    }
}
