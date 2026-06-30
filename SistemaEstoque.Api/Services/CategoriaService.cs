using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de categorias.
    public class CategoriaService
    {
        // Repositório utilizado para acessar os dados de categorias.
        private readonly ICategoriaRepository _categoriaRepository;

        // O repositório é recebido por injeção de dependência.
        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // Retorna todas as categorias cadastradas.
        public List<CategoriaResponseDto> ListarCategorias()
        {
            return _categoriaRepository
                .ListarTodas()
                .Select(MapearParaResponseDto)
                .ToList();
        }

        // Busca uma categoria pelo identificador.
        public CategoriaResponseDto? BuscarCategoriaPorId(int id)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria is null)
            {
                return null;
            }

            return MapearParaResponseDto(categoria);
        }

        // Cadastra uma nova categoria.
        public CategoriaResponseDto CadastrarCategoria(
            CategoriaCreateDto categoriaDto)
        {
            var categoria = new Categoria
            {
                Nome = categoriaDto.Nome.Trim()
            };

            _categoriaRepository.Cadastrar(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Atualiza uma categoria existente.
        public CategoriaResponseDto? AtualizarCategoria(
            int id,
            CategoriaUpdateDto categoriaDto)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria is null)
            {
                return null;
            }

            categoria.Nome = categoriaDto.Nome.Trim();

            _categoriaRepository.Atualizar(categoria);

            return MapearParaResponseDto(categoria);
        }

        // Verifica se existem produtos vinculados à categoria.
        public bool CategoriaPossuiProdutos(int id)
        {
            return _categoriaRepository.PossuiProdutos(id);
        }

        // Remove uma categoria existente.
        public bool RemoverCategoria(int id)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria is null)
            {
                return false;
            }

            _categoriaRepository.Remover(categoria);

            return true;
        }

        // Converte uma entidade Categoria em um DTO de resposta.
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