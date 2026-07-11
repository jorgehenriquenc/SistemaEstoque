using System.Collections;
using System.ComponentModel.DataAnnotations;
using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Dtos.Produtos;
using Xunit;

namespace SistemaEstoque.Tests.Dtos
{
    public class DtoUpdateValidationTests
    {
        [Fact]
        public void CategoriaUpdateDto_DeveSerValido_QuandoNomeForCorreto()
        {
            var dto = new CategoriaUpdateDto
            {
                Nome = "Mercearia"
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void CategoriaUpdateDto_DeveSerInvalido_QuandoNomeEstiverVazio()
        {
            var dto = new CategoriaUpdateDto
            {
                Nome = ""
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void CategoriaUpdateDto_DeveSerInvalido_QuandoNomeForMuitoCurto()
        {
            var dto = new CategoriaUpdateDto
            {
                Nome = "A"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerValido_QuandoDadosForemCorretos()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoNomeEstiverVazio()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoNomeForMuitoCurto()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "A",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoPrecoForZero()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 0,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoQuantidadeForNegativa()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 30.00m,
                QuantidadeEmEstoque = -1,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoEstoqueMinimoForNegativo()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = -1,
                Ativo = true,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoUpdateDto_DeveSerInvalido_QuandoCategoriaIdForZero()
        {
            var dto = new ProdutoUpdateDto
            {
                Nome = "Arroz Integral",
                Preco = 30.00m,
                QuantidadeEmEstoque = 15,
                EstoqueMinimo = 3,
                Ativo = true,
                CategoriaId = 0
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        private static List<ValidationResult> Validar(object objeto)
        {
            var erros = new List<ValidationResult>();

            ValidarObjeto(objeto, erros);

            return erros;
        }

        private static void ValidarObjeto(object? objeto, List<ValidationResult> erros)
        {
            if (objeto is null)
            {
                return;
            }

            var contexto = new ValidationContext(objeto);

            Validator.TryValidateObject(
                objeto,
                contexto,
                erros,
                validateAllProperties: true
            );

            var propriedades = objeto
                .GetType()
                .GetProperties()
                .Where(propriedade => propriedade.CanRead);

            foreach (var propriedade in propriedades)
            {
                var valor = propriedade.GetValue(objeto);

                if (valor is null || valor is string)
                {
                    continue;
                }

                if (valor is IEnumerable lista)
                {
                    foreach (var item in lista)
                    {
                        ValidarObjeto(item, erros);
                    }

                    continue;
                }

                if (!propriedade.PropertyType.IsValueType)
                {
                    ValidarObjeto(valor, erros);
                }
            }
        }
    }
}