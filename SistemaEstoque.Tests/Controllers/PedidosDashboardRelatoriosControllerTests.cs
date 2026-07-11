using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Controllers;
using SistemaEstoque.Api.Dtos.Dashboard;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Dtos.Relatorios;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Controllers
{
    public class PedidosDashboardRelatoriosControllerTests
    {
        [Fact]
        public async Task ListarPedidos_DeveRetornarOkComPedidos()
        {
            var repository = CriarRepositoryComPedido();
            var service = new PedidoService(repository);
            var controller = new PedidosController(service);

            var resultado = await controller.ListarPedidos();

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pedidos = Assert.IsType<List<PedidoResponseDto>>(okResult.Value);

            Assert.Single(pedidos);
            Assert.Equal(51.80m, pedidos[0].TotalPedido);
        }

        [Fact]
        public async Task BuscarPedidoPorId_DeveRetornarOkComPedido()
        {
            var repository = CriarRepositoryComPedido();
            var service = new PedidoService(repository);
            var controller = new PedidosController(service);

            var resultado = await controller.BuscarPedidoPorId(1);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pedido = Assert.IsType<PedidoResponseDto>(okResult.Value);

            Assert.Equal(1, pedido.Id);
            Assert.Equal(51.80m, pedido.TotalPedido);
        }

        [Fact]
        public async Task RegistrarPedido_DeveRetornarCreatedAtAction()
        {
            var repository = CriarRepositoryComProduto();
            var service = new PedidoService(repository);
            var controller = new PedidosController(service);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 1,
                        Quantidade = 2
                    }
                }
            };

            var resultado = await controller.RegistrarPedido(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var pedido = Assert.IsType<PedidoResponseDto>(createdResult.Value);

            Assert.Equal(nameof(PedidosController.BuscarPedidoPorId), createdResult.ActionName);
            Assert.Equal(51.80m, pedido.TotalPedido);
            Assert.Equal(8, repository.Produtos[0].QuantidadeEmEstoque);
        }

        [Fact]
        public async Task CancelarPedido_DeveRetornarNoContent()
        {
            var repository = CriarRepositoryComPedido();
            var service = new PedidoService(repository);
            var controller = new PedidosController(service);

            var resultado = await controller.CancelarPedido(1);

            Assert.IsType<NoContentResult>(resultado);
            Assert.Empty(repository.Pedidos);
            Assert.Equal(12, repository.Produtos[0].QuantidadeEmEstoque);
        }

        [Fact]
        public async Task ExibirDashboard_DeveRetornarOkComDashboard()
        {
            var repository = CriarRepositoryComPedido();
            var service = new PedidoService(repository);
            var controller = new DashboardController(service);

            var resultado = await controller.ExibirDashboard();

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var dashboard = Assert.IsType<DashboardResponseDto>(okResult.Value);

            Assert.Equal(repository.Produtos.Count, dashboard.TotalProdutos);
            Assert.Equal(repository.Categorias.Count, dashboard.TotalCategorias);
            Assert.Equal(repository.Pedidos.Count, dashboard.TotalPedidos);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidos_DeveRetornarOkComRelatorio()
        {
            var repository = CriarRepositoryComPedido();
            var service = new PedidoService(repository);
            var controller = new RelatoriosController(service);

            var resultado = await controller.ListarProdutosMaisVendidos();

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var produtos = Assert.IsType<List<ProdutoMaisVendidoResponseDto>>(okResult.Value);

            Assert.Single(produtos);
            Assert.Equal("Arroz", produtos[0].Produto);
            Assert.Equal(2, produtos[0].QuantidadeVendida);
            Assert.Equal(51.80m, produtos[0].ValorTotalVendido);
        }

        private static PedidoRepositoryFake CriarRepositoryComProduto()
        {
            var repository = new PedidoRepositoryFake();

            var categoria = new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            };

            var produto = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = categoria
            };

            repository.Categorias.Add(categoria);
            repository.Produtos.Add(produto);

            return repository;
        }

        private static PedidoRepositoryFake CriarRepositoryComPedido()
        {
            var repository = CriarRepositoryComProduto();

            var produto = repository.Produtos[0];

            repository.Pedidos.Add(new Pedido
            {
                Id = 1,
                DataPedido = DateTime.UtcNow,
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        Id = 1,
                        PedidoId = 1,
                        ProdutoId = 1,
                        Produto = produto,
                        Quantidade = 2,
                        PrecoUnitario = 25.90m
                    }
                }
            });

            return repository;
        }

        private class PedidoRepositoryFake : IPedidoRepository
        {
            public List<Pedido> Pedidos { get; } = new();

            public List<Produto> Produtos { get; } = new();

            public List<Categoria> Categorias { get; } = new();

            public Task<List<Pedido>> ListarTodosAsync()
            {
                return Task.FromResult(Pedidos);
            }

            public Task<Pedido?> BuscarPorIdAsync(int id)
            {
                var pedido = Pedidos.FirstOrDefault(pedido =>
                    pedido.Id == id
                );

                return Task.FromResult(pedido);
            }

            public Task<Produto?> BuscarProdutoPorIdAsync(int produtoId)
            {
                var produto = Produtos.FirstOrDefault(produto =>
                    produto.Id == produtoId
                );

                return Task.FromResult(produto);
            }

            public Task CadastrarAsync(Pedido pedido)
            {
                if (pedido.Id == 0)
                {
                    pedido.Id = Pedidos.Count == 0
                        ? 1
                        : Pedidos.Max(item => item.Id) + 1;
                }

                for (var i = 0; i < pedido.Itens.Count; i++)
                {
                    var item = pedido.Itens[i];

                    if (item.Id == 0)
                    {
                        item.Id = i + 1;
                    }

                    item.PedidoId = pedido.Id;

                    var produto = Produtos.First(produto =>
                        produto.Id == item.ProdutoId
                    );

                    item.Produto = produto;
                }

                Pedidos.Add(pedido);

                return Task.CompletedTask;
            }

            public Task RemoverAsync(Pedido pedido)
            {
                Pedidos.Remove(pedido);

                return Task.CompletedTask;
            }

            public Task<List<ItemPedido>> ListarItensPedidoAsync()
            {
                var itens = Pedidos
                    .SelectMany(pedido => pedido.Itens)
                    .ToList();

                return Task.FromResult(itens);
            }

            public Task<List<Produto>> ListarProdutosAsync()
            {
                return Task.FromResult(Produtos);
            }

            public Task<int> ContarCategoriasAsync()
            {
                return Task.FromResult(Categorias.Count);
            }

            public Task<int> ContarPedidosAsync()
            {
                return Task.FromResult(Pedidos.Count);
            }
        }
    }
}