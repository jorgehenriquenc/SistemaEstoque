using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Services
{
    public class PedidoServiceTests
    {
        [Fact]
        public async Task ListarPedidosAsync_DeveRetornarPedidosMapeados()
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

            repository.Pedidos.Add(new Pedido
            {
                Id = 1,
                DataPedido = new DateTime(2026, 7, 10, 12, 0, 0, DateTimeKind.Utc),
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        Id = 1,
                        ProdutoId = 1,
                        Produto = produto,
                        Quantidade = 2,
                        PrecoUnitario = 25.90m
                    }
                }
            });

            var service = new PedidoService(repository);

            var resultado = await service.ListarPedidosAsync();

            Assert.Single(resultado);
            Assert.Equal(1, resultado[0].Id);
            Assert.Equal(51.80m, resultado[0].TotalPedido);
            Assert.Single(resultado[0].Itens);
            Assert.Equal("Arroz", resultado[0].Itens[0].Produto);
            Assert.Equal("Alimentos", resultado[0].Itens[0].Categoria);
        }

        [Fact]
        public async Task BuscarPedidoPorIdAsync_DeveLancarNotFoundException_QuandoPedidoNaoExistir()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.BuscarPedidoPorIdAsync(999)
            );
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveLancarBusinessValidationException_QuandoPedidoNaoPossuirItens()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>()
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarPedidoAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveLancarNotFoundException_QuandoProdutoNaoExistir()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 999,
                        Quantidade = 1
                    }
                }
            };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.RegistrarPedidoAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveLancarBusinessValidationException_QuandoProdutoEstiverInativo()
        {
            var repository = new PedidoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = false,
                CategoriaId = 1,
                Categoria = new Categoria
                {
                    Id = 1,
                    Nome = "Alimentos"
                }
            });

            var service = new PedidoService(repository);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 1,
                        Quantidade = 1
                    }
                }
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarPedidoAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveLancarBusinessValidationException_QuandoEstoqueForInsuficiente()
        {
            var repository = new PedidoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 2,
                EstoqueMinimo = 1,
                Ativo = true,
                CategoriaId = 1,
                Categoria = new Categoria
                {
                    Id = 1,
                    Nome = "Alimentos"
                }
            });

            var service = new PedidoService(repository);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 1,
                        Quantidade = 3
                    }
                }
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarPedidoAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveRegistrarPedidoValidoEBaixarEstoque()
        {
            var repository = new PedidoRepositoryFake();

            var produto = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = new Categoria
                {
                    Id = 1,
                    Nome = "Alimentos"
                }
            };

            repository.Produtos.Add(produto);

            var service = new PedidoService(repository);

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

            var resultado = await service.RegistrarPedidoAsync(dto);

            Assert.NotNull(repository.PedidoCadastrado);
            Assert.Equal(8, produto.QuantidadeEmEstoque);
            Assert.Equal(51.80m, resultado.TotalPedido);
            Assert.Single(resultado.Itens);
            Assert.Equal("Arroz", resultado.Itens[0].Produto);
        }

        [Fact]
        public async Task RegistrarPedidoAsync_DeveAgruparItensRepetidosDoMesmoProduto()
        {
            var repository = new PedidoRepositoryFake();

            var produto = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = new Categoria
                {
                    Id = 1,
                    Nome = "Alimentos"
                }
            };

            repository.Produtos.Add(produto);

            var service = new PedidoService(repository);

            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 1,
                        Quantidade = 2
                    },
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 1,
                        Quantidade = 3
                    }
                }
            };

            var resultado = await service.RegistrarPedidoAsync(dto);

            Assert.Equal(5, repository.PedidoCadastrado?.Itens[0].Quantidade);
            Assert.Equal(5, produto.QuantidadeEmEstoque);
            Assert.Equal(129.50m, resultado.TotalPedido);
            Assert.Single(resultado.Itens);
        }

        [Fact]
        public async Task CancelarPedidoAsync_DeveLancarNotFoundException_QuandoPedidoNaoExistir()
        {
            var repository = new PedidoRepositoryFake();
            var service = new PedidoService(repository);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CancelarPedidoAsync(999)
            );
        }

        [Fact]
        public async Task CancelarPedidoAsync_DeveDevolverEstoqueERemoverPedido()
        {
            var repository = new PedidoRepositoryFake();

            var produto = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 8,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = new Categoria
                {
                    Id = 1,
                    Nome = "Alimentos"
                }
            };

            var pedido = new Pedido
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
            };

            repository.Produtos.Add(produto);
            repository.Pedidos.Add(pedido);

            var service = new PedidoService(repository);

            await service.CancelarPedidoAsync(1);

            Assert.Equal(10, produto.QuantidadeEmEstoque);
            Assert.Equal(pedido, repository.PedidoRemovido);
            Assert.DoesNotContain(repository.Pedidos, item => item.Id == 1);
        }

        private class PedidoRepositoryFake : IPedidoRepository
        {
            public List<Pedido> Pedidos { get; } = new();

            public List<Produto> Produtos { get; } = new();

            public List<Categoria> Categorias { get; } = new();

            public Pedido? PedidoCadastrado { get; private set; }

            public Pedido? PedidoRemovido { get; private set; }

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

                PedidoCadastrado = pedido;
                Pedidos.Add(pedido);

                return Task.CompletedTask;
            }

            public Task RemoverAsync(Pedido pedido)
            {
                PedidoRemovido = pedido;
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