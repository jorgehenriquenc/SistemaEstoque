using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace SistemaEstoque.Api.Controllers
{

    [Authorize]
    // Define a rota base: api/dashboard
    [Route("api/[controller]")]

    // Indica que esta classe é um Controller de API
    [ApiController]
    public class DashboardController : ControllerBase
    {
        // Serviço responsável pelas regras de negócio de pedidos e dashboard
        private readonly PedidoService _pedidoService;

        // Construtor que recebe o PedidoService por injeção de dependência
        public DashboardController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // Endpoint responsável por exibir um resumo geral do estoque
        [HttpGet]
        public IActionResult ExibirDashboard()
        {
            var dashboard = _pedidoService.ExibirDashboard();

            return Ok(dashboard);
        }
    }
}