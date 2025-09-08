-- Script de criação das tabelas do sistema BankMore
-- Compatível com MySQL 8.0+

-- Tabela de Usuários
CREATE TABLE IF NOT EXISTS usuarios (
    id CHAR(36) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    senhaHash VARCHAR(255) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Contas Correntes
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente CHAR(36) PRIMARY KEY,
    numero INT NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,
    senha VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    saldo DECIMAL(15,2) NOT NULL DEFAULT 0.00,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizadoEm DATETIME NULL
);

-- Tabela de Movimentos
CREATE TABLE IF NOT EXISTS movimento (
    idmovimento CHAR(36) PRIMARY KEY,
    idcontacorrente CHAR(36) NOT NULL,
    datamovimento DATETIME NOT NULL,
    tipomovimento CHAR(1) NOT NULL CHECK (tipomovimento IN ('C', 'D')),
    valor DECIMAL(15,2) NOT NULL,
    chave_idempotencia VARCHAR(255) NOT NULL UNIQUE,
    descricao VARCHAR(255) NULL,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de Transferências
CREATE TABLE IF NOT EXISTS transferencia (
    idtransferencia CHAR(36) PRIMARY KEY,
    idcontaorigem CHAR(36) NOT NULL,
    idcontadestino CHAR(36) NOT NULL,
    valor DECIMAL(15,2) NOT NULL,
    chave_idempotencia VARCHAR(255) NOT NULL UNIQUE,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDENTE',
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processadoEm DATETIME NULL,
    FOREIGN KEY (idcontaorigem) REFERENCES contacorrente(idcontacorrente),
    FOREIGN KEY (idcontadestino) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de Controle de Idempotência
CREATE TABLE IF NOT EXISTS idempotencia (
    chave VARCHAR(255) PRIMARY KEY,
    tipo_operacao VARCHAR(50) NOT NULL,
    dados_operacao JSON NULL,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    processadoEm DATETIME NULL
);

-- Índices para melhor performance
CREATE INDEX idx_usuarios_cpf ON usuarios(cpf);
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_contacorrente_numero ON contacorrente(numero);
CREATE INDEX idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX idx_movimento_data ON movimento(datamovimento);
CREATE INDEX idx_movimento_idempotencia ON movimento(chave_idempotencia);
CREATE INDEX idx_transferencia_origem ON transferencia(idcontaorigem);
CREATE INDEX idx_transferencia_destino ON transferencia(idcontadestino);
CREATE INDEX idx_transferencia_idempotencia ON transferencia(chave_idempotencia);
CREATE INDEX idx_idempotencia_tipo ON idempotencia(tipo_operacao);
