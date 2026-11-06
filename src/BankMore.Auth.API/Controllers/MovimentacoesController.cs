using BankMore.Auth.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Controllers
{
    [ApiController]
    [Route("api/movimentacoes")]
    [Authorize]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovimentacoesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Depósito ("C") ou saque ("D"). Saque só na conta do próprio titular.</summary>
        [HttpPost]
        public async Task<IActionResult> Movimentar(MovimentarContaCommand command, CancellationToken cancellationToken)
        {
            var resultado = await _mediator.Send(command, cancellationToken);
            var corpo = new { id = resultado.Id, replay = resultado.Replay };
            return resultado.Replay ? Ok(corpo) : StatusCode(StatusCodes.Status201Created, corpo);
        }
    }
}
