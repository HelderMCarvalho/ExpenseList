using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DespesasLibrary
{
    public abstract class ApiData
    {
        /// <summary>
        /// Check if the request contains data
        /// </summary>
        /// <returns>True: All fields are filled | False: Some fields are not field </returns>
        public virtual bool isRequestFilled() {
            bool filled = true;
            foreach(var prop in GetType().GetProperties())
            {
                var x = prop.GetValue(this);
                filled = (x == null) ? false : true;
            }
            return filled;
        }


        /// <summary>
        /// Check if a user can update a expense
        /// </summary>
        /// <param name="despesaId">Expense id that identify only one expense</param>
        /// <param name="hashUser">Unique data that identify only one user</param>
        /// <returns>True: The user can update the expense with this id| False: The user cant update the expense </returns>
        public virtual bool canUpdate(string despesaId, string hashUser) {
            DbConnect db = new DbConnect();
            if(db.isConnectionOpen())
            {
                string query = "";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
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

                    return false;
                }
                catch(Exception e)
                {
                    return false;
                }

            }
            return false;

        }
    }
}
