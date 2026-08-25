# DIARIO

Registro cronologico de sessoes de trabalho neste projeto.

Adicionar uma entrada ao encerrar cada sessao. Adicionar sempre ao topo.
Nao editar entradas antigas.

---

## Entradas

### 2026-08-24 — Implementação Repositórios EF Core, Blazor WebApp e Ajuste de Porta PostgreSQL

**O que foi feito:** 
- Implementação completa dos repositórios EF Core (`UsuarioRepositoryEfCore`, `MovimentoRepositoryEfCore`, `TransferenciaRepositoryEfCore`, `IdempotenciaRepositoryEfCore`) substituindo os stubs com `NotImplementedException`.
- Ajuste do handler `MovimentarContaCommandHandler` para calcular e atualizar o saldo da conta corrente no PostgreSQL.
- Adição de autenticação JWT Bearer no Swagger UI (`Program.cs`).
- Ajuste da porta externa do PostgreSQL para `5433:5432` no `docker-compose.yml` para evitar conflito com serviço local do Windows.
- Criação e integração do projeto **Blazor Web App (`BankMore.Web`)** em .NET 8 com design bancário moderno (Glassmorphism, Dark Theme, cartões, modal de depósitos/saques e extrato em tempo real na porta `5001`).
**Decisoes tomadas:** Criação de interface rica em Blazor (.NET 8) para facilitar a operação e testes do banco BankMore.
**Problemas encontrados:** Conflito na porta 5432 resolvido mapeando para 5433; compatibilização do Blazor para .NET 8.
**Proximos passos:** Manutenção e expansão de novas funcionalidades financeiras.

### 2026-08-24 — Migração PostgreSQL e Containerização Docker

**O que foi feito:** 
- Análise completa da arquitetura do projeto (Clean Architecture, DDD, CQRS, MediatR, FluentValidation, xUnit).
- Migração da base de dados de MySQL para PostgreSQL (`postgres:16-alpine`), com atualização do driver EF Core para `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Adaptação do script DDL `sql/create.sql` para tipos e sintaxe PostgreSQL.
- Ajuste do `docker-compose.yml` (PostgreSQL com `healthcheck` via `pg_isready`, `bankmore_api` escutando na porta 80/5000, Kafka e Zookeeper).
- Correção de case-sensitivity no `Dockerfile` (`BankMore.Auth.API`).
- Compilação do projeto (0 erros) e execução de 29/29 testes unitários aprovados com sucesso.
**Decisoes tomadas:** Substituição de MySQL por PostgreSQL e conteinerização completa da stack via Docker Compose.
**Problemas encontrados:** Ajuste de versões do EF Core (8.0.11) para compatibilidade com o Npgsql.
**Proximos passos:** Validar a inicialização e comunicação dos containers Docker com o PostgreSQL e Swagger.

### 2026-08-24 — Criacao Do Projeto

**O que foi feito:** projeto criado a partir do template da CENTRAL-ROBO.
**Decisoes tomadas:** estrutura de arquivos definida conforme protocolo da central.
**Problemas encontrados:** nenhum.
**Proximos passos:** definir objetivo real em ESTADO.md, ORIGEM.md e CONTRATO_DA_IA.md.
