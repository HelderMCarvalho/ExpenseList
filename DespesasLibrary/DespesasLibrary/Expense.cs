using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DespesasLibrary
{

    /// <summary>
    /// Summary description for Expense
    /// </summary>
    public class Expense : ApiData
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public DateTime dataHoraCriacao { get; set; }
        public decimal valEuro { get; set; }
        public decimal valUsd { get; set; }
        public string hashUser { get; set; }

        public Expense() { }
        public Expense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
            this.nome = nome;
            this.descricao = descricao;
            this.dataHoraCriacao = dataHoraCriacao;
            this.valEuro = valEuro;
            this.valUsd = valUsd;
            this.hashUser = hashUser;
        }

        /// <summary>
        /// Check if the expense with this ID exist in DB
        /// </summary>
        /// <param name="id">Unique data that identify only one expense</param>
        /// <returns>True: Exist | False: Dont Exist </returns>
        public bool hasExpense(string id) {
            DbConnect db = new DbConnect();
            if(db.isConnectionOpen())
            {
                string query = "SELECT id FROM despesas_isi.despesas where despesas_isi.despesas.id = @id;";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@id", id));
                var reader = db.execOpWithData(query, parameters);

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
                catch(Exception e)
                {
                    reader.Close();
                    return false;
                }
            }
            return false;
        }
    }
}