namespace BankMore.Web.Livro
{
    public enum EstiloArquitetura { Fundamento, MonolitoModular, Microsservico, Prova }

    public enum StatusCapitulo { Escrito, EmAndamento, Planejado }

    public sealed record Capitulo(
        string Parte,
        string Numero,
        string Slug,
        string Titulo,
        string Pergunta,
        EstiloArquitetura Estilo,
        StatusCapitulo Status)
    {
        public string Rota => $"livro/{Numero}-{Slug}";
    }

    /// <summary>
    /// Índice do "Livro do Laboratório". Cada capítulo responde UMA pergunta que uma empresa faria.
    /// O formato de cada capítulo segue a didática do microservico-senior:
    /// Problema → Objetivo → Analogia → Diagrama → Código → Lab → Entrevista.
    /// </summary>
    public static class CatalogoDoLivro
    {
        public static readonly IReadOnlyList<Capitulo> Capitulos = new[]
        {
            new Capitulo("I · Antes de migrar", "00", "estabilizar-antes-de-migrar", "Estabilizar antes de migrar",
                "\"Alex, migra pra microsserviços.\" Por onde começa?", EstiloArquitetura.Fundamento, StatusCapitulo.Escrito),
            new Capitulo("I · Antes de migrar", "01", "por-que-migrar", "Por que (e quando) migrar",
                "Qual dor os microsserviços resolvem aqui? E se a dor não existir?", EstiloArquitetura.Fundamento, StatusCapitulo.Planejado),
            new Capitulo("I · Antes de migrar", "02", "bounded-contexts", "O mapa de contextos do BankMore",
                "Onde estão as fronteiras reais do negócio bancário?", EstiloArquitetura.Fundamento, StatusCapitulo.Planejado),

            new Capitulo("II · Por que NÃO migrou", "03", "monolito-modular", "Monólito modular: Contas e Transferências",
                "Como ter fronteiras fortes sem pagar o preço da rede?", EstiloArquitetura.MonolitoModular, StatusCapitulo.Planejado),
            new Capitulo("II · Por que NÃO migrou", "04", "ledger-e-invariante", "Ledger e a invariante do saldo",
                "Por que transferência interna não vira saga?", EstiloArquitetura.MonolitoModular, StatusCapitulo.Planejado),
            new Capitulo("II · Por que NÃO migrou", "05", "contratos-entre-modulos", "Contratos entre módulos",
                "Como um módulo conversa com outro sem FK nem join?", EstiloArquitetura.MonolitoModular, StatusCapitulo.Planejado),

            new Capitulo("III · Por que migrou", "06", "gateway-strangler-fig", "Gateway e Strangler Fig",
                "Como trocar o motor com o avião voando?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),
            new Capitulo("III · Por que migrou", "07", "extraindo-identity", "Extraindo o Identity",
                "Por que o Identity é o primeiro a sair?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),
            new Capitulo("III · Por que migrou", "08", "outbox-kafka-tarifas", "Outbox + Kafka: Tarifas e Notificações",
                "Como garantir que o evento sai se o banco commitou?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),
            new Capitulo("III · Por que migrou", "09", "circuit-breaker-spi", "Circuit Breaker no SPI",
                "O SPI caiu. O que acontece com o PIX do cliente?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),
            new Capitulo("III · Por que migrou", "10", "saga-pix", "A saga do PIX",
                "Bloqueio, liquidação, estorno: e se o estorno falhar?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),
            new Capitulo("III · Por que migrou", "11", "observabilidade-lgpd", "Observabilidade com LGPD",
                "Como rastrear uma transação sem vazar CPF?", EstiloArquitetura.Microsservico, StatusCapitulo.Planejado),

            new Capitulo("IV · Prova", "12", "concorrencia-e-carga", "Concorrência e carga",
                "Como provar que o saldo fecha com 500 requisições simultâneas?", EstiloArquitetura.Prova, StatusCapitulo.Planejado),
            new Capitulo("IV · Prova", "13", "modular-vs-microsservicos", "Modular x Microsserviços: o comparativo",
                "Com números na mão: o que cada escolha custou?", EstiloArquitetura.Prova, StatusCapitulo.Planejado),
        };

        public static Capitulo Obter(string numero) => Capitulos.First(c => c.Numero == numero);

        public static Capitulo? Proximo(string numero)
        {
            var indice = Capitulos.ToList().FindIndex(c => c.Numero == numero);
            return indice >= 0 && indice + 1 < Capitulos.Count ? Capitulos[indice + 1] : null;
        }
    }
}
