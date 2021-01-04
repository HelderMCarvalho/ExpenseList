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
    }
}