using System.ComponentModel.DataAnnotations;

namespace DespesasLibrary
{
    public class AuthenticateRequest
    {
        public AuthenticateRequest(string hashUser)
        {
            HashUser = hashUser;
        }
        public AuthenticateRequest()
        {
        }

        [Required]
        public string HashUser { get; set; }
    }
}
