using MySql.Data.MySqlClient;
using System;

public class DespesasSOAP : IDespesasSOAP
{

    bool IDespesasSOAP.addDespesa(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {

        DbConnect db = new DbConnect();
        if(db.IsConnect() && hashUser != null && db.checkUser(hashUser))
        {
            if(nome != null && descricao != null && dataHoraCriacao != null)
            {
                var query = "INSERT INTO `despesas`.`despesas` (`nome`, `descricao`, `valEur`, `valUsd`, `utilizador_id`) VALUES (@nome, @desc, @valEuro, @valUsd, @utilizador);";
                var cmd = new MySqlCommand(query, db.Connection);
                cmd.Parameters.AddWithValue("?nome", nome);
                cmd.Parameters.AddWithValue("?desc", descricao);
                cmd.Parameters.AddWithValue("?dataHoraCriacao", dataHoraCriacao);
                cmd.Parameters.AddWithValue("?valEuro", valEuro);
                cmd.Parameters.AddWithValue("?valUsd", valUsd);
                cmd.Parameters.AddWithValue("?utilizador", hashUser);
                var reader = cmd.ExecuteReader();

                db.Connection.Close();
                return true;
            }
        }
        db.Connection.Close();
        return false;

    }
    bool IDespesasSOAP.updateDespesa(int id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {

        return true;
    }
}
