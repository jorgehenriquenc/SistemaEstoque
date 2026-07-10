using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de categorias.
    public class CategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // Retorna todas as categorias cadastradas.
        public async Task<List<CategoriaResponseDto>> ListarCategoriasAsync()
        {
            var categorias = await _categoriaRepository.ListarTodasAsync();

            return categorias
                .Select(MapearParaResponseDto)
                .ToList();
        }

        // Busca uma categoria pelo identificador.
        public async Task<CategoriaResponseDto> BuscarCategoriaPorIdAsync(int id)
        {
            var categoria = await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            return MapearParaResponseDto(categoria);
        }

        // Cadastra uma nova categoria.
        public async Task<CategoriaResponseDto> CadastrarCategoriaAsync(
            CategoriaCreateDto categoriaDto)
        {
            var nome = categoriaDto.Nome.Trim();

            var nomeExiste = await _categoriaRepository.NomeExisteAsync(nome);

            if (nomeExiste)
            {
                throw new ConflictException("Já existe uma categoria com esse nome.");
            }

            var categoria = new Categoria
            {
                Nome = nome
            };

            await _categoriaRepository.CadastrarAsync(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Atualiza uma categoria existente.
        public async Task<CategoriaResponseDto> AtualizarCategoriaAsync(
            int id,
            CategoriaUpdateDto categoriaDto)
        {
            var categoria = await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            var nome = categoriaDto.Nome.Trim();

            var nomeExiste = await _categoriaRepository.NomeExisteAsync(
                nome,
                id
            );

            if (nomeExiste)
            {
                throw new ConflictException("Já existe outra categoria com esse nome.");
            }

            categoria.Nome = nome;

            await _categoriaRepository.AtualizarAsync(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Remove uma categoria existente.
        public async Task RemoverCategoriaAsync(int id)
        {
            var categoria = await _categoriaRepository.BuscarPorIdAsync(id);

            if (categoria is null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            var possuiProdutos = await _categoriaRepository.PossuiProdutosAsync(id);

            if (possuiProdutos)
            {
                throw new ConflictException(
                    "Não é possível remover uma categoria que possui produtos cadastrados."
                );
            }

            await _categoriaRepository.RemoverAsync(categoria);
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