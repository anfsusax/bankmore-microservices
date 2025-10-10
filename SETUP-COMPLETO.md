# ✅ Setup Completo - BankMore API

## 📋 O que foi configurado

### 1️⃣ **Entity Framework Core**
- ✅ DbContext configurado com suporte dual (SQL Server local / MySQL Docker)
- ✅ Migration `InitialCreate` criada e aplicada
- ✅ Tabela `contacorrente` criada no banco SQL Server

### 2️⃣ **Injeções de Dependência**
- ✅ `IContaCorrenteRepository` → `ContaCorrenteRepositoryEfCore` (implementação completa)
- ✅ `IHttpContextAccessor` configurado
- ✅ `IUsuarioRepository` → `UsuarioRepositoryStub` (temporário)
- ✅ `IMovimentoRepository` → `MovimentoRepositoryStub` (temporário)
- ✅ `ITransferenciaRepository` → `TransferenciaRepositoryStub` (temporário)
- ✅ `IIdempotenciaRepository` → `IdempotenciaRepositoryStub` (temporário)

### 3️⃣ **API Pronta para Teste**
- ✅ Endpoint: `POST /api/contacorrente` - Criar conta corrente
- ✅ Endpoint: `GET /api/contacorrente/contas/{id}/saldo` - Obter saldo
- ✅ Arquivo `Tests.http` criado para facilitar testes

## 🚀 Como Iniciar a API

### Opção 1: Via Terminal
```powershell
cd src/BankMore.Auth.API
dotnet run
```

### Opção 2: Via Visual Studio / Rider
- Pressione F5 ou clique em "Run"

### Opção 3: Via VS Code
- Use o comando: `dotnet watch run` (para hot reload)

## 🧪 Como Testar

### 1. Abra o Swagger
```
http://localhost:5276/swagger/index.html
```

### 2. Ou use o arquivo Tests.http
- Abra: `src/BankMore.Auth.API/Tests.http`
- Execute as requisições diretamente do VS Code (com extensão REST Client)

### 3. Exemplo de Teste Manual (PowerShell)
```powershell
# Criar conta corrente
$body = @{
    nome = "João da Silva"
    numero = 1001
    senha = "senha123"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5276/api/contacorrente" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

## 📊 Banco de Dados

### Connection String (Local - SQL Server)
```
Server=localhost\SQLEXPRESS;Database=BankMoreDb;Integrated Security=True;
```

### Tabela Criada
```sql
contacorrente (
    idcontacorrente UNIQUEIDENTIFIER PRIMARY KEY,
    numero INT NOT NULL,
    nome NVARCHAR(200) NOT NULL,
    ativo BIT NOT NULL DEFAULT 1,
    senha NVARCHAR(200) NOT NULL,
    salt NVARCHAR(200) NOT NULL,
    saldo DECIMAL(18,2) NOT NULL DEFAULT 0,
    criadoEm DATETIME2 NOT NULL,
    atualizadoEm DATETIME2 NULL
)
```

## ⚠️ Limitações Atuais

### Endpoints NÃO Disponíveis (aguardando migrations):
- ❌ Criar/autenticar usuário (tabela `usuarios` não existe)
- ❌ Criar movimento (tabela `movimento` não existe)
- ❌ Realizar transferência (tabela `transferencia` não existe)

### Endpoints DISPONÍVEIS:
- ✅ **POST** `/api/contacorrente` - Criar conta corrente
- ✅ **GET** `/api/contacorrente/contas/{id}/saldo` - Obter saldo

## 🎯 Próximos Passos

1. **Testar API** ← **VOCÊ ESTÁ AQUI**
2. Criar migrations para: Usuario, Movimento, Transferencia, Idempotencia
3. Adicionar índices para otimização
4. Configurar seeds de dados iniciais

## 💡 Dicas

- A senha é automaticamente criptografada com BCrypt
- O salt é gerado automaticamente
- O saldo inicial é sempre 0
- A conta é criada como ativa (ativo = true)
- Use números únicos para as contas (validação automática)

---

**Status**: ✅ API pronta para testes de Conta Corrente  
**Próximo**: Avisar quando estiver tudo funcionando para criar as outras migrations

