using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    // Define a rota base: api/relatorios
    [Route("api/[controller]")]

    // Indica que esta classe é um Controller de API
    [ApiController]
    public class RelatoriosController : ControllerBase
    {
        // Serviço responsável pelas regras de relatório
        private readonly PedidoService _pedidoService;

        // Construtor que recebe o PedidoService por injeção de dependência
        public RelatoriosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // Endpoint responsável por listar os produtos mais vendidos
        [HttpGet("produtos-mais-vendidos")]
        public IActionResult ListarProdutosMaisVendidos()
        {
            var produtosMaisVendidos = _pedidoService.ListarProdutosMaisVendidos();

            return Ok(produtosMaisVendidos);
        }
    }
}