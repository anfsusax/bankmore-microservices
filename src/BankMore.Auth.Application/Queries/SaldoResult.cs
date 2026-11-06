namespace BankMore.Auth.Application.Queries
{
    public class SaldoResult
    {
        public Guid IdConta { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Saldo { get; set; }
    }
}
