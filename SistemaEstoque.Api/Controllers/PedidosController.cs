using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PedidoResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PedidoResponseDto>>> ListarPedidos()
        {
            var pedidos = await _pedidoService.ListarPedidosAsync();

            return Ok(pedidos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoResponseDto>> BuscarPedidoPorId(int id)
        {
            var pedido = await _pedidoService.BuscarPedidoPorIdAsync(id);

            return Ok(pedido);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PedidoResponseDto>> RegistrarPedido(
            [FromBody] PedidoCreateDto pedidoDto)
        {
            var pedido = await _pedidoService.RegistrarPedidoAsync(pedidoDto);

            return CreatedAtAction(
                nameof(BuscarPedidoPorId),
                new { id = pedido.Id },
                pedido
            );
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            await _pedidoService.CancelarPedidoAsync(id);

            return NoContent();
        }
    }
}