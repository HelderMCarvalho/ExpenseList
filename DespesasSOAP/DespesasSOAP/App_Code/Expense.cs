using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for Expense
/// </summary>
public class Expense
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
    /// Check if the request contains data
    /// </summary>
    /// <returns>True: All fields are filled | False: Some fields are not field </returns>
    public bool isRequestFilled() {
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
    /// <param name="id">Expense id that identify only one expense</param>
    /// <param name="hashUser">Unique data that identify only one user</param>
    /// <returns>True: The user can update the expense with this id| False: The user cant update the expense </returns>
    public bool canUpdate(int id, string hashUser) {
        DbConnect db = new DbConnect();
        if(db.isConnectionOpen())
        {
            string query = "SELECT id FROM despesas_isi.despesas WHERE despesas_isi.despesas.utilizador_id = @hashUser AND despesas_isi.despesas.id = @id;";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@hashUser", hashUser));
            parameters.Add(new MySqlParameter("@id", id));
            var reader = db.execOpWithData(query, parameters);

            try
            {
                if(reader != null && reader.HasRows)
                {
                    return true;
                }
                reader.Close();
                return false;
            }
            catch(Exception e)
            {
                return false;
            }

        }
        return false;

    }

    /// <summary>
    /// Check if the expense with this ID exist in DB
    /// </summary>
    /// <param name="id">Unique data that identify only one expense</param>
    /// <returns>True: Exist | False: Dont Exist </returns>
    public bool hasExpense(int id) {
        DbConnect db = new DbConnect();
        if(db.isConnectionOpen())
        {
            string query = "SELECT id FROM despesas_isi.despesas where despesas_isi.despesas.id = @id;";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@id", id));
            var reader = db.execOpWithData(query, parameters);

            try
            {
                if(reader != null && reader.HasRows)
                {
                    reader.Close();
                    return true;
                }
                reader.Close();
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