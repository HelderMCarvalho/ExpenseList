using System;

namespace DespesasLibrary
{
    public class AuthenticateResponse
    {
        public string HashUser { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public AuthenticateResponse()
        {
        }

        /// <summary>
        ///     Cria uma AuthenticateResponse
        /// </summary>
        /// <param name="user">Hash do Utilizador</param>
        /// <param name="token">Token de segurança</param>
        public AuthenticateResponse(string user, string token)
        {
            HashUser = user;
            Token = token;
            Expiration = DateTime.Now.AddMinutes(120);
        }

        /// <summary>
        ///     Cria uma AuthenticateResponse
        /// </summary>
        /// <param name="user">Hash do Utilizador</param>
        /// <param name="token">Token de segurança</param>
        /// <param name="expires">Data de expiração do token</param>
        public AuthenticateResponse(AuthenticateRequest user, string token, DateTime expires)
        {
            HashUser = user.HashUser;
            Token = token;
            Expiration = expires;
        }
    }
}