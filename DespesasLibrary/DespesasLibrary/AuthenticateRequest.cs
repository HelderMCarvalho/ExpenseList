using System.ComponentModel.DataAnnotations;

namespace DespesasLibrary
{
    public class AuthenticateRequest
    {
        public AuthenticateRequest(string hashUser)
        {
            HashUser = hashUser;
        }

        [Required] public string HashUser { get; set; }
    }
}