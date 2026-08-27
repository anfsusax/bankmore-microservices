using BankMore.Auth.Application.Commands;
using BankMore.Auth.Domain.Entities;
using BankMore.Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Auth.API.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IContaCorrenteRepository _contaRepo;

        public LoginController(IMediator mediator, IUsuarioRepository usuarioRepo, IContaCorrenteRepository contaRepo)
        {
            _mediator = mediator;
            _usuarioRepo = usuarioRepo;
            _contaRepo = contaRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AutenticarUsuarioCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);

                // Localiza o usuário autenticado
                var usuario = await _usuarioRepo.ObterPorCpfAsync(command.DocumentoOuConta)
                             ?? await _usuarioRepo.ObterPorEmailAsync(command.DocumentoOuConta);

                ContaCorrente? conta = null;
                if (usuario != null)
                {
                    conta = await _contaRepo.ObterPorNomeAsync(usuario.Nome);
                }

                if (conta == null && int.TryParse(command.DocumentoOuConta, out int numConta))
                {
                    conta = await _contaRepo.ObterPorNumeroAsync(numConta);
                }

                return Ok(new
                {
                    token,
                    usuario = usuario == null ? null : new
                    {
                        id = usuario.Id,
                        nome = usuario.Nome,
                        cpf = usuario.Cpf.Numero,
                        email = usuario.Email.Endereco
                    },
                    conta = conta == null ? null : new
                    {
                        id = conta.Id,
                        numero = conta.Numero,
                        nome = conta.Nome,
                        saldo = conta.Saldo,
                        ativo = conta.Ativo
                    }
                });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new
                {
                    message = e.Message,
                    type = "USER_UNAUTHORIZED"
                });
            }
        }
    }
}
