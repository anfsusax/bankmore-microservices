using System;

namespace BankMore.Auth.Domain.Entities
{
    public class Movimento
    {
        public Guid Id { get; private set; }
        public Guid IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public string TipoMovimento { get; private set; } = string.Empty; // "C" para Crédito, "D" para Débito
        public decimal Valor { get; private set; }
        public string ChaveIdempotencia { get; private set; } = string.Empty;
        public string? Descricao { get; private set; }

        public Movimento(Guid id, Guid idContaCorrente, DateTime dataMovimento, string tipoMovimento, decimal valor, string chaveIdempotencia, string? descricao = null)
        {
            Id = id;
            IdContaCorrente = idContaCorrente;
            DataMovimento = dataMovimento;
            TipoMovimento = tipoMovimento;
            Valor = valor;
            ChaveIdempotencia = chaveIdempotencia;
            Descricao = descricao;
        }

        public static Movimento CriarCredito(Guid idContaCorrente, decimal valor, string chaveIdempotencia, string? descricao = null)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor para crédito deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(chaveIdempotencia))
                throw new ArgumentException("Chave de idempotência é obrigatória");

            return new Movimento(
                Guid.NewGuid(),
                idContaCorrente,
                DateTime.UtcNow,
                "C",
                valor,
                chaveIdempotencia,
                descricao
            );
        }

        public static Movimento CriarDebito(Guid idContaCorrente, decimal valor, string chaveIdempotencia, string? descricao = null)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor para débito deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(chaveIdempotencia))
                throw new ArgumentException("Chave de idempotência é obrigatória");

            return new Movimento(
                Guid.NewGuid(),
                idContaCorrente,
                DateTime.UtcNow,
                "D",
                valor,
                chaveIdempotencia,
                descricao
            );
        }

        public bool EhCredito => TipoMovimento == "C";
        public bool EhDebito => TipoMovimento == "D";

        public decimal ValorComSinal => EhCredito ? Valor : -Valor;
    }
}
