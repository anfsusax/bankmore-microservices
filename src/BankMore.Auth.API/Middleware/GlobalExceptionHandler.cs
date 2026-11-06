using BankMore.Auth.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Middleware
{
    /// <summary>
    /// Traduz exceções em ProblemDetails (RFC 9457). Regras:
    /// - erro de negócio carrega um "code" estável (INSUFFICIENT_FUNDS...) que o cliente pode tratar;
    /// - erro inesperado NUNCA expõe detalhe interno, mas é logado com o traceId para correlação.
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetails;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetails)
        {
            _logger = logger;
            _problemDetails = problemDetails;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, titulo, codigo, detalhe) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Erro de validação", "VALIDATION_ERROR", "Dados inválidos fornecidos."),
                ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", "INVALID_ARGUMENT", e.Message),
                UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Não autorizado", "UNAUTHORIZED", e.Message),
                AcessoNegadoException e => (StatusCodes.Status403Forbidden, "Acesso negado", e.Codigo, e.Message),
                RecursoNaoEncontradoException e => (StatusCodes.Status404NotFound, "Não encontrado", e.Codigo, e.Message),
                ConflitoDeIdempotenciaException e => (StatusCodes.Status422UnprocessableEntity, "Conflito de idempotência", e.Codigo, e.Message),
                RegraDeNegocioException e => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio", e.Codigo, e.Message),
                InvalidOperationException e => (StatusCodes.Status422UnprocessableEntity, "Operação inválida", "INVALID_OPERATION", e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno", "INTERNAL_ERROR", "Ocorreu um erro inesperado.")
            };

            if (status >= 500)
                _logger.LogError(exception, "Erro não tratado em {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            else
                _logger.LogInformation("Requisição recusada com {Status} ({Codigo}): {Mensagem}", status, codigo, exception.Message);

            var problema = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = detalhe,
                Extensions = { ["code"] = codigo }
            };

            if (exception is ValidationException validacao)
            {
                problema.Extensions["errors"] = validacao.Errors
                    .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            }

            httpContext.Response.StatusCode = status;
            return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problema,
                Exception = exception
            });
        }
    }
}
