using DespesasLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DespesasREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;

        /// <summary>
        ///     Construtor
        /// </summary>
        /// <param name="jWtAuthenticationManager">Gestor de autenticação</param>
        public SecurityController(IJwtAuthenticationManager jWtAuthenticationManager)
        {
            _jWtAuthenticationManager = jWtAuthenticationManager;
        }

        /// <summary>
        ///     Método de Autenticação
        /// </summary>
        /// <param name="authenticationDetails">Detalhes da autenticação</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthenticateResponse> Login(AuthenticateRequest authenticationDetails)
        {
            AuthenticateResponse token = _jWtAuthenticationManager.Authenticate(authenticationDetails);
            if (token == null) return Unauthorized();
            return Ok(token);
        }
    }
}