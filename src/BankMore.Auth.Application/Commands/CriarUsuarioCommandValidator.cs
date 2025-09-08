using FluentValidation;

namespace BankMore.Auth.Application.Commands
{
    public class CriarUsuarioCommandValidator : AbstractValidator<CriarUsuarioCommand>
    {
        public CriarUsuarioCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .MinimumLength(2).WithMessage("Nome deve ter no mínimo 2 caracteres");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Length(11, 14).WithMessage("CPF deve ter entre 11 e 14 caracteres")
                .Must(CPFValido).WithMessage("CPF deve ter um formato válido");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ter um formato válido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$")
                .WithMessage("Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número");
        }

        private bool CPFValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres especiais
            var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

            // Verifica se tem 11 dígitos
            if (cpfLimpo.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpfLimpo.Distinct().Count() == 1)
                return false;

            // Verifica se contém apenas números
            if (!cpfLimpo.All(char.IsDigit))
                return false;

            return true;
        }
    }
}