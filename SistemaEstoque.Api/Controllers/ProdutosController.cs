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
        public async Task<IActionResult> ListarProdutos()
        {
            var produtos =
                await _produtoService.ListarProdutosAsync();

            return Ok(produtos);
        }

        // GET: api/Produtos/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarProdutoPorId(
            int id)
        {
            var produto =
                await _produtoService
                    .BuscarProdutoPorIdAsync(id);

            if (produto is null)
            {
                return NotFound(
                    "Produto não encontrado."
                );
            }

            return Ok(produto);
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<IActionResult> CadastrarProduto(
            [FromBody] ProdutoCreateDto produtoDto)
        {
            var categoriaExiste =
                await _produtoService
                    .CategoriaExisteAsync(
                        produtoDto.CategoriaId
                    );

            if (!categoriaExiste)
            {
                return BadRequest(
                    "Categoria não encontrada."
                );
            }

            var produto =
                await _produtoService
                    .CadastrarProdutoAsync(produtoDto);

            return CreatedAtAction(
                nameof(BuscarProdutoPorId),
                new { id = produto.Id },
                new
                {
                    produto.Id,
                    produto.Nome,
                    produto.Preco,
                    produto.QuantidadeEmEstoque,
                    produto.EstoqueMinimo,
                    produto.Ativo,
                    produto.CategoriaId
                }
            );
        }

        // PUT: api/Produtos/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarProduto(
            int id,
            [FromBody] ProdutoUpdateDto produtoDto)
        {
            var categoriaExiste =
                await _produtoService
                    .CategoriaExisteAsync(
                        produtoDto.CategoriaId
                    );

            if (!categoriaExiste)
            {
                return BadRequest(
                    "Categoria não encontrada."
                );
            }

            var atualizado =
                await _produtoService
                    .AtualizarProdutoAsync(
                        id,
                        produtoDto
                    );

            if (!atualizado)
            {
                return NotFound(
                    "Produto não encontrado."
                );
            }

            return Ok(
                "Produto atualizado com sucesso."
            );
        }

        // DELETE: api/Produtos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoverProduto(int id)
        {
            var removido =
                await _produtoService
                    .RemoverProdutoAsync(id);

            if (!removido)
            {
                return NotFound(
                    "Produto não encontrado."
                );
            }

            return Ok(
                "Produto removido com sucesso."
            );
        }
    }
}