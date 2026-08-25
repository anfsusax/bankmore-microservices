-- Script de criação das tabelas do sistema BankMore
-- Compatível com PostgreSQL 15/16+

-- Tabela de Usuários
CREATE TABLE IF NOT EXISTS usuarios (
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    "senhaHash" VARCHAR(255) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,
    "criadoEm" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Contas Correntes
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente UUID PRIMARY KEY,
    numero INT NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,
    senha VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    saldo NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    "criadoEm" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "atualizadoEm" TIMESTAMP NULL
);

-- Tabela de Movimentos
CREATE TABLE IF NOT EXISTS movimento (
    idmovimento UUID PRIMARY KEY,
    idcontacorrente UUID NOT NULL,
    datamovimento TIMESTAMP NOT NULL,
    tipomovimento CHAR(1) NOT NULL CHECK (tipomovimento IN ('C', 'D')),
    valor NUMERIC(18,2) NOT NULL,
    chave_idempotencia VARCHAR(255) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL,
    "criadoEm" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_movimento_contacorrente FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de Transferências
CREATE TABLE IF NOT EXISTS transferencia (
    idtransferencia UUID PRIMARY KEY,
    idcontaorigem UUID NOT NULL,
    idcontadestino UUID NOT NULL,
    datamovimento TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor NUMERIC(18,2) NOT NULL,
    chave_idempotencia VARCHAR(255) NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDENTE',
    "criadoEm" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "processadoEm" TIMESTAMP NULL,
    CONSTRAINT fk_transferencia_origem FOREIGN KEY (idcontaorigem) REFERENCES contacorrente(idcontacorrente),
    CONSTRAINT fk_transferencia_destino FOREIGN KEY (idcontadestino) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de Controle de Idempotência
CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia VARCHAR(255) PRIMARY KEY,
    requisicao TEXT NULL,
    resultado TEXT NULL,
    "criadoEm" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_usuarios_cpf ON usuarios(cpf);
CREATE INDEX IF NOT EXISTS idx_usuarios_email ON usuarios(email);
CREATE INDEX IF NOT EXISTS idx_contacorrente_numero ON contacorrente(numero);
CREATE INDEX IF NOT EXISTS idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_movimento_data ON movimento(datamovimento);
CREATE INDEX IF NOT EXISTS idx_movimento_idempotencia ON movimento(chave_idempotencia);
CREATE INDEX IF NOT EXISTS idx_transferencia_origem ON transferencia(idcontaorigem);
CREATE INDEX IF NOT EXISTS idx_transferencia_destino ON transferencia(idcontadestino);
