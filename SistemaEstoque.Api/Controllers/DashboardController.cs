using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Dashboard;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public DashboardController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardResponseDto>> ExibirDashboard()
        {
            var dashboard = await _pedidoService.ExibirDashboardAsync();

            return Ok(dashboard);
        }
    }
}