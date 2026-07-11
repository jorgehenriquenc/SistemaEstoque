using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Services
{
    public class CategoriaServiceTests
    {
        [Fact]
        public async Task ListarCategoriasAsync_DeveRetornarCategoriasMapeadas()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.Categorias.Add(new Categoria
            {
                Id = 2,
                Nome = "Limpeza"
            });

            var service = new CategoriaService(repository);

            var resultado = await service.ListarCategoriasAsync();

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Alimentos", resultado[0].Nome);
            Assert.Equal("Limpeza", resultado[1].Nome);
        }

        [Fact]
        public async Task BuscarCategoriaPorIdAsync_DeveRetornarCategoria_QuandoExistir()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);

            var resultado = await service.BuscarCategoriaPorIdAsync(1);

            Assert.Equal(1, resultado.Id);
            Assert.Equal("Alimentos", resultado.Nome);
        }

        [Fact]
        public async Task BuscarCategoriaPorIdAsync_DeveLancarNotFoundException_QuandoNaoExistir()
        {
            var repository = new CategoriaRepositoryFake();
            var service = new CategoriaService(repository);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.BuscarCategoriaPorIdAsync(999)
            );
        }

        [Fact]
        public async Task CadastrarCategoriaAsync_DeveCadastrarCategoriaValida()
        {
            var repository = new CategoriaRepositoryFake();
            var service = new CategoriaService(repository);

            var dto = new CategoriaCreateDto
            {
                Nome = "  Alimentos  "
            };

            var resultado = await service.CadastrarCategoriaAsync(dto);

            Assert.NotNull(repository.CategoriaCadastrada);
            Assert.Equal("Alimentos", repository.CategoriaCadastrada.Nome);
            Assert.Equal("Alimentos", resultado.Nome);
        }

        [Fact]
        public async Task CadastrarCategoriaAsync_DeveLancarConflictException_QuandoNomeJaExistir()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);

            var dto = new CategoriaCreateDto
            {
                Nome = "alimentos"
            };

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CadastrarCategoriaAsync(dto)
            );
        }

        [Fact]
        public async Task AtualizarCategoriaAsync_DeveAtualizarCategoriaValida()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);

            var dto = new CategoriaUpdateDto
            {
                Nome = "  Mercearia  "
            };

            var resultado = await service.AtualizarCategoriaAsync(1, dto);

            Assert.NotNull(repository.CategoriaAtualizada);
            Assert.Equal("Mercearia", repository.CategoriaAtualizada.Nome);
            Assert.Equal("Mercearia", resultado.Nome);
        }

        [Fact]
        public async Task AtualizarCategoriaAsync_DeveLancarNotFoundException_QuandoCategoriaNaoExistir()
        {
            var repository = new CategoriaRepositoryFake();
            var service = new CategoriaService(repository);

            var dto = new CategoriaUpdateDto
            {
                Nome = "Mercearia"
            };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.AtualizarCategoriaAsync(999, dto)
            );
        }

        [Fact]
        public async Task AtualizarCategoriaAsync_DeveLancarConflictException_QuandoNomeJaExistirEmOutraCategoria()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.Categorias.Add(new Categoria
            {
                Id = 2,
                Nome = "Limpeza"
            });

            var service = new CategoriaService(repository);

            var dto = new CategoriaUpdateDto
            {
                Nome = "Alimentos"
            };

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.AtualizarCategoriaAsync(2, dto)
            );
        }

        [Fact]
        public async Task RemoverCategoriaAsync_DeveLancarNotFoundException_QuandoCategoriaNaoExistir()
        {
            var repository = new CategoriaRepositoryFake();
            var service = new CategoriaService(repository);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.RemoverCategoriaAsync(999)
            );
        }

        [Fact]
        public async Task RemoverCategoriaAsync_DeveLancarConflictException_QuandoCategoriaPossuirProdutos()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            repository.CategoriasComProdutos.Add(1);

            var service = new CategoriaService(repository);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.RemoverCategoriaAsync(1)
            );
        }

        [Fact]
        public async Task RemoverCategoriaAsync_DeveRemoverCategoria_QuandoNaoPossuirProdutos()
        {
            var repository = new CategoriaRepositoryFake();

            var categoria = new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            };

            repository.Categorias.Add(categoria);

            var service = new CategoriaService(repository);

            await service.RemoverCategoriaAsync(1);

            Assert.Equal(categoria, repository.CategoriaRemovida);
            Assert.DoesNotContain(repository.Categorias, item => item.Id == 1);
        }

        private class CategoriaRepositoryFake : ICategoriaRepository
        {
            public List<Categoria> Categorias { get; } = new();

            public HashSet<int> CategoriasComProdutos { get; } = new();

            public Categoria? CategoriaCadastrada { get; private set; }

            public Categoria? CategoriaAtualizada { get; private set; }

            public Categoria? CategoriaRemovida { get; private set; }

            public Task<List<Categoria>> ListarTodasAsync()
            {
                return Task.FromResult(Categorias);
            }

            public Task<Categoria?> BuscarPorIdAsync(int id)
            {
                var categoria = Categorias.FirstOrDefault(categoria =>
                    categoria.Id == id
                );

                return Task.FromResult(categoria);
            }

            public Task<bool> NomeExisteAsync(string nome, int? idIgnorado = null)
            {
                var nomeNormalizado = nome.Trim().ToLowerInvariant();

                var existe = Categorias.Any(categoria =>
                    categoria.Nome.Trim().ToLowerInvariant() == nomeNormalizado &&
                    (!idIgnorado.HasValue || categoria.Id != idIgnorado.Value)
                );

                return Task.FromResult(existe);
            }

            public Task CadastrarAsync(Categoria categoria)
            {
                if (categoria.Id == 0)
                {
                    categoria.Id = Categorias.Count == 0
                        ? 1
                        : Categorias.Max(item => item.Id) + 1;
                }

                CategoriaCadastrada = categoria;
                Categorias.Add(categoria);

                return Task.CompletedTask;
            }

            public Task AtualizarAsync(Categoria categoria)
            {
                CategoriaAtualizada = categoria;

                return Task.CompletedTask;
            }

            public Task RemoverAsync(Categoria categoria)
            {
                CategoriaRemovida = categoria;
                Categorias.Remove(categoria);

                return Task.CompletedTask;
            }

            public Task<bool> PossuiProdutosAsync(int categoriaId)
            {
                var possuiProdutos = CategoriasComProdutos.Contains(categoriaId);

                return Task.FromResult(possuiProdutos);
            }
        }
    }
}