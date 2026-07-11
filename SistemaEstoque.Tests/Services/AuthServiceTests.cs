using Microsoft.Extensions.Configuration;
using SistemaEstoque.Api.Dtos.Auth;
using SistemaEstoque.Api.Exceptions;
using SistemaEstoque.Api.Services;
using SistemaEstoque.Data.Entities;
using SistemaEstoque.Data.Repositories.Interfaces;
using Xunit;

namespace SistemaEstoque.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegistrarAsync_DeveCadastrarUsuarioValido()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "  Jorge Henrique  ",
                Email = "  JORGE@EMAIL.COM  ",
                Senha = "123456"
            };

            await service.RegistrarAsync(dto);

            Assert.NotNull(repository.UsuarioCadastrado);
            Assert.Equal("Jorge Henrique", repository.UsuarioCadastrado.Nome);
            Assert.Equal("jorge@email.com", repository.UsuarioCadastrado.Email);
            Assert.False(string.IsNullOrWhiteSpace(repository.UsuarioCadastrado.SenhaHash));
            Assert.NotEqual("123456", repository.UsuarioCadastrado.SenhaHash);
        }

        [Fact]
        public async Task RegistrarAsync_DeveLancarBusinessValidationException_QuandoNomeEstiverVazio()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "",
                Email = "jorge@email.com",
                Senha = "123456"
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarAsync_DeveLancarBusinessValidationException_QuandoEmailEstiverVazio()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "",
                Senha = "123456"
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarAsync_DeveLancarBusinessValidationException_QuandoSenhaEstiverVazia()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = ""
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarAsync_DeveLancarBusinessValidationException_QuandoSenhaForMuitoCurta()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123"
            };

            await Assert.ThrowsAsync<BusinessValidationException>(() =>
                service.RegistrarAsync(dto)
            );
        }

        [Fact]
        public async Task RegistrarAsync_DeveLancarConflictException_QuandoEmailJaExistir()
        {
            var repository = new UsuarioRepositoryFake();

            repository.Usuarios.Add(new Usuario
            {
                Id = 1,
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                SenhaHash = "hash-existente"
            });

            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new RegisterDto
            {
                Nome = "Outro Usuário",
                Email = "JORGE@EMAIL.COM",
                Senha = "123456"
            };

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.RegistrarAsync(dto)
            );
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisForemValidas()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            await service.RegistrarAsync(new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123456"
            });

            var resultado = await service.LoginAsync(new LoginDto
            {
                Email = "JORGE@EMAIL.COM",
                Senha = "123456"
            });

            Assert.Equal("Jorge Henrique", resultado.Nome);
            Assert.Equal("jorge@email.com", resultado.Email);
            Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarInvalidCredentialsException_QuandoUsuarioNaoExistir()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            var dto = new LoginDto
            {
                Email = "naoexiste@email.com",
                Senha = "123456"
            };

            await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                service.LoginAsync(dto)
            );
        }

        [Fact]
        public async Task LoginAsync_DeveLancarInvalidCredentialsException_QuandoSenhaForInvalida()
        {
            var repository = new UsuarioRepositoryFake();
            var service = new AuthService(repository, CriarConfiguracaoJwtValida());

            await service.RegistrarAsync(new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123456"
            });

            var dto = new LoginDto
            {
                Email = "jorge@email.com",
                Senha = "senhaerrada"
            };

            await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                service.LoginAsync(dto)
            );
        }

        [Fact]
        public async Task LoginAsync_DeveLancarInvalidOperationException_QuandoJwtKeyNaoEstiverConfigurada()
        {
            var repository = new UsuarioRepositoryFake();
            var serviceComJwt = new AuthService(repository, CriarConfiguracaoJwtValida());

            await serviceComJwt.RegistrarAsync(new RegisterDto
            {
                Nome = "Jorge Henrique",
                Email = "jorge@email.com",
                Senha = "123456"
            });

            var serviceSemJwtKey = new AuthService(
                repository,
                CriarConfiguracaoSemJwtKey()
            );

            var dto = new LoginDto
            {
                Email = "jorge@email.com",
                Senha = "123456"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                serviceSemJwtKey.LoginAsync(dto)
            );
        }

        private static IConfiguration CriarConfiguracaoJwtValida()
        {
            var dados = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-super-secreta-para-testes-com-tamanho-suficiente",
                ["Jwt:Issuer"] = "SistemaEstoque.Tests",
                ["Jwt:Audience"] = "SistemaEstoque.Tests"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dados)
                .Build();
        }

        private static IConfiguration CriarConfiguracaoSemJwtKey()
        {
            var dados = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "SistemaEstoque.Tests",
                ["Jwt:Audience"] = "SistemaEstoque.Tests"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dados)
                .Build();
        }

        private class UsuarioRepositoryFake : IUsuarioRepository
        {
            public List<Usuario> Usuarios { get; } = new();

            public Usuario? UsuarioCadastrado { get; private set; }

            public Task<Usuario?> BuscarPorEmailAsync(string email)
            {
                var emailNormalizado = email.Trim().ToLowerInvariant();

                var usuario = Usuarios.FirstOrDefault(usuario =>
                    usuario.Email.Trim().ToLowerInvariant() == emailNormalizado
                );

                return Task.FromResult(usuario);
            }

            public Task<bool> EmailExisteAsync(string email)
            {
                var emailNormalizado = email.Trim().ToLowerInvariant();

                var existe = Usuarios.Any(usuario =>
                    usuario.Email.Trim().ToLowerInvariant() == emailNormalizado
                );

                return Task.FromResult(existe);
            }

            public Task CadastrarAsync(Usuario usuario)
            {
                if (usuario.Id == 0)
                {
                    usuario.Id = Usuarios.Count == 0
                        ? 1
                        : Usuarios.Max(item => item.Id) + 1;
                }

                UsuarioCadastrado = usuario;
                Usuarios.Add(usuario);

                return Task.CompletedTask;
            }
        }
    }
}