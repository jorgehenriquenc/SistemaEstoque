using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Data.Context;
using SistemaEstoque.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    // Define a rota base: api/categorias
    [Route("api/[controller]")]

    // Indica que esta classe é um Controller de API
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        // Contexto usado para acessar o banco de dados
        private readonly AppDbContext _context;

        // Construtor que recebe o AppDbContext por injeção de dependência
        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint responsável por listar todas as categorias
        [HttpGet]
        public IActionResult ListarCategorias()
        {
            var categorias = _context.Categorias
                .Select(categoria => new
                {
                    categoria.Id,
                    categoria.Nome
                })
                .ToList();

            return Ok(categorias);
        }

        // Endpoint responsável por buscar uma categoria pelo ID
        [HttpGet("{id}")]
        public IActionResult BuscarCategoriaPorId(int id)
        {
            var categoria = _context.Categorias
                .Where(categoria => categoria.Id == id)
                .Select(categoria => new
                {
                    categoria.Id,
                    categoria.Nome
                })
                .FirstOrDefault();

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            return Ok(categoria);
        }

        // Endpoint responsável por cadastrar uma nova categoria
        [HttpPost]
        public IActionResult CadastrarCategoria([FromBody] CategoriaCreateDto categoriaDto)
        {
            if (string.IsNullOrWhiteSpace(categoriaDto.Nome))
            {
                return BadRequest("O nome da categoria é obrigatório.");
            }

            Categoria categoria = new Categoria();

            categoria.Nome = categoriaDto.Nome.Trim();

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return CreatedAtAction(nameof(BuscarCategoriaPorId), new { id = categoria.Id }, new
            {
                categoria.Id,
                categoria.Nome
            });
        }

        // Endpoint responsável por atualizar uma categoria existente
        [HttpPut("{id}")]
        public IActionResult AtualizarCategoria(int id, [FromBody] CategoriaUpdateDto categoriaDto)
        {
            var categoria = _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            if (string.IsNullOrWhiteSpace(categoriaDto.Nome))
            {
                return BadRequest("O nome da categoria é obrigatório.");
            }

            categoria.Nome = categoriaDto.Nome.Trim();

            _context.SaveChanges();

            return Ok("Categoria atualizada com sucesso.");
        }

        // Endpoint responsável por remover uma categoria existente
        [HttpDelete("{id}")]
        public IActionResult RemoverCategoria(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            var possuiProdutos = _context.Produtos.Any(produto => produto.CategoriaId == id);

            if (possuiProdutos)
            {
                return BadRequest("Não é possível remover uma categoria que possui produtos cadastrados.");
            }

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok("Categoria removida com sucesso.");
        }
    }
}