using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;

namespace SistemaEstoque.Api.Services
{
    // Serviço responsável pelas regras de negócio de produtos.
    public class ProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Retorna todos os produtos cadastrados.
        public async Task<List<ProdutoResponseDto>> ListarProdutosAsync()
        {
            var produtos = await _produtoRepository.ListarTodosAsync();

            return produtos
                .Select(produto => MapearParaResponseDto(produto))
                .ToList();
        }

        // Busca um produto pelo identificador.
        public async Task<ProdutoResponseDto> BuscarProdutoPorIdAsync(int id)
        {
            var produto = await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            return MapearParaResponseDto(produto);
        }

        // Cadastra um novo produto.
        public async Task<ProdutoResponseDto> CadastrarProdutoAsync(
            ProdutoCreateDto produtoDto)
        {
            var nome = produtoDto.Nome.Trim();

            var categoria = await _produtoRepository.BuscarCategoriaPorIdAsync(
                produtoDto.CategoriaId
            );

            if (categoria is null)
            {
                throw new BusinessValidationException("Categoria não encontrada.");
            }

            var produtoDuplicado = await _produtoRepository
                .ProdutoExisteNaCategoriaAsync(
                    nome,
                    produtoDto.CategoriaId
                );

            if (produtoDuplicado)
            {
                throw new ConflictException(
                    "Já existe um produto com esse nome nessa categoria."
                );
            }

            var produto = new Produto
            {
                Nome = nome,
                Preco = produtoDto.Preco,
                QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque,
                EstoqueMinimo = produtoDto.EstoqueMinimo,
                Ativo = true,
                CategoriaId = produtoDto.CategoriaId
            };

            await _produtoRepository.CadastrarAsync(produto);

            return MapearParaResponseDto(produto, categoria.Nome);
        }

        // Atualiza um produto existente.
        public async Task<ProdutoResponseDto> AtualizarProdutoAsync(
            int id,
            ProdutoUpdateDto produtoDto)
        {
            var produto = await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var nome = produtoDto.Nome.Trim();

            var categoria = await _produtoRepository.BuscarCategoriaPorIdAsync(
                produtoDto.CategoriaId
            );

            if (categoria is null)
            {
                throw new BusinessValidationException("Categoria não encontrada.");
            }

            var produtoDuplicado = await _produtoRepository
                .ProdutoExisteNaCategoriaAsync(
                    nome,
                    produtoDto.CategoriaId,
                    id
                );

            if (produtoDuplicado)
            {
                throw new ConflictException(
                    "Já existe outro produto com esse nome nessa categoria."
                );
            }

            produto.Nome = nome;
            produto.Preco = produtoDto.Preco;
            produto.QuantidadeEmEstoque = produtoDto.QuantidadeEmEstoque;
            produto.EstoqueMinimo = produtoDto.EstoqueMinimo;
            produto.Ativo = produtoDto.Ativo;
            produto.CategoriaId = produtoDto.CategoriaId;

            await _produtoRepository.AtualizarAsync(produto);

            return MapearParaResponseDto(produto, categoria.Nome);
        }

        // Remove um produto existente.
        public async Task RemoverProdutoAsync(int id)
        {
            var produto = await _produtoRepository.BuscarPorIdAsync(id);

            if (produto is null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var produtoPossuiPedidos = await _produtoRepository
                .ProdutoPossuiPedidosAsync(id);

            if (produtoPossuiPedidos)
            {
                throw new ConflictException(
                    "Não é possível remover um produto que já foi usado em pedido. Desative o produto em vez de removê-lo."
                );
            }

            await _produtoRepository.RemoverAsync(produto);
        }

        // Converte a entidade Produto em DTO de resposta.
        private static ProdutoResponseDto MapearParaResponseDto(
            Produto produto,
            string? nomeCategoria = null)
        {
            return new ProdutoResponseDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                QuantidadeEmEstoque = produto.QuantidadeEmEstoque,
                EstoqueMinimo = produto.EstoqueMinimo,
                Ativo = produto.Ativo,
                CategoriaId = produto.CategoriaId,
                Categoria = nomeCategoria ?? produto.Categoria?.Nome ?? string.Empty
            };
        }
    }
}