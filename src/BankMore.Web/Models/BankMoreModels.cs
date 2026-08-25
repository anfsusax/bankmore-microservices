namespace BankMore.Web.Models
{
    public class LoginModel
    {
        public string DocumentoOuConta { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int NumeroConta { get; set; } = 1001;
    }

    public class CriarContaModel
    {
        public string Nome { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Senha { get; set; } = string.Empty;
    }

    public class MovimentoModel
    {
        public string Tipo { get; set; } = "C"; // "C" para Crédito, "D" para Débito
        public decimal Valor { get; set; } = 100m;
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public int? NumeroConta { get; set; }
        public string? Descricao { get; set; }
    }

    public class TransferenciaModel
    {
        public Guid IdContaOrigem { get; set; }
        public Guid IdContaDestino { get; set; }
        public int? NumeroContaDestino { get; set; }
        public decimal Valor { get; set; } = 50m;
        public string ChaveIdempotencia { get; set; } = string.Empty;
    }

    public class MovimentoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Tipo { get; set; } = "C";
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; } = DateTime.Now;
        public string ChaveIdempotencia { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
    }
}
