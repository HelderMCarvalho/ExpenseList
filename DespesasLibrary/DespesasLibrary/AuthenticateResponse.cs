using System;
using DespesasLibrary;

namespace DespesasREST.Models
{
    public class AuthenticateResponse
    {
        public string HashUser { get; set; }
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public AuthenticateResponse() { }
        public AuthenticateResponse(string user, string token)
        {
            HashUser = user;
            Token = token;
            Expiration = DateTime.Now.AddMinutes(120);
        }

        public AuthenticateResponse(AuthenticateRequest user, string token, DateTime expires)
        {
            HashUser = user.HashUser;
            Token = token;
            Expiration = expires;
        }
    }
}
