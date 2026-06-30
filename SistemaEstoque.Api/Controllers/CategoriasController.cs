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
        [ProducesResponseType(
            typeof(List<CategoriaResponseDto>),
            StatusCodes.Status200OK)]
        public ActionResult<List<CategoriaResponseDto>> ListarCategorias()
        {
            var categorias = _categoriaService.ListarCategorias();

            return Ok(categorias);
        }

        // GET: api/Categorias/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CategoriaResponseDto> BuscarCategoriaPorId(int id)
        {
            var categoria = _categoriaService.BuscarCategoriaPorId(id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrada.");
            }

            return Ok(categoria);
        }

        // POST: api/Categorias
        [HttpPost]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CategoriaResponseDto> CadastrarCategoria(
            [FromBody] CategoriaCreateDto categoriaDto)
        {
            if (string.IsNullOrWhiteSpace(categoriaDto.Nome))
            {
                return BadRequest(
                    "O nome da categoria é obrigatório.");
            }

            var categoria =
                _categoriaService.CadastrarCategoria(categoriaDto);

            return CreatedAtAction(
                nameof(BuscarCategoriaPorId),
                new { id = categoria.Id },
                categoria);
        }

        // PUT: api/Categorias/1
        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(CategoriaResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CategoriaResponseDto> AtualizarCategoria(
            int id,
            [FromBody] CategoriaUpdateDto categoriaDto)
        {
            if (string.IsNullOrWhiteSpace(categoriaDto.Nome))
            {
                return BadRequest(
                    "O nome da categoria é obrigatório.");
            }

            var categoria =
                _categoriaService.AtualizarCategoria(id, categoriaDto);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrada.");
            }

            return Ok(categoria);
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult RemoverCategoria(int id)
        {
            var categoria =
                _categoriaService.BuscarCategoriaPorId(id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrada.");
            }

            if (_categoriaService.CategoriaPossuiProdutos(id))
            {
                return Conflict(
                    "Não é possível remover uma categoria que possui produtos cadastrados.");
            }

            _categoriaService.RemoverCategoria(id);

            return NoContent();
        }
    }
}