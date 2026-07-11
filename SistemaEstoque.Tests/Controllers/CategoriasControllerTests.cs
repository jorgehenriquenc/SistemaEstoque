using Microsoft.AspNetCore.Mvc;
using SistemaEstoque.Api.Controllers;
using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Controllers
{
    public class CategoriasControllerTests
    {
        [Fact]
        public async Task ListarCategorias_DeveRetornarOkComCategorias()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);
            var controller = new CategoriasController(service);

            var resultado = await controller.ListarCategorias();

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var categorias = Assert.IsType<List<CategoriaResponseDto>>(okResult.Value);

            Assert.Single(categorias);
            Assert.Equal("Alimentos", categorias[0].Nome);
        }

        [Fact]
        public async Task BuscarCategoriaPorId_DeveRetornarOkComCategoria()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);
            var controller = new CategoriasController(service);

            var resultado = await controller.BuscarCategoriaPorId(1);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var categoria = Assert.IsType<CategoriaResponseDto>(okResult.Value);

            Assert.Equal(1, categoria.Id);
            Assert.Equal("Alimentos", categoria.Nome);
        }

        [Fact]
        public async Task CadastrarCategoria_DeveRetornarCreatedAtAction()
        {
            var repository = new CategoriaRepositoryFake();
            var service = new CategoriaService(repository);
            var controller = new CategoriasController(service);

            var dto = new CategoriaCreateDto
            {
                Nome = "Alimentos"
            };

            var resultado = await controller.CadastrarCategoria(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var categoria = Assert.IsType<CategoriaResponseDto>(createdResult.Value);

            Assert.Equal(nameof(CategoriasController.BuscarCategoriaPorId), createdResult.ActionName);
            Assert.Equal("Alimentos", categoria.Nome);
            Assert.Equal(1, categoria.Id);
        }

        [Fact]
        public async Task AtualizarCategoria_DeveRetornarOkComCategoriaAtualizada()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);
            var controller = new CategoriasController(service);

            var dto = new CategoriaUpdateDto
            {
                Nome = "Mercearia"
            };

            var resultado = await controller.AtualizarCategoria(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var categoria = Assert.IsType<CategoriaResponseDto>(okResult.Value);

            Assert.Equal("Mercearia", categoria.Nome);
        }

        [Fact]
        public async Task RemoverCategoria_DeveRetornarNoContent()
        {
            var repository = new CategoriaRepositoryFake();

            repository.Categorias.Add(new Categoria
            {
                Id = 1,
                Nome = "Alimentos"
            });

            var service = new CategoriaService(repository);
            var controller = new CategoriasController(service);

            var resultado = await controller.RemoverCategoria(1);

            Assert.IsType<NoContentResult>(resultado);
            Assert.Empty(repository.Categorias);
        }

        private class CategoriaRepositoryFake : ICategoriaRepository
        {
            public List<Categoria> Categorias { get; } = new();

            public HashSet<int> CategoriasComProdutos { get; } = new();

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

                Categorias.Add(categoria);

                return Task.CompletedTask;
            }

            public Task AtualizarAsync(Categoria categoria)
            {
                return Task.CompletedTask;
            }

            public Task RemoverAsync(Categoria categoria)
            {
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