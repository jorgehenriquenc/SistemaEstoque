using System.Collections;
using System.ComponentModel.DataAnnotations;
using SistemaEstoque.Api.Dtos.Auth;
using SistemaEstoque.Api.Dtos.Categorias;
using SistemaEstoque.Api.Dtos.Pedidos;
using SistemaEstoque.Api.Dtos.Produtos;
using Xunit;

namespace SistemaEstoque.Tests.Dtos
{
    public class DtoValidationTests
    {
        [Fact]
        public void RegisterDto_DeveSerValido_QuandoDadosForemCorretos()
        {
            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123456"
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void RegisterDto_DeveSerInvalido_QuandoNomeEstiverVazio()
        {
            var dto = new RegisterDto
            {
                Nome = "",
                Email = "jorge@email.com",
                Senha = "123456"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void RegisterDto_DeveSerInvalido_QuandoEmailForInvalido()
        {
            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "email-invalido",
                Senha = "123456"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void RegisterDto_DeveSerInvalido_QuandoSenhaForMuitoCurta()
        {
            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void LoginDto_DeveSerValido_QuandoDadosForemCorretos()
        {
            var dto = new LoginDto
            {
                Email = "jorge@email.com",
                Senha = "123456"
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void LoginDto_DeveSerInvalido_QuandoEmailForInvalido()
        {
            var dto = new LoginDto
            {
                Email = "email-invalido",
                Senha = "123456"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void LoginDto_DeveSerInvalido_QuandoSenhaForMuitoCurta()
        {
            var dto = new LoginDto
            {
                Email = "jorge@email.com",
                Senha = "123"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void CategoriaCreateDto_DeveSerValido_QuandoNomeForCorreto()
        {
            var dto = new CategoriaCreateDto
            {
                Nome = "Alimentos"
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void CategoriaCreateDto_DeveSerInvalido_QuandoNomeForMuitoCurto()
        {
            var dto = new CategoriaCreateDto
            {
                Nome = "A"
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerValido_QuandoDadosForemCorretos()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerInvalido_QuandoNomeForMuitoCurto()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "A",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerInvalido_QuandoPrecoForZero()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 0,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerInvalido_QuandoQuantidadeForNegativa()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = -1,
                EstoqueMinimo = 2,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerInvalido_QuandoEstoqueMinimoForNegativo()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = -1,
                CategoriaId = 1
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ProdutoCreateDto_DeveSerInvalido_QuandoCategoriaIdForZero()
        {
            var dto = new ProdutoCreateDto
            {
                Nome = "Arroz",
                Preco = 25.90m,
                QuantidadeEmEstoque = 10,
                EstoqueMinimo = 2,
                CategoriaId = 0
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void PedidoCreateDto_DeveSerValido_QuandoPossuirItemValido()
        {
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

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void PedidoCreateDto_DeveSerInvalido_QuandoListaDeItensEstiverVazia()
        {
            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>()
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ItemPedidoCreateDto_DeveSerValido_QuandoDadosForemCorretos()
        {
            var dto = new ItemPedidoCreateDto
            {
                ProdutoId = 1,
                Quantidade = 2
            };

            var erros = Validar(dto);

            Assert.Empty(erros);
        }

        [Fact]
        public void ItemPedidoCreateDto_DeveSerInvalido_QuandoProdutoIdForZero()
        {
            var dto = new ItemPedidoCreateDto
            {
                ProdutoId = 0,
                Quantidade = 2
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void ItemPedidoCreateDto_DeveSerInvalido_QuandoQuantidadeForZero()
        {
            var dto = new ItemPedidoCreateDto
            {
                ProdutoId = 1,
                Quantidade = 0
            };

            var erros = Validar(dto);

            Assert.NotEmpty(erros);
        }

        [Fact]
        public void PedidoCreateDto_DeveSerInvalido_QuandoItemDentroDoPedidoForInvalido()
        {
            var dto = new PedidoCreateDto
            {
                Itens = new List<ItemPedidoCreateDto>
                {
                    new ItemPedidoCreateDto
                    {
                        ProdutoId = 0,
                        Quantidade = 0
                    }
                }
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