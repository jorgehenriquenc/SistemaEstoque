using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de produtos
    public class ProdutoService
    {
        // Repositório usado para acessar os dados de produtos
        private readonly IProdutoRepository _produtoRepository;

        // Construtor que recebe o repositório por injeção de dependência
        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Método responsável por listar todos os produtos
        public object ListarProdutos()
        {
            var produtos = _produtoRepository.ListarTodos()
                .Select(produto => new
                {
                    produto.Id,
                    produto.Nome,
                    produto.Preco,
                    produto.QuantidadeEmEstoque,
                    produto.EstoqueMinimo,
                    produto.Ativo,
                    produto.CategoriaId,
                    Categoria = produto.Categoria.Nome
                })
                .ToList();

            return produtos;
        }

        // Método responsável por buscar um produto pelo ID
        public object BuscarProdutoPorId(int id)
        {
            var produto = _produtoRepository.BuscarPorId(id);

            if (produto == null)
            {
                return null;
            }

            return new
            {
                produto.Id,
                produto.Nome,
                produto.Preco,
                produto.QuantidadeEmEstoque,
                produto.EstoqueMinimo,
                produto.Ativo,
                produto.CategoriaId,
                Categoria = produto.Categoria.Nome
            };
        }

        // Método responsável por verificar se uma categoria existe
        public bool CategoriaExiste(int categoriaId)
        {
            return _produtoRepository.CategoriaExiste(categoriaId);
        }

        // Método responsável por cadastrar um novo produto
        public Produto CadastrarProduto(ProdutoCreateDto produtoDto)
        {
            Produto produto = new Produto
            {
                Nome = produtoDto.Nome.Trim(),
                Preco = produtoDto.Preco,
                QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque,
                EstoqueMinimo = produtoDto.EstoqueMinimo,
                Ativo = true,
                CategoriaId = produtoDto.CategoriaId
            };

            _produtoRepository.Cadastrar(produto);

            return produto;
        }

        // Método responsável por atualizar um produto existente
        public bool AtualizarProduto(int id, ProdutoUpdateDto produtoDto)
        {
            Produto produto = _produtoRepository.BuscarPorId(id);

            if (produto == null)
            {
                return false;
            }

            produto.Nome = produtoDto.Nome.Trim();
            produto.Preco = produtoDto.Preco;
            produto.QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque;
            produto.EstoqueMinimo = produtoDto.EstoqueMinimo;
            produto.Ativo = produtoDto.Ativo;
            produto.CategoriaId = produtoDto.CategoriaId;

            _produtoRepository.Atualizar(produto);

            return true;
        }

        // Método responsável por remover um produto existente
        public bool RemoverProduto(int id)
        {
            Produto produto = _produtoRepository.BuscarPorId(id);

            if (produto == null)
            {
                return false;
            }

            _produtoRepository.Remover(produto);

            return true;
        }
    }
}