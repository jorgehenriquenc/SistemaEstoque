using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Services
{
    public class PedidoDashboardRelatorioServiceTests
    {
        [Fact]
        public async Task ExibirDashboardAsync_DeveRetornarZeros_QuandoNaoHouverDados()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            var resultado = await service.ExibirDashboardAsync();

            Assert.Equal(0, resultado.TotalProdutos);
            Assert.Equal(0, resultado.TotalCategorias);
            Assert.Equal(0, resultado.TotalPedidos);
            Assert.Equal(0, resultado.ProdutosComEstoqueBaixo);
            Assert.Equal(0, resultado.QuantidadeTotalEmEstoque);
            Assert.Equal(0, resultado.ValorTotalEstoque);
        }

        [Fact]
        public async Task ExibirDashboardAsync_DeveContarProdutosCategoriasEPedidos()
        {
            var repository = new PedidoRepositoryFake();

            repository.Categorias.Add(new Categoria { Id = 1, Nome = "Alimentos" });
            repository.Categorias.Add(new Categoria { Id = 2, Nome = "Limpeza" });

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.Produtos.Add(new Produto
            {
                Id = 2,
                Nome = "Sabão",
                Preco = 8m,
                QuantidadeEmEstoque = 5,
                EstoqueMinimo = 1,
                Ativo = true,
                CategoriaId = 2
            });

            repository.Pedidos.Add(new Pedido { Id = 1, DataPedido = DateTime.UtcNow });

            var service = new PedidoService(repository);

            var resultado = await service.ExibirDashboardAsync();

            Assert.Equal(2, resultado.TotalProdutos);
            Assert.Equal(2, resultado.TotalCategorias);
            Assert.Equal(1, resultado.TotalPedidos);
        }

        [Fact]
        public async Task ExibirDashboardAsync_DeveContarProdutosComEstoqueBaixo()
        {
            var repository = new PedidoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25m,
                QuantidadeEmEstoque = 2,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.Produtos.Add(new Produto
            {
                Id = 2,
                Nome = "Feijão",
                Preco = 9m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new PedidoService(repository);

            var resultado = await service.ExibirDashboardAsync();

            Assert.Equal(1, resultado.ProdutosComEstoqueBaixo);
        }

        [Fact]
        public async Task ExibirDashboardAsync_DeveCalcularQuantidadeTotalEmEstoque()
        {
            var repository = new PedidoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.Produtos.Add(new Produto
            {
                Id = 2,
                Nome = "Feijão",
                Preco = 9m,
                QuantidadeEmEstoque = 5,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new PedidoService(repository);

            var resultado = await service.ExibirDashboardAsync();

            Assert.Equal(15, resultado.QuantidadeTotalEmEstoque);
        }

        [Fact]
        public async Task ExibirDashboardAsync_DeveCalcularValorTotalDoEstoque()
        {
            var repository = new PedidoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.Produtos.Add(new Produto
            {
                Id = 2,
                Nome = "Feijão",
                Preco = 9m,
                QuantidadeEmEstoque = 5,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new PedidoService(repository);

            var resultado = await service.ExibirDashboardAsync();

            Assert.Equal(295m, resultado.ValorTotalEstoque);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidosAsync_DeveRetornarListaVazia_QuandoNaoHouverItens()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            var resultado = await service.ListarProdutosMaisVendidosAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidosAsync_DeveAgruparVendasDoMesmoProduto()
        {
            var repository = CriarRepositoryComVendas();
            var service = new PedidoService(repository);

            var resultado = await service.ListarProdutosMaisVendidosAsync();

            var arroz = resultado.First(produto => produto.ProdutoId == 1);

            Assert.Equal(5, arroz.QuantidadeVendida);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidosAsync_DeveOrdenarPorQuantidadeVendidaDecrescente()
        {
            var repository = CriarRepositoryComVendas();
            var service = new PedidoService(repository);

            var resultado = await service.ListarProdutosMaisVendidosAsync();

            Assert.Equal(1, resultado[0].ProdutoId);
            Assert.Equal(2, resultado[1].ProdutoId);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidosAsync_DeveCalcularValorTotalVendido()
        {
            var repository = CriarRepositoryComVendas();
            var service = new PedidoService(repository);

            var resultado = await service.ListarProdutosMaisVendidosAsync();

            var arroz = resultado.First(produto => produto.ProdutoId == 1);

            Assert.Equal(125m, arroz.ValorTotalVendido);
        }

        [Fact]
        public async Task ListarProdutosMaisVendidosAsync_DeveRetornarNomeDoProduto()
        {
            var repository = CriarRepositoryComVendas();
            var service = new PedidoService(repository);

            var resultado = await service.ListarProdutosMaisVendidosAsync();

            Assert.Equal("Arroz", resultado[0].Produto);
        }

        private static PedidoRepositoryFake CriarRepositoryComVendas()
        {
            var repository = new PedidoRepositoryFake();

            var arroz = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            };

            var feijao = new Produto
            {
                Id = 2,
                Nome = "Feijão",
                Preco = 10m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            };

            repository.Produtos.Add(arroz);
            repository.Produtos.Add(feijao);

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
                        Produto = arroz,
                        Quantidade = 2,
                        PrecoUnitario = 25m
                    },
                    new ItemPedido
                    {
                        Id = 2,
                        PedidoId = 1,
                        ProdutoId = 2,
                        Produto = feijao,
                        Quantidade = 3,
                        PrecoUnitario = 10m
                    }
                }
            });

            repository.Pedidos.Add(new Pedido
            {
                Id = 2,
                DataPedido = DateTime.UtcNow,
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        Id = 3,
                        PedidoId = 2,
                        ProdutoId = 1,
                        Produto = arroz,
                        Quantidade = 3,
                        PrecoUnitario = 25m
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
                var pedido = Pedidos.FirstOrDefault(pedido => pedido.Id == id);

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