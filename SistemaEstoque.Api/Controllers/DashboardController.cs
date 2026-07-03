using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Services;
using System.Threading.Tasks;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public DashboardController(
            PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> ExibirDashboard()
        {
            var dashboard =
                await _pedidoService
                    .ExibirDashboardAsync();

            return Ok(dashboard);
        }
    }
}