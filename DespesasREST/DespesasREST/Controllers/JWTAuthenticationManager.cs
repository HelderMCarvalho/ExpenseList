using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DespesasLibrary;
using Microsoft.IdentityModel.Tokens;

namespace DespesasREST.Controllers
{
    public interface IJwtAuthenticationManager
    {
        /// <summary>
        ///     Autenticar o Utilizador através da sua Hash
        /// </summary>
        /// <param name="authenticationDetails">Detalhes da autenticação</param>
        /// <returns>AuthenticateResponse com Token</returns>
        AuthenticateResponse Authenticate(AuthenticateRequest authenticationDetails);
    }

    /// <summary>
    ///     Gerir Autenticação e JWT
    /// </summary>
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string _tokenKey;

        /// <summary>
        ///     Gere Token
        /// </summary>
        /// <param name="tokenKey"></param>
        public JwtAuthenticationManager(string tokenKey)
        {
            _tokenKey = tokenKey;
        }

        /// <summary>
        ///     Autenticar o Utilizador através da sua Hash
        /// </summary>
        /// <param name="authenticationDetails">Detalhes da autenticação</param>
        /// <returns>AuthenticateResponse com Token</returns>
        public AuthenticateResponse Authenticate(AuthenticateRequest authenticationDetails)
        {
            if (!ValidarUser(authenticationDetails)) return null;

            string token = GenerateTokenString(authenticationDetails.HashUser, DateTime.UtcNow);

            return new AuthenticateResponse
            {
                HashUser = authenticationDetails.HashUser,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        /// <summary>
        ///     Gerar token
        /// </summary>
        /// <param name="hashUser">Hash do Utilizador que vai ter o token gerado</param>
        /// <param name="expires">Data de expiração do token</param>
        /// <param name="claims"></param>
        /// <returns>Token gerado</returns>
        private string GenerateTokenString(string hashUser, DateTime expires, Claim[] claims = null)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_tokenKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims ?? new Claim[] {new(ClaimTypes.Name, hashUser)}),
                Expires = expires.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        /// <summary>
        ///     Certificar dados de autenticação
        /// </summary>
        /// <param name="authenticationDetails">Detalhes de autenticação</param>
        /// <returns></returns>
        private bool ValidarUser(AuthenticateRequest authenticationDetails)
        {
            DbConnect dbConnect = new();

            // Verifica se a conexão está aberta e se existe Utilizador
            return dbConnect.IsConnectionOpen() && dbConnect.HasUser(authenticationDetails.HashUser);
        }
    }
}