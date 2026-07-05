using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(
            ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        // GET: api/Produtos
        [HttpGet]
        [ProducesResponseType(
            typeof(List<ProdutoResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<
            ActionResult<List<ProdutoResponseDto>>>
            ListarProdutos()
        {
            var produtos =
                await _produtoService.ListarProdutosAsync();

            return Ok(produtos);
        }

        // GET: api/Produtos/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(ProdutoResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoResponseDto>>
            BuscarProdutoPorId(int id)
        {
            var produto =
                await _produtoService
                    .BuscarProdutoPorIdAsync(id);

            return Ok(produto);
        }

        // POST: api/Produtos
        [HttpPost]
        [ProducesResponseType(
            typeof(ProdutoResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProdutoResponseDto>>
            CadastrarProduto(
                [FromBody] ProdutoCreateDto produtoDto)
        {
            var produto =
                await _produtoService
                    .CadastrarProdutoAsync(produtoDto);

            return CreatedAtAction(
                nameof(BuscarProdutoPorId),
                new { id = produto.Id },
                produto
            );
        }

        // PUT: api/Produtos/1
        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(ProdutoResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoResponseDto>>
            AtualizarProduto(
                int id,
                [FromBody] ProdutoUpdateDto produtoDto)
        {
            var produto =
                await _produtoService
                    .AtualizarProdutoAsync(
                        id,
                        produtoDto
                    );

            return Ok(produto);
        }

        // DELETE: api/Produtos/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(
            StatusCodes.Status204NoContent)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverProduto(
            int id)
        {
            await _produtoService
                .RemoverProdutoAsync(id);

            return NoContent();
        }
    }
}