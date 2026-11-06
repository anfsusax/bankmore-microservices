using BankMore.Auth.Application.Commands;
using BankMore.Auth.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Controllers
{
    /// <summary>
    /// Todas as rotas exigem autenticação. As antigas rotas públicas de saldo
    /// (/contas/{id}/saldo e /contas/numero/{n}/saldo) foram removidas: expunham o saldo de qualquer conta.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Abre a conta corrente do usuário autenticado (uma por titular).</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarContaCorrenteCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(MinhaConta), null, new { id });
        }

        /// <summary>Conta e saldo do usuário autenticado.</summary>
        [HttpGet("minha")]
        public async Task<ActionResult<SaldoResult>> MinhaConta(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetSaldoQuery(), cancellationToken));
        }

        /// <summary>Confirma o destino de uma transferência: número + titular mascarado. Sem saldo.</summary>
        [HttpGet("contas/numero/{numero:int}")]
        public async Task<ActionResult<ContaDestinoResult>> ObterContaDestino(int numero, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new ObterContaDestinoQuery(numero), cancellationToken));
        }
    }
}
