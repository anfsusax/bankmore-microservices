using BankMore.Auth.API.Requests;
using BankMore.Auth.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/transferencias")]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Transferência interna, a partir da conta do usuário autenticado.</summary>
        /// <response code="201">Transferência realizada.</response>
        /// <response code="200">Reenvio da mesma chave com o mesmo conteúdo: devolve o resultado original.</response>
        /// <response code="422">Saldo insuficiente, conta inativa ou chave reutilizada com outro conteúdo.</response>
        [HttpPost]
        public async Task<IActionResult> RealizarTransferencia(
            [FromBody] RealizarTransferenciaRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            var chave = string.IsNullOrWhiteSpace(idempotencyKey) ? request.ChaveIdempotencia : idempotencyKey;

            var resultado = await _mediator.Send(
                new RealizarTransferenciaCommand(request.NumeroContaDestino, request.Valor, chave ?? string.Empty),
                cancellationToken);

            var corpo = new { id = resultado.Id, replay = resultado.Replay };
            return resultado.Replay ? Ok(corpo) : StatusCode(StatusCodes.Status201Created, corpo);
        }
    }
}
