using FluentValidation;

namespace BankMore.Auth.Application.Commands
{
    public class AutenticarUsuarioCommandValidator : AbstractValidator<AutenticarUsuarioCommand>
    {
        public AutenticarUsuarioCommandValidator()
        {
            RuleFor(x => x.DocumentoOuConta)
                .NotEmpty().WithMessage("Documento ou conta é obrigatório")
                .MaximumLength(100).WithMessage("Documento ou conta deve ter no máximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres");
        }
    }
}
