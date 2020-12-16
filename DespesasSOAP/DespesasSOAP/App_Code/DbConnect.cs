using MySql.Data.MySqlClient;

public class DbConnect
{

    public string Server { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }



    public MySqlConnection Connection { get; set; }

    public bool isConnectionOpen() {
        this.init();
        if(Connection == null)
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password);
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }

        return true;
    }

    public bool executeOp(int op, Despesa desp, int id = 0) {

        if(isConnectionOpen() && desp.hashUser != null &&
            existeUser(desp.hashUser) && desp.nome != null &&
            desp.descricao != null && desp.dataHoraCriacao != null)
        {
            string query = ""; 
            switch(op)
            {
                case 1: // Operação Criar
                    query = "INSERT INTO `despesas`.`despesas` (`nome`, `descricao`, `valEur`, `valUsd`, `utilizador_id`) VALUES (@nome, @desc, @valEuro, @valUsd, @utilizador);";
                    break;
                case 2: // Operação Update
                    query = "UPDATE `despesas`.`despesas` SET `nome` = @nome, `descricao` = @desc, `valEur` = @valEuro, `valUsd` = @valUsd WHERE(`id` = @id);";
                    break;
            }
            MySqlCommand cmd = new MySqlCommand(query, Connection);

            switch(op)
            {
                case 1: // Operação Criar
                    cmd.Parameters.AddWithValue("?utilizador", desp.hashUser);
                    break;
                case 2: // Operação Update
                    cmd.Parameters.AddWithValue("?id", id);
                    break;
            }

            cmd.Parameters.AddWithValue("?nome", desp.nome);
            cmd.Parameters.AddWithValue("?desc", desp.descricao);
            cmd.Parameters.AddWithValue("?dataHoraCriacao", desp.dataHoraCriacao);
            cmd.Parameters.AddWithValue("?valEuro", desp.valEuro);
            cmd.Parameters.AddWithValue("?valUsd", desp.valUsd);

            var reader = cmd.ExecuteReader();

            Connection.Close();
            return true;

        }
        Connection.Close();
        return false;
    }




    public void Close() {
        Connection.Close();
    }

    public void init() {

        this.Server = "localhost";
        this.DatabaseName = "despesas";
        this.UserName = "root";
        this.Password = "";

    }


    public bool existeUser(string hashUser) {

        string query = "SELECT count(emailSha) as existeUser FROM despesas.utilizadores WHERE despesas.utilizadores.emailSha = @hashUser;";
        var cmd = new MySqlCommand(query, this.Connection);

        cmd.Parameters.AddWithValue("@hashUser", hashUser);
        var reader = cmd.ExecuteReader();

        if(reader.HasRows)
        {
            int qtdUser = 0;
            while(reader.Read())
            {
                qtdUser = reader.GetInt32(0);
            }
            if(qtdUser > 0)
            {
                reader.Close();
                return true;
            }
        }
        reader.Close();
        return false;
    }

}

