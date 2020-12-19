namespace DespesasLibrary
{
    public class User : ApiData
    {

        public string emailSha { get; set; }
        public string moedaPadrao { get; set; }

        public User() { }
        public User(string emailSha, string moedaPadrao) {
            this.emailSha = emailSha;
            this.moedaPadrao = moedaPadrao;
        }
    }
}
