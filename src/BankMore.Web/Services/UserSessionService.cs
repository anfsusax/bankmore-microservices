using BankMore.Web.Models;

namespace BankMore.Web.Services
{
    public class UserSessionService
    {
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
        public string Token { get; private set; } = string.Empty;
        public string UsuarioId { get; private set; } = string.Empty;
        public string Nome { get; private set; } = "Visitante";
        public string Cpf { get; private set; } = string.Empty;
        public int NumeroConta { get; private set; } = 1001;
        public Guid ContaId { get; private set; }
        public decimal Saldo { get; private set; } = 0m;
        public bool EsconderSaldo { get; set; } = false;

        public List<MovimentoItem> Extrato { get; private set; } = new();

        public event Action? OnStateChanged;

        public void DefinirSessao(string token, string nome, string cpf, int numeroConta, Guid contaId, decimal saldo)
        {
            Token = token;
            Nome = nome;
            Cpf = cpf;
            NumeroConta = numeroConta;
            ContaId = contaId;
            Saldo = saldo;

            OnStateChanged?.Invoke();
        }

        public void AtualizarSaldo(decimal novoSaldo)
        {
            Saldo = novoSaldo;
            OnStateChanged?.Invoke();
        }

        public void AdicionarMovimento(string tipo, decimal valor, string descricao, string chaveIdempotencia)
        {
            Extrato.Insert(0, new MovimentoItem
            {
                Tipo = tipo,
                Valor = valor,
                Descricao = descricao,
                Data = DateTime.Now,
                ChaveIdempotencia = chaveIdempotencia
            });

            OnStateChanged?.Invoke();
        }

        public void AlternarVisibilidadeSaldo()
        {
            EsconderSaldo = !EsconderSaldo;
            OnStateChanged?.Invoke();
        }

        public void EncerrarSessao()
        {
            Token = string.Empty;
            UsuarioId = string.Empty;
            Nome = "Visitante";
            Cpf = string.Empty;
            NumeroConta = 1001;
            ContaId = Guid.Empty;
            Saldo = 0m;
            Extrato.Clear();

            OnStateChanged?.Invoke();
        }
    }
}
