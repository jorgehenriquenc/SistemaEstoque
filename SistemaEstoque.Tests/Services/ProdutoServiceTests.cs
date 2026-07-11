using SistemaEstoque.Api.Dtos.Produtos;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Services
{
    public class ProdutoServiceTests
    {
        [Fact]
        public async Task ListarProdutosAsync_DeveRetornarProdutosMapeados()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1,
                Categoria = repository.Categorias[0]
            });

            var service = new ProdutoService(repository);

            var resultado = await service.ListarProdutosAsync();

            Assert.Single(resultado);
            Assert.Equal("Arroz", resultado[0].Nome);
            Assert.Equal("Alimentos", resultado[0].Categoria);
        }

        [Fact]
        public async Task BuscarProdutoPorIdAsync_DeveLancarNotFoundException_QuandoProdutoNaoExistir()
        {
            var repository = new ProdutoRepositoryFake();
            var service = new ProdutoService(repository);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.BuscarProdutoPorIdAsync(999)
            );
        }

        [Fact]
        public async Task CadastrarProdutoAsync_DeveLancarBusinessValidationException_QuandoCategoriaNaoExistir()
        {
            var repository = new ProdutoRepositoryFake();
            var service = new ProdutoService(repository);

            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 999
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.CadastrarProdutoAsync(dto)
            );
        }

        [Fact]
        public async Task CadastrarProdutoAsync_DeveLancarConflictException_QuandoProdutoDuplicadoNaMesmaCategoria()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new ProdutoService(repository);

            var dto = new ProdutoCreateDto
            {
                Nome = "arroz",
                Preco = 30.00m,
                QuantidadeEmEstoque = 5,
                EstoqueMinimo = 1,
                CategoriaId = 1
            };

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CadastrarProdutoAsync(dto)
            );
        }

        [Fact]
        public async Task CadastrarProdutoAsync_DeveCadastrarProdutoValido()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new ProdutoService(repository);

            var dto = new ProdutoCreateDto
            {
                Nome = "  Arroz  ",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var resultado = await service.CadastrarProdutoAsync(dto);

            Assert.NotNull(repository.ProdutoCadastrado);
            Assert.Equal("Arroz", repository.ProdutoCadastrado.Nome);
            Assert.True(repository.ProdutoCadastrado.Ativo);
            Assert.Equal("Arroz", resultado.Nome);
            Assert.Equal("Alimentos", resultado.Categoria);
        }

        [Fact]
        public async Task AtualizarProdutoAsync_DeveLancarNotFoundException_QuandoProdutoNaoExistir()
        {
            var repository = new ProdutoRepositoryFake();
            var service = new ProdutoService(repository);

            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.AtualizarProdutoAsync(999, dto)
            );
        }

        [Fact]
        public async Task AtualizarProdutoAsync_DeveLancarBusinessValidationException_QuandoCategoriaNaoExistir()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new ProdutoService(repository);

            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 999
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.AtualizarProdutoAsync(1, dto)
            );
        }

        [Fact]
        public async Task AtualizarProdutoAsync_DeveLancarConflictException_QuandoProdutoDuplicadoNaMesmaCategoria()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.Produtos.Add(new Produto
            {
                Id = 2,
                Nome = "Feijão",
                Preco = 9.90m,
                QuantidadeEmEstoque = 20,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            });

            var service = new ProdutoService(repository);

            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz",
                Preco = 10.90m,
                QuantidadeEmEstoque = 30,
                EstoqueMinimo = 5,
                Ativo = true,
                CategoriaId = 1
            };

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.AtualizarProdutoAsync(2, dto)
            );
        }

        [Fact]
        public async Task RemoverProdutoAsync_DeveLancarConflictException_QuandoProdutoPossuirPedidos()
        {
            var repository = new ProdutoRepositoryFake();

            repository.Produtos.Add(new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            });

            repository.ProdutosComPedidos.Add(1);

            var service = new ProdutoService(repository);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.RemoverProdutoAsync(1)
            );
        }

        [Fact]
        public async Task RemoverProdutoAsync_DeveRemoverProduto_QuandoProdutoNaoPossuirPedidos()
        {
            var repository = new ProdutoRepositoryFake();

            var produto = new Produto
            {
                Id = 1,
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                Ativo = true,
                CategoriaId = 1
            };

            repository.Produtos.Add(produto);

            var service = new ProdutoService(repository);

            await service.RemoverProdutoAsync(1);

            Assert.Equal(produto, repository.ProdutoRemovido);
            Assert.DoesNotContain(repository.Produtos, item => item.Id == 1);
        }

        private class ProdutoRepositoryFake : IProdutoRepository
        {
            public List<Produto> Produtos { get; } = new();

            public List<Categoria> Categorias { get; } = new();

            public HashSet<int> ProdutosComPedidos { get; } = new();

            public Produto? ProdutoCadastrado { get; private set; }

            public Produto? ProdutoAtualizado { get; private set; }

            public Produto? ProdutoRemovido { get; private set; }

            public Task<List<Produto>> ListarTodosAsync()
            {
                return Task.FromResult(Produtos);
            }

            public Task<Produto?> BuscarPorIdAsync(int id)
            {
                var produto = Produtos.FirstOrDefault(produto => produto.Id == id);

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

                ProdutoCadastrado = produto;
                Produtos.Add(produto);

                return Task.CompletedTask;
            }

            public Task AtualizarAsync(Produto produto)
            {
                ProdutoAtualizado = produto;

                return Task.CompletedTask;
            }

            public Task RemoverAsync(Produto produto)
            {
                ProdutoRemovido = produto;
                Produtos.Remove(produto);

                return Task.CompletedTask;
            }
        }
    }
}