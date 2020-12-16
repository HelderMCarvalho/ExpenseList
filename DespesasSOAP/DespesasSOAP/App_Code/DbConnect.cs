using MySql.Data.MySqlClient;

public class DbConnect
{

    public string Server { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }



    public MySqlConnection Connection { get; set; }

    public bool IsConnect() {
        this.init();
        if(Connection == null)
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password);
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }

        return true;
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


    public bool checkUser(string hashUser) {

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
                return true;
            }
        }
        return false;
    }
}

