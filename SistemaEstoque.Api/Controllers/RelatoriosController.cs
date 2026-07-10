using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Relatorios;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public RelatoriosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet("produtos-mais-vendidos")]
        [ProducesResponseType(typeof(List<ProdutoMaisVendidoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProdutoMaisVendidoResponseDto>>> ListarProdutosMaisVendidos()
        {
            var produtosMaisVendidos = await _pedidoService.ListarProdutosMaisVendidosAsync();

            return Ok(produtosMaisVendidos);
        }
    }
}