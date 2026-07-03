using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public IActionResult ListarProdutos()
        {
            var produtos = _produtoService.ListarProdutos();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarProdutoPorId(int id)
        {
            var produto = _produtoService.BuscarProdutoPorId(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok(produto);
        }

        [HttpPost]
        public IActionResult CadastrarProduto(
            [FromBody] ProdutoCreateDto produtoDto)
        {
            bool categoriaExiste =
                _produtoService.CategoriaExiste(produtoDto.CategoriaId);

            if (!categoriaExiste)
            {
                return BadRequest("Categoria não encontrada.");
            }

            var produto =
                _produtoService.CadastrarProduto(produtoDto);

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

        [HttpPut("{id}")]
        public IActionResult AtualizarProduto(
            int id,
            [FromBody] ProdutoUpdateDto produtoDto)
        {
            bool categoriaExiste =
                _produtoService.CategoriaExiste(produtoDto.CategoriaId);

            if (!categoriaExiste)
            {
                return BadRequest("Categoria não encontrada.");
            }

            bool atualizado =
                _produtoService.AtualizarProduto(id, produtoDto);

            if (!atualizado)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok("Produto atualizado com sucesso.");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoverProduto(int id)
        {
            bool removido =
                _produtoService.RemoverProduto(id);

            if (!removido)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok("Produto removido com sucesso.");
        }
    }
}