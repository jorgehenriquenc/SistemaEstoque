using Microsoft.AspNetCore.Diagnostics;

namespace SistemaEstoque.Api.Exceptions
{
    // Responsável por tratar exceções não capturadas
    // e convertê-las em respostas HTTP padronizadas.
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (
                statusCode,
                titulo,
                detalhe
            ) = exception switch
            {
                BusinessValidationException => (
                    StatusCodes.Status400BadRequest,
                    "Dados inválidos",
                    exception.Message
                ),

                InvalidCredentialsException => (
                    StatusCodes.Status401Unauthorized,
                    "Não autorizado",
                    exception.Message
                ),

                NotFoundException => (
                    StatusCodes.Status404NotFound,
                    "Recurso não encontrado",
                    exception.Message
                ),

                ConflictException => (
                    StatusCodes.Status409Conflict,
                    "Conflito",
                    exception.Message
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Erro interno do servidor",
                    "Ocorreu um erro interno. Tente novamente mais tarde."
                )
            };

            RegistrarErro(
                httpContext,
                exception,
                statusCode
            );

            await Results.Problem(
                statusCode: statusCode,
                title: titulo,
                detail: detalhe,
                instance: httpContext.Request.Path,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = httpContext.TraceIdentifier
                }
            ).ExecuteAsync(httpContext);

            // Retorna true para informar que a exceção
            // já foi tratada por este handler.
            return true;
        }

        private void RegistrarErro(
            HttpContext httpContext,
            Exception exception,
            int statusCode)
        {
            if (statusCode ==
                StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Erro interno não tratado. TraceId: {TraceId}",
                    httpContext.TraceIdentifier
                );

                return;
            }

            _logger.LogWarning(
                "Erro tratado do tipo {TipoExcecao}. " +
                "Mensagem: {Mensagem}. TraceId: {TraceId}",
                exception.GetType().Name,
                exception.Message,
                httpContext.TraceIdentifier
            );
        }
    }
}