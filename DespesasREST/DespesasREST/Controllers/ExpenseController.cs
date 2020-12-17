using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DespesasREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        /// <summary>
        /// Get all Expenses
        /// </summary>
        /// <returns>JSON with all Exposes</returns>
        [HttpGet]
        public ActionResult<List<Expense>> getAll() {
            DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();
            if(dbConnect.isConnectionOpen())
            {
                var query = "SELECT id,nome, descricao, dataHoraCriacao, valEur, valUsd, utilizador_id FROM despesas_isi.despesas;";
                MySqlDataReader reader = dbConnect.execOpWithData(query, new List<MySqlParameter>());
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
                    throw e;
                }
            }
            return res;
        }


        /// <summary>
        /// Delete an Expense by ID
        /// </summary>
        /// <param name="id">The expense that will be deleted</param>
        /// <returns>True: Success | False: Error</returns>
        [HttpDelete("delete/{id}")]
        public ActionResult<bool> delete(int id) {

           DbConnect dbConnect = new DbConnect();
            List<Expense> res = new List<Expense>();
            if(dbConnect.isConnectionOpen())
            {
                var query = "DELETE FROM `despesas_isi`.`despesas` WHERE (`id` = @id);";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@id",id));
                return dbConnect.execOpWithStatus(query, parameters);
            }
            return false;
        }




    }
}
