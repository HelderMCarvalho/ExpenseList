using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DespesasLibrary
{
    public class Expense : ApiData
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public DateTime dataHoraCriacao { get; set; }
        public decimal valEuro { get; set; }
        public decimal valUsd { get; set; }
        public string hashUser { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Expense() { }

        /// <summary>
        ///     Constructor with data
        /// </summary>
        /// <param name="nome">Expense Name</param>
        /// <param name="descricao">Expense Description</param>
        /// <param name="dataHoraCriacao">Expense DateTime of creation</param>
        /// <param name="valEuro">Expense EUR value</param>
        /// <param name="valUsd">Expense USD value</param>
        /// <param name="hashUser">Hash (ID) of the user that owns the Expense</param>
        public Expense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
            this.nome = nome;
            this.descricao = descricao;
            this.dataHoraCriacao = dataHoraCriacao;
            this.valEuro = valEuro;
            this.valUsd = valUsd;
            this.hashUser = hashUser;
        }

        /// <summary>
        ///     Check if the Expense exists in DB
        /// </summary>
        /// <param name="id">ID of the Expense to be updated</param>
        /// <returns>
        ///     <para>TRUE: Expense exists</para>
        ///     <para>FALSE: Expense does not exist</para>
        /// </returns>
        public bool HasExpense(string id) {
            DbConnect db = new DbConnect();
            if(db.IsConnectionOpen())
            {
                string query = "SELECT id FROM despesas_isi.despesas where despesas_isi.despesas.id = @id;";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@id", id));
                MySqlDataReader reader = db.ExecSQLWithData(query, parameters);
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
                    reader.Close();
                    return false;
                }
            }
            return false;
        }
    }
}