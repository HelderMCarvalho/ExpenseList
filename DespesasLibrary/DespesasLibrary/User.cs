namespace DespesasLibrary
{
    public class User : ApiData
    {
        public string emailSha { get; set; }
        public string moedaPadrao { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public User() { }

        /// <summary>
        ///     Constructor with data
        /// </summary>
        /// <param name="emailSha">Hashed email of the User</param>
        /// <param name="moedaPadrao">Default currency of the User (EUR, USD)</param>
        public User(string emailSha, string moedaPadrao) {
            this.emailSha = emailSha;
            this.moedaPadrao = moedaPadrao;
        }
    }
}