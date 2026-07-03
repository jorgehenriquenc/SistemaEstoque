using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de categorias.
    public class CategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(
            ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // Retorna todas as categorias cadastradas.
        public async Task<List<CategoriaResponseDto>>
            ListarCategoriasAsync()
        {
            var categorias =
                await _categoriaRepository.ListarTodasAsync();

            return categorias
                .Select(MapearParaResponseDto)
                .ToList();
        }

        // Busca uma categoria pelo identificador.
        public async Task<CategoriaResponseDto?>
            BuscarCategoriaPorIdAsync(int id)
        {
            var categoria =
                await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                return null;
            }

            return MapearParaResponseDto(categoria);
        }

        // Cadastra uma nova categoria.
        public async Task<CategoriaResponseDto>
            CadastrarCategoriaAsync(
                CategoriaCreateDto categoriaDto)
        {
            var categoria = new Categoria
            {
                Nome = categoriaDto.Nome.Trim()
            };

            await _categoriaRepository
                .CadastrarAsync(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Atualiza uma categoria existente.
        public async Task<CategoriaResponseDto?>
            AtualizarCategoriaAsync(
                int id,
                CategoriaUpdateDto categoriaDto)
        {
            var categoria =
                await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                return null;
            }

            categoria.Nome = categoriaDto.Nome.Trim();

            await _categoriaRepository
                .AtualizarAsync(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Verifica se existem produtos vinculados à categoria.
        public async Task<bool>
            CategoriaPossuiProdutosAsync(int id)
        {
            return await _categoriaRepository
                .PossuiProdutosAsync(id);
        }

        // Remove uma categoria existente.
        public async Task<bool> RemoverCategoriaAsync(int id)
        {
            var categoria =
                await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                return false;
            }

            await _categoriaRepository
                .RemoverAsync(categoria);

            return true;
        }

        // Converte a entidade Categoria em DTO de resposta.
        private static CategoriaResponseDto MapearParaResponseDto(
            Categoria categoria)
        {
            return new CategoriaResponseDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome
            };
        }
    }
}