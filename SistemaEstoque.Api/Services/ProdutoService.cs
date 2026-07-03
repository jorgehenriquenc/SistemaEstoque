using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de produtos.
    public class ProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(
            IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Retorna todos os produtos cadastrados.
        public async Task<object> ListarProdutosAsync()
        {
            var produtos =
                await _produtoRepository.ListarTodosAsync();

            var resposta = produtos
                .Select(produto => new
                {
                    produto.Id,
                    produto.Nome,
                    produto.Preco,
                    produto.QuantidadeEmEstoque,
                    produto.EstoqueMinimo,
                    produto.Ativo,
                    produto.CategoriaId,
                    Categoria = produto.Categoria?.Nome
                })
                .ToList();

            return resposta;
        }

        // Busca um produto pelo identificador.
        public async Task<object?> BuscarProdutoPorIdAsync(
            int id)
        {
            var produto =
                await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
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
                Categoria = produto.Categoria?.Nome
            };
        }

        // Verifica se uma categoria existe.
        public async Task<bool> CategoriaExisteAsync(
            int categoriaId)
        {
            return await _produtoRepository
                .CategoriaExisteAsync(categoriaId);
        }

        // Cadastra um novo produto.
        public async Task<Produto> CadastrarProdutoAsync(
            ProdutoCreateDto produtoDto)
        {
            var produto = new Produto
            {
                Nome = produtoDto.Nome.Trim(),
                Preco = produtoDto.Preco,
                QuantidadeEmEstoque =
                    produtoDto.QuantidadeEmEstoque,
                EstoqueMinimo =
                    produtoDto.EstoqueMinimo,
                Ativo = true,
                CategoriaId =
                    produtoDto.CategoriaId
            };

            await _produtoRepository
                .CadastrarAsync(produto);

            return produto;
        }

        // Atualiza um produto existente.
        public async Task<bool> AtualizarProdutoAsync(
            int id,
            ProdutoUpdateDto produtoDto)
        {
            var produto =
                await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
            {
                return false;
            }

            produto.Nome = produtoDto.Nome.Trim();
            produto.Preco = produtoDto.Preco;
            produto.QuantidadeEmEstoque =
                produtoDto.QuantidadeEmEstoque;
            produto.EstoqueMinimo =
                produtoDto.EstoqueMinimo;
            produto.Ativo = produtoDto.Ativo;
            produto.CategoriaId =
                produtoDto.CategoriaId;

            await _produtoRepository
                .AtualizarAsync(produto);

            return true;
        }

        // Remove um produto existente.
        public async Task<bool> RemoverProdutoAsync(int id)
        {
            var produto =
                await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
            {
                return false;
            }

            await _produtoRepository
                .RemoverAsync(produto);

            return true;
        }
    }
}