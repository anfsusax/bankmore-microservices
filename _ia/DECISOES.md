# DECISOES

Registro de decisoes importantes tomadas neste projeto.

Registrar no momento em que a decisao for tomada, nao ao encerrar a sessao.

---

## Quando Registrar

Registre toda decisao que afete arquitetura, escopo, tecnologia, fluxo de trabalho
ou qualquer coisa que impacte o futuro do projeto.

---

## Formato

```
## 2026-08-24 Titulo da Decisao
**Contexto:** por que foi necessaria
**Decisao:** o que foi decidido
**Impacto:** o que muda
```

---

## Registro De Decisoes

### 2026-08-25 — Reformulação Visual com Paleta Suave de Banco Real
**Contexto:** o tema escuro anterior apresentava contraste excessivo e pouca aderência estética aos padrões de bancos digitais e corporativos reais do dia a dia.
**Decisao:** remodelar todo o design system no `app.css` e componentes Blazor para utilizar superfícies claras em tons suaves (Slate/Soft Pearl `#f8fafc` e `#f1f5f9`), cartões brancos com sombras suaves, detalhes em Royal Blue (`#2563eb`), cartão digital azul-marinho com chip metálico e tabelas financeiras limpas.
**Impacto:** a interface do BankMore agora oferece alta legibilidade, conforto visual e aparência idêntica a bancos digitais modernos (como Nubank, Inter, BTG e Revolut).

### 2026-08-24 — Criação da Interface Blazor Web App (BankMore.Web)

**Contexto:** necessidade do usuário por uma interface de banco digital intuitiva, moderna e responsiva para operar contas, depósitos, saques e saldos.
**Decisao:** criar o projeto `BankMore.Web` em Blazor (.NET 8) com suporte a componentes interativos (`InteractiveServer`), visual Glassmorphic com tema escuro e integração HTTP via `BankMoreApiClient`.
**Impacto:** os usuários agora possuem um portal bancário web completo na porta `5001` integrado ao Docker.

### 2026-08-24 — Migração de MySQL para PostgreSQL

**Contexto:** solicitação explícita do usuário para alterar a tecnologia de banco de dados do projeto de MySQL para PostgreSQL.
**Decisao:** substituir os pacotes MySQL por `Npgsql.EntityFrameworkCore.PostgreSQL` no .NET 8, atualizar `Program.cs` para suporte ao Npgsql e alterar a infraestrutura Docker para usar o container `postgres:16-alpine`.
**Impacto:** toda a persistência via Docker e EF Core agora utiliza PostgreSQL com o script de inicialização `sql/create.sql` convertido para a sintaxe Postgres.

### 2026-08-24 — Criacao Do Projeto

**Contexto:** projeto criado a partir do template da CENTRAL-ROBO.
**Decisao:** adotar a estrutura de memoria operacional padrao.
**Impacto:** todos os registros seguirao os formatos definidos em CONTRATO_DA_IA.md.
