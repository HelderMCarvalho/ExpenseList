/*
 * lufer
 * ISI
 * */

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DespesasLibrary;
using DespesasREST.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
// Incluidas

namespace DespesasREST.Controllers
{
    public interface IJwtAuthenticationManager
    {
        /// <summary>
        /// Várias possibilidades de autenticação
        /// Podem ser criadas mais!
        /// </summary>
        /// <param name="loginDetails"></param>
        /// <returns></returns>
        AuthenticateResponse Authenticate(AuthenticateRequest loginDetails);
    }

    /// <summary>
    /// Classe auxiliar para gerir Autenticação e JWT
    /// </summary>
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string _tokenKey;

        #region ACEDER_APPSETTINGS.JSON
        //caso se pretenda aceder a appsettings.json
        private IConfiguration _config;
        /// <summary>
        /// instanciado em startup.cs:
        /// services.AddSingleton<IConfiguration>(Configuration)
        /// </summary>
        /// <param name="configuration"></param>
        public JwtAuthenticationManager(IConfiguration configuration)
        {
            _config = configuration;                            //para aceder a appsettings.json
        }
        #endregion

        /// <summary>
        /// Gere Token
        /// </summary>
        /// <param name="tokenKey"></param>
        public JwtAuthenticationManager(string tokenKey)
        {
            this._tokenKey = tokenKey;
        }


        /// <summary>
        /// Autenticar o utilizando através da HashUser
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="loginDetails"></param>
        /// <returns></returns>
        public AuthenticateResponse Authenticate(AuthenticateRequest loginDetails)
        {
            if (!ValidarUser(loginDetails)) return null;

            var token = GenerateTokenString(loginDetails.HashUser, DateTime.UtcNow);

            return new AuthenticateResponse
            {
                HashUser = loginDetails.HashUser,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        /// <summary>
        /// Gerar JWT token!
        /// </summary>
        /// <param name="username"></param>
        /// <param name="expires"></param>
        /// <param name="claims">Claims Opcional!!!</param>
        /// <returns></returns>
        string GenerateTokenString(string username, DateTime expires, Claim[] claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 claims ?? new Claim[]          //caso existam claims usar, senão, criar novo com username
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                //NotBefore = expires,
                Expires = expires.AddMinutes(60),    //expira em 60 minutos
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }


        #region GEREAUTH

        /// <summary>
        /// Certificar dados de aautenticação
        /// </summary>
        /// <param name="loginDetalhes"></param>
        /// <returns></returns>
        private bool ValidarUser(AuthenticateRequest loginDetalhes)
        {
            //Verifica na BD
            
            DbConnect dbConnect = new();

            // Check if connection is opened
            if (!dbConnect.IsConnectionOpen()) return false;
            var user = dbConnect.HasUser(loginDetalhes.HashUser);
            
            if (user)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        #endregion
    }
}
