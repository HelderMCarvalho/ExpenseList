/*
 * lufer
 * ISI
 * */

using DespesasLibrary;
using DespesasREST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DespesasREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        //classe que gera o JWT
        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;

        public SecurityController(IJwtAuthenticationManager jWtAuthenticationManager)
        {
            _jWtAuthenticationManager = jWtAuthenticationManager;

        }

        /// <summary>
        /// Método para Autenticação
        /// </summary>
        /// <param name="loginDetalhes"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthenticateResponse> Login(AuthenticateRequest loginDetalhes) //or ([FromBody] AuthenticateRequest loginDetalhes)
        {
            var token = _jWtAuthenticationManager.Authenticate(loginDetalhes);           
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
    }
}
