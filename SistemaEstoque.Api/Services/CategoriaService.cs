using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de categorias
    public class CategoriaService
    {
        // Repositório usado para acessar os dados de categorias
        private readonly ICategoriaRepository _categoriaRepository;

        // Construtor que recebe o repositório por injeção de dependência
        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // Método responsável por listar todas as categorias
        public object ListarCategorias()
        {
            var categorias = _categoriaRepository.ListarTodas()
                .Select(categoria => new
                {
                    categoria.Id,
                    categoria.Nome
                })
                .ToList();

            return categorias;
        }

        // Método responsável por buscar uma categoria pelo ID
        public object BuscarCategoriaPorId(int id)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria == null)
            {
                return null;
            }

            return new
            {
                categoria.Id,
                categoria.Nome
            };
        }

        // Método responsável por cadastrar uma nova categoria
        public Categoria CadastrarCategoria(CategoriaCreateDto categoriaDto)
        {
            Categoria categoria = new Categoria();

            categoria.Nome = categoriaDto.Nome.Trim();

            _categoriaRepository.Cadastrar(categoria);

            return categoria;
        }

        // Método responsável por atualizar uma categoria existente
        public bool AtualizarCategoria(int id, CategoriaUpdateDto categoriaDto)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria == null)
            {
                return false;
            }

            categoria.Nome = categoriaDto.Nome.Trim();

            _categoriaRepository.Atualizar(categoria);

            return true;
        }

        // Método responsável por verificar se uma categoria possui produtos cadastrados
        public bool CategoriaPossuiProdutos(int id)
        {
            return _categoriaRepository.PossuiProdutos(id);
        }

        // Método responsável por remover uma categoria existente
        public bool RemoverCategoria(int id)
        {
            var categoria = _categoriaRepository.BuscarPorId(id);

            if (categoria == null)
            {
                return false;
            }

            _categoriaRepository.Remover(categoria);

            return true;
        }
    }
}