using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaEstoque.Api.Exceptions;
using Xunit;

namespace SistemaEstoque.Tests.Exceptions
{
    public class GlobalExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_DeveRetornar400_QuandoBusinessValidationException()
        {
            var resultado = await ExecutarHandlerAsync(
                new BusinessValidationException("Dados inválidos.")
            );

            Assert.Equal(HttpStatusCode.BadRequest, resultado.StatusCode);
            Assert.Contains("Dados inválidos", resultado.Body);
            Assert.Contains("traceId", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornar401_QuandoInvalidCredentialsException()
        {
            var resultado = await ExecutarHandlerAsync(
                new InvalidCredentialsException("Email ou senha inválidos.")
            );

            Assert.Equal(HttpStatusCode.Unauthorized, resultado.StatusCode);
            Assert.Contains("Não autorizado", resultado.Body);
            Assert.Contains("Email ou senha inválidos.", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornar404_QuandoNotFoundException()
        {
            var resultado = await ExecutarHandlerAsync(
                new NotFoundException("Produto não encontrado.")
            );

            Assert.Equal(HttpStatusCode.NotFound, resultado.StatusCode);
            Assert.Contains("Recurso não encontrado", resultado.Body);
            Assert.Contains("Produto não encontrado.", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornar409_QuandoConflictException()
        {
            var resultado = await ExecutarHandlerAsync(
                new ConflictException("Já existe um registro com esses dados.")
            );

            Assert.Equal(HttpStatusCode.Conflict, resultado.StatusCode);
            Assert.Contains("Conflito", resultado.Body);
            Assert.Contains("Já existe um registro com esses dados.", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornar500_QuandoInvalidOperationException()
        {
            var resultado = await ExecutarHandlerAsync(
                new InvalidOperationException("Jwt:Key ausente.")
            );

            Assert.Equal(HttpStatusCode.InternalServerError, resultado.StatusCode);
            Assert.Contains("Erro de configuração", resultado.Body);
            Assert.Contains("A aplicação não está configurada corretamente.", resultado.Body);
            Assert.DoesNotContain("Jwt:Key ausente.", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornar500_QuandoExceptionGenerica()
        {
            var resultado = await ExecutarHandlerAsync(
                new Exception("Erro sensível interno.")
            );

            Assert.Equal(HttpStatusCode.InternalServerError, resultado.StatusCode);
            Assert.Contains("Erro interno do servidor", resultado.Body);
            Assert.Contains("Ocorreu um erro interno. Tente novamente mais tarde.", resultado.Body);
            Assert.DoesNotContain("Erro sensível interno.", resultado.Body);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornarTrue_QuandoExcecaoForTratada()
        {
            var handler = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance
            );

            var httpContext = CriarHttpContext();

            var tratado = await handler.TryHandleAsync(
                httpContext,
                new NotFoundException("Recurso não encontrado."),
                CancellationToken.None
            );

            Assert.True(tratado);
        }

        [Fact]
        public async Task TryHandleAsync_DeveRetornarProblemDetailsComoJson()
        {
            var resultado = await ExecutarHandlerAsync(
                new NotFoundException("Categoria não encontrada.")
            );

            using var documento = JsonDocument.Parse(resultado.Body);

            var root = documento.RootElement;

            Assert.True(root.TryGetProperty("title", out _));
            Assert.True(root.TryGetProperty("status", out _));
            Assert.True(root.TryGetProperty("detail", out _));
            Assert.True(root.TryGetProperty("instance", out _));
            Assert.True(root.TryGetProperty("traceId", out _));
        }

        [Fact]
        public async Task TryHandleAsync_DevePreencherInstanceComPathDaRequisicao()
        {
            var resultado = await ExecutarHandlerAsync(
                new NotFoundException("Produto não encontrado."),
                "/api/produtos/999"
            );

            Assert.Contains("/api/produtos/999", resultado.Body);
        }

        private static async Task<(HttpStatusCode StatusCode, string Body)> ExecutarHandlerAsync(
            Exception exception,
            string path = "/api/teste")
        {
            var handler = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance
            );

            var httpContext = CriarHttpContext(path);

            await handler.TryHandleAsync(
                httpContext,
                exception,
                CancellationToken.None
            );

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(httpContext.Response.Body);

            var body = await reader.ReadToEndAsync();

            return ((HttpStatusCode)httpContext.Response.StatusCode, body);
        }

        private static DefaultHttpContext CriarHttpContext(string path = "/api/teste")
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddProblemDetails();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            httpContext.Request.Path = path;
            httpContext.Response.Body = new MemoryStream();

            return httpContext;
        }
    }
}