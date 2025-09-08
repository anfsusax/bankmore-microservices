using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BankMore.Auth.API.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMediator mediator)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    Status = 400,
                    Title = "Erro de validação",
                    Detail = "Dados inválidos fornecidos",
                    Errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            }
        }
    }
}
