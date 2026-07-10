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

        public CategoriasController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: api/Categorias
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaResponseDto>>> ListarCategorias()
        {
            var categorias = await _categoriaService.ListarCategoriasAsync();

            return Ok(categorias);
        }

        // GET: api/Categorias/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponseDto>> BuscarCategoriaPorId(
            int id)
        {
            var categoria = await _categoriaService.BuscarCategoriaPorIdAsync(id);

            return Ok(categoria);
        }

        // POST: api/Categorias
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoriaResponseDto>> CadastrarCategoria(
            [FromBody] CategoriaCreateDto categoriaDto)
        {
            var categoria = await _categoriaService.CadastrarCategoriaAsync(
                categoriaDto
            );

            return CreatedAtAction(
                nameof(BuscarCategoriaPorId),
                new { id = categoria.Id },
                categoria
            );
        }

        // PUT: api/Categorias/1
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoriaResponseDto>> AtualizarCategoria(
            int id,
            [FromBody] CategoriaUpdateDto categoriaDto)
        {
            var categoria = await _categoriaService.AtualizarCategoriaAsync(
                id,
                categoriaDto
            );

            return Ok(categoria);
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoverCategoria(int id)
        {
            await _categoriaService.RemoverCategoriaAsync(id);

            return NoContent();
        }
    }
}