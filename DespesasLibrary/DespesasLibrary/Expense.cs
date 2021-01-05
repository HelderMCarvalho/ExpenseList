using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DespesasLibrary
{
    public class Expense : ApiData
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public decimal ValEur { get; set; }
        public decimal ValUsd { get; set; }
        public string HashUser { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Expense()
        {
        }

        /// <summary>
        ///     Constructor with data
        /// </summary>
        /// <param name="id">Expense Id</param>
        /// <param name="nome">Expense Name</param>
        /// <param name="descricao">Expense Description</param>
        /// <param name="dataHoraCriacao">Expense DateTime of creation</param>
        /// <param name="valEur">Expense EUR value</param>
        /// <param name="valUsd">Expense USD value</param>
        /// <param name="hashUser">Hash (ID) of the user that owns the Expense</param>
        public Expense(string id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEur,
            decimal valUsd, string hashUser)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            DataHoraCriacao = dataHoraCriacao;
            ValEur = valEur;
            ValUsd = valUsd;
            HashUser = hashUser;
        }

        /// <summary>
        ///     Check if the Expense exists in DB
        /// </summary>
        /// <param name="id">ID of the Expense to be updated</param>
        /// <returns>
        ///     <para>TRUE: Expense exists</para>
        ///     <para>FALSE: Expense does not exist</para>
        /// </returns>
        public static bool HasExpense(string id)
        {
            DbConnect db = new DbConnect();

            // Check if connection is opened
            if (!db.IsConnectionOpen()) return false;

            const string query = "SELECT id FROM despesas_isi.despesas WHERE id = @id;";
            List<MySqlParameter> parameters = new List<MySqlParameter> {new MySqlParameter("@id", id)};
            MySqlDataReader reader = db.ExecSqlWithData(query, parameters);
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
                reader?.Close();
                return false;
            }
        }
    }
}