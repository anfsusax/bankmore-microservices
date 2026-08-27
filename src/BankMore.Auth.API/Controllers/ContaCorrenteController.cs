using BankMore.Auth.Application.Commands;
using BankMore.Auth.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly BankMore.Auth.Domain.Repositories.IContaCorrenteRepository _repository;

        public ContaCorrenteController(IMediator mediator, BankMore.Auth.Domain.Repositories.IContaCorrenteRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarContaCorrenteCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Criar), new { id }, new { id });
        }

        [HttpGet("contas/{id:guid}/saldo")]
        public async Task<IActionResult> ObterSaldo(Guid id)
        {
            var saldo = await _mediator.Send(new ObterSaldoQuery(id));
            return Ok(saldo);
        }

        [HttpGet("contas/numero/{numero:int}/saldo")]
        public async Task<IActionResult> ObterSaldoPorNumero(int numero)
        {
            var conta = await _repository.ObterPorNumeroAsync(numero);
            if (conta == null)
                return NotFound(new { message = "Conta não encontrada." });

            return Ok(conta.Saldo);
        }

        [HttpGet("contas/numero/{numero:int}")]
        public async Task<IActionResult> ObterContaPorNumero(int numero)
        {
            var conta = await _repository.ObterPorNumeroAsync(numero);
            if (conta == null)
                return NotFound(new { message = "Conta não encontrada." });

            return Ok(new
            {
                id = conta.Id,
                numero = conta.Numero,
                nome = conta.Nome,
                saldo = conta.Saldo,
                ativo = conta.Ativo,
                criadoEm = conta.CriadoEm
            });
        }


    }
}
