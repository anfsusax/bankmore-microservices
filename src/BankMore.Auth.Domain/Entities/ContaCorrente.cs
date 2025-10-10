namespace BankMore.Auth.Domain.Entities
{
    public class ContaCorrente
    {
        public Guid Id { get; private set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public string Senha { get; private set; }
        public string Salt { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? AtualizadoEm { get; private set; }

        public ContaCorrente(Guid id, int numero, string nome, bool ativo, string senha, string salt, decimal saldo, DateTime criadoEm, DateTime? atualizadoEm)
        {
            Id = id;
            Numero = numero;
            Nome = nome;
            Ativo = ativo;
            Senha = senha;
            Salt = salt;
            Saldo = saldo;
            CriadoEm = criadoEm;
            AtualizadoEm = atualizadoEm;
        }

        public static ContaCorrente Criar(int numero, string nome, string senhaHash, string salt)
        {
            if (numero <= 0)
                throw new ArgumentException("Número da conta deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha é obrigatória");

            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentException("Salt é obrigatório");

            return new ContaCorrente(Guid.NewGuid(), numero, nome, true, senhaHash, salt, 0, DateTime.UtcNow, null);
        }

        public void Desativar() 
        { 
            Ativo = false; 
            AtualizadoEm = DateTime.UtcNow;
        }

        public void Inativar() => Desativar();

        public void Creditar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor para crédito deve ser maior que zero");

            Saldo += valor;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void Debitar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor para débito deve ser maior que zero");

            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para realizar o débito");

            Saldo -= valor;
            AtualizadoEm = DateTime.UtcNow;
        }

        public bool TemSaldoSuficiente(decimal valor) => Saldo >= valor;
    }
}
 