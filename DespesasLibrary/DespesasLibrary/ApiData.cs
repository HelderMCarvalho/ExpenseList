using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DespesasLibrary
{
    public abstract class ApiData
    {
        /// <summary>
        ///     Check if the request contains data
        /// </summary>
        /// <returns>
        ///     <para>TRUE: All fields are filled</para>
        ///     <para>FALSE: Some fields are not field</para>
        /// </returns>
        public virtual bool IsRequestFilled() {
            bool filled = true;
            foreach(PropertyInfo prop in GetType().GetProperties())
            {
                object x = prop.GetValue(this);
                filled = x != null;
            }
            return filled;
        }

        /// <summary>
        ///     Check if a User can update an Expense
        ///     If the User is the owner of the Expense he can update it
        /// </summary>
        /// <param name="despesaId">Expense ID to be checked</param>
        /// <param name="hashUser">User Hash to be checked</param>
        /// <returns>
        ///     <para>TRUE: The User can update the Expense</para>
        ///     <para>FALSE: The User cant update the Expense</para>
        /// </returns>
        public virtual bool CanUpdate(string despesaId, string hashUser) {
            DbConnect db = new DbConnect();
            if(db.IsConnectionOpen())
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                string query;
                if(GetType() == typeof(Expense))
                {
                    query = "SELECT id FROM despesas_isi.despesas WHERE despesas_isi.despesas.utilizador_id = @hashUser AND despesas_isi.despesas.id = @id;";
                    parameters.Add(new MySqlParameter("@id", despesaId));
                }
                else
                {
                    query = "SELECT emailSha FROM despesas_isi.utilizadores WHERE despesas_isi.utilizadores.emailSha = @hashUser;";
                }
                parameters.Add(new MySqlParameter("@hashUser", hashUser));

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
                    return false;
                }
            }
            return false;
        }
    }
}