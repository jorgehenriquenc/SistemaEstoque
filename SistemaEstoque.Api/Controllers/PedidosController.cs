using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public IActionResult ListarPedidos()
        {
            var pedidos = _pedidoService.ListarPedidos();

            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPedidoPorId(int id)
        {
            var pedido = _pedidoService.BuscarPedidoPorId(id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            return Ok(pedido);
        }

        [HttpPost]
        public IActionResult RegistrarPedido([FromBody] PedidoCreateDto pedidoDto)
        {
            var pedido = _pedidoService.RegistrarPedido(pedidoDto, out string erro);

            if (pedido == null)
            {
                return BadRequest(erro);
            }

            return CreatedAtAction(nameof(BuscarPedidoPorId), new { id = pedido.Id }, new
            {
                pedido.Id,
                pedido.DataPedido
            });
        }

        [HttpDelete("{id}")]
        public IActionResult CancelarPedido(int id)
        {
            bool cancelado = _pedidoService.CancelarPedido(id);

            if (!cancelado)
            {
                return NotFound("Pedido não encontrado.");
            }

            return Ok("Pedido cancelado e estoque devolvido com sucesso.");
        }
    }
}