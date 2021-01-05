using System;
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


        public void SetUser()
        {
            DbConnect dbConnect = new DbConnect();
            if (dbConnect.IsConnectionOpen())
            {
                const string query =
                    "SELECT emailSha, moedaPadrao FROM despesas_isi.utilizadores WHERE despesas_isi.utilizadores.emailSha = @hashUser;";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@hashUser", EmailSha)
                };
                MySqlDataReader reader = dbConnect.ExecSqlWithData(query, parameters);
                if (reader != null && reader.HasRows && reader.Read())
                {
                    EmailSha = reader.GetString(0);
                    MoedaPadrao = reader.GetString(1);
                    reader.Close();
                }
            }
        }
    }
}