namespace BankMore.Auth.Domain.Entities
{
    public class Idempotencia
    {
        public string Chave { get; private set; } = string.Empty;
        public string Requisicao { get; private set; } = string.Empty;
        public string Resultado { get; private set; } = string.Empty;
        public DateTime CriadoEm { get; private set; }

        public Idempotencia(string chave, string requisicao, string resultado)
        {
            Chave = chave;
            Requisicao = requisicao;
            Resultado = resultado;
            CriadoEm = DateTime.UtcNow;
        }

        private Idempotencia() { }
    }
}
