using FluentValidation;

namespace BankMore.Auth.Application.Commands
{
    public class CriarContaCorrenteCommandValidator : AbstractValidator<CriarContaCorrenteCommand>
    {
        public CriarContaCorrenteCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .MinimumLength(2).WithMessage("Nome deve ter no mínimo 2 caracteres");

            RuleFor(x => x.Numero)
                .GreaterThan(0).WithMessage("Número da conta deve ser maior que zero")
                .LessThan(999999999).WithMessage("Número da conta deve ser menor que 999999999");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
                .WithMessage("Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número");
        }
    }
}
