using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Controllers;
using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Controllers
{
    public class ProdutosControllerTests
    {
        [Fact]
        public async Task ListarProdutos_DeveRetornarOkComProdutos()
        {
            var repository = CriarRepositoryComCategoriaEProduto();
            var service = new ProdutoService(repository);
            var controller = new ProdutosController(service);

            var resultado = await controller.ListarProdutos();

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var produtos = Assert.IsType<List<ProdutoResponseDto>>(okResult.Value);

            Assert.Single(produtos);
            Assert.Equal("Arroz", produtos[0].Nome);
            Assert.Equal("Alimentos", produtos[0].Categoria);
        }

        [Fact]
        public async Task BuscarProdutoPorId_DeveRetornarOkComProduto()
        {
            var repository = CriarRepositoryComCategoriaEProduto();
            var service = new ProdutoService(repository);
            var controller = new ProdutosController(service);

            var resultado = await controller.BuscarProdutoPorId(1);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var produto = Assert.IsType<ProdutoResponseDto>(okResult.Value);

            Assert.Equal(1, produto.Id);
            Assert.Equal("Arroz", produto.Nome);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarCreatedAtAction()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new ProdutoService(repository);
            var controller = new ProdutosController(service);

            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var resultado = await controller.CadastrarProduto(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var produto = Assert.IsType<ProdutoResponseDto>(createdResult.Value);

            Assert.Equal(nameof(ProdutosController.BuscarProdutoPorId), createdResult.ActionName);
            Assert.Equal("Arroz", produto.Nome);
            Assert.True(produto.Ativo);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarOkComProdutoAtualizado()
        {
            var repository = CriarRepositoryComCategoriaEProduto();
            var service = new ProdutoService(repository);
            var controller = new ProdutosController(service);

            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var resultado = await controller.AtualizarProduto(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var produto = Assert.IsType<ProdutoResponseDto>(okResult.Value);

            Assert.Equal("Arroz Integral", produto.Nome);
            Assert.Equal(30.00m, produto.Preco);
            Assert.Equal(15, produto.QuantidadeEmEstoque);
        }

        [Fact]
        public async Task RemoverProduto_DeveRetornarNoContent()
        {
            var repository = CriarRepositoryComCategoriaEProduto();
            var service = new ProdutoService(repository);
            var controller = new ProdutosController(service);

            var resultado = await controller.RemoverProduto(1);

            Assert.IsType<NoContentResult>(resultado);
            Assert.Empty(repository.Produtos);
        }

        private static ProdutoRepositoryFake CriarRepositoryComCategoriaEProduto()
        {
            var repository = new ProdutoRepositoryFake();

            var categoria = new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            };

            repository.Categorias.Add(categoria);

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = categoria
            });

            return repository;
        }

        private class ProdutoRepositoryFake : IProdutoRepository
        {
            public List<Produto> Produtos { get; } = new();

            public List<Categoria> Categorias { get; } = new();

            public HashSet<int> ProdutosComPedidos { get; } = new();

            public Task<List<Produto>> ListarTodosAsync()
            {
                return Task.FromResult(Produtos);
            }

            public Task<Produto?> BuscarPorIdAsync(int id)
            {
                var produto = Produtos.FirstOrDefault(produto =>
                    produto.Id == id
                );

                return Task.FromResult(produto);
            }

            public Task<Categoria?> BuscarCategoriaPorIdAsync(int categoriaId)
            {
                var categoria = Categorias.FirstOrDefault(categoria =>
                    categoria.Id == categoriaId
                );

                return Task.FromResult(categoria);
            }

            public Task<bool> ProdutoExisteNaCategoriaAsync(
                string nome,
                int categoriaId,
                int? idIgnorado = null)
            {
                var nomeNormalizado = nome.Trim().ToLowerInvariant();

                var existe = Produtos.Any(produto =>
                    produto.Nome.Trim().ToLowerInvariant() == nomeNormalizado &&
                    produto.CategoriaId == categoriaId &&
                    (!idIgnorado.HasValue || produto.Id != idIgnorado.Value)
                );

                return Task.FromResult(existe);
            }

            public Task<bool> ProdutoPossuiPedidosAsync(int produtoId)
            {
                var possuiPedidos = ProdutosComPedidos.Contains(produtoId);

                return Task.FromResult(possuiPedidos);
            }

            public Task CadastrarAsync(Produto produto)
            {
                if (produto.Id == 0)
                {
                    produto.Id = Produtos.Count == 0
                        ? 1
                        : Produtos.Max(item => item.Id) + 1;
                }

                var categoria = Categorias.First(categoria =>
                    categoria.Id == produto.CategoriaId
                );

                produto.Categoria = categoria;

                Produtos.Add(produto);

                return Task.CompletedTask;
            }

            public Task AtualizarAsync(Produto produto)
            {
                var categoria = Categorias.First(categoria =>
                    categoria.Id == produto.CategoriaId
                );

                produto.Categoria = categoria;

                return Task.CompletedTask;
            }

            public Task RemoverAsync(Produto produto)
            {
                Produtos.Remove(produto);

                return Task.CompletedTask;
            }
        }
    }
}