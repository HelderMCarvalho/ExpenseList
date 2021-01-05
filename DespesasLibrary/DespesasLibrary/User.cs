using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DespesasLibrary
{
    public class User : ApiData
    {
        public string EmailSha { get; set; }
        public string MoedaPadrao { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public User()
        {
        }

        /// <summary>
        ///     Constructor with data
        /// </summary>
        /// <param name="emailSha">Hashed email of the User</param>
        /// <param name="moedaPadrao">Default currency of the User (EUR, USD)</param>
        public User(string emailSha, string moedaPadrao)
        {
            EmailSha = emailSha;
            MoedaPadrao = moedaPadrao;
        }

        /// <summary>
        ///     Obtém a informação do Utilizador, se este existir
        /// </summary>
        public void GetUserFromDb()
        {
            DbConnect dbConnect = new DbConnect();

            // Se a conexão não está aberta sai
            if (!dbConnect.IsConnectionOpen()) return;

            const string query =
                "SELECT emailSha, moedaPadrao FROM despesas_isi.utilizadores WHERE emailSha = @hashUser;";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@hashUser", EmailSha)
            };
            MySqlDataReader reader = dbConnect.ExecSqlWithData(query, parameters);

            // Se não receber dados sai
            if (reader == null || !reader.HasRows || !reader.Read()) return;

            EmailSha = reader.GetString(0);
            MoedaPadrao = reader.GetString(1);
            reader.Close();
        }
    }
}