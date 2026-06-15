using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Auth;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Registrar(RegisterDto dto)
        {
            try
            {
                _authService.Registrar(dto);

                return Ok(new
                {
                    mensagem = "Usuário cadastrado com sucesso."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            try
            {
                var resposta = _authService.Login(dto);

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }
    }
}