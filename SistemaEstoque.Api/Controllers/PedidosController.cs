using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Services;
using System.Threading.Tasks;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(
            PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<IActionResult> ListarPedidos()
        {
            var pedidos =
                await _pedidoService.ListarPedidosAsync();

            return Ok(pedidos);
        }

        // GET: api/Pedidos/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPedidoPorId(
            int id)
        {
            var pedido =
                await _pedidoService
                    .BuscarPedidoPorIdAsync(id);

            if (pedido is null)
            {
                return NotFound(
                    "Pedido não encontrado."
                );
            }

            return Ok(pedido);
        }

        // POST: api/Pedidos
        [HttpPost]
        public async Task<IActionResult> RegistrarPedido(
            [FromBody] PedidoCreateDto pedidoDto)
        {
            var resultado =
                await _pedidoService
                    .RegistrarPedidoAsync(pedidoDto);

            if (resultado.Pedido is null)
            {
                return BadRequest(resultado.Erro);
            }

            return CreatedAtAction(
                nameof(BuscarPedidoPorId),
                new { id = resultado.Pedido.Id },
                new
                {
                    resultado.Pedido.Id,
                    resultado.Pedido.DataPedido
                }
            );
        }

        // DELETE: api/Pedidos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var cancelado =
                await _pedidoService
                    .CancelarPedidoAsync(id);

            if (!cancelado)
            {
                return NotFound(
                    "Pedido não encontrado."
                );
            }

            return Ok(
                "Pedido cancelado e estoque devolvido com sucesso."
            );
        }
    }
}