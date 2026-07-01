using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Auth;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status409Conflict)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status500InternalServerError)]
        public IActionResult Registrar(
            [FromBody] RegisterDto dto)
        {
            _authService.Registrar(dto);

            return Ok(new
            {
                mensagem = "Usuário cadastrado com sucesso."
            });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [ProducesResponseType(
            typeof(AuthResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status500InternalServerError)]
        public ActionResult<AuthResponseDto> Login(
            [FromBody] LoginDto dto)
        {
            var resposta = _authService.Login(dto);

            return Ok(resposta);
        }
    }
}