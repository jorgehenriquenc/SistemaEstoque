using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Services;

namespace SistemaEstoque.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategoriasController(
            CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: api/Categorias
        [HttpGet]
        [ProducesResponseType(
            typeof(List<CategoriaResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaResponseDto>>>
            ListarCategorias()
        {
            var categorias =
                await _categoriaService.ListarCategoriasAsync();

            return Ok(categorias);
        }

        // GET: api/Categorias/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponseDto>>
            BuscarCategoriaPorId(int id)
        {
            var categoria =
                await _categoriaService
                    .BuscarCategoriaPorIdAsync(id);

            if (categoria is null)
            {
                return NotFound(
                    "Categoria não encontrada."
                );
            }

            return Ok(categoria);
        }

        // POST: api/Categorias
        [HttpPost]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaResponseDto>>
            CadastrarCategoria(
                [FromBody] CategoriaCreateDto categoriaDto)
        {
            var categoria =
                await _categoriaService
                    .CadastrarCategoriaAsync(categoriaDto);

            return CreatedAtAction(
                nameof(BuscarCategoriaPorId),
                new { id = categoria.Id },
                categoria
            );
        }

        // PUT: api/Categorias/1
        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponseDto>>
            AtualizarCategoria(
                int id,
                [FromBody] CategoriaUpdateDto categoriaDto)
        {
            var categoria =
                await _categoriaService
                    .AtualizarCategoriaAsync(
                        id,
                        categoriaDto
                    );

            if (categoria is null)
            {
                return NotFound(
                    "Categoria não encontrada."
                );
            }

            return Ok(categoria);
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoverCategoria(int id)
        {
            var categoria =
                await _categoriaService
                    .BuscarCategoriaPorIdAsync(id);

            if (categoria is null)
            {
                return NotFound(
                    "Categoria não encontrada."
                );
            }

            var possuiProdutos =
                await _categoriaService
                    .CategoriaPossuiProdutosAsync(id);

            if (possuiProdutos)
            {
                return Conflict(
                    "Não é possível remover uma categoria que possui produtos cadastrados."
                );
            }

            await _categoriaService
                .RemoverCategoriaAsync(id);

            return NoContent();
        }
    }
}