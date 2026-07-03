using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Services;
using System.Threading.Tasks;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public RelatoriosController(
            PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/Relatorios/produtos-mais-vendidos
        [HttpGet("produtos-mais-vendidos")]
        public async Task<IActionResult>
            ListarProdutosMaisVendidos()
        {
            var produtosMaisVendidos =
                await _pedidoService
                    .ListarProdutosMaisVendidosAsync();

            return Ok(produtosMaisVendidos);
        }
    }
}