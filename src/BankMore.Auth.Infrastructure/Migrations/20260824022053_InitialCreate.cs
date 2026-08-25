using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contacorrente",
                columns: table => new
                {
                    idcontacorrente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    numero = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    senha = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    salt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    criadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    atualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaCorrente", x => x.idcontacorrente);
                });

            migrationBuilder.CreateTable(
                name: "idempotencia",
                columns: table => new
                {
                    chave_idempotencia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    requisicao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    resultado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    criadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idempotencia", x => x.chave_idempotencia);
                });

            migrationBuilder.CreateTable(
                name: "movimento",
                columns: table => new
                {
                    idmovimento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    idcontacorrente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    datamovimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipomovimento = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    chave_idempotencia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimento", x => x.idmovimento);
                });

            migrationBuilder.CreateTable(
                name: "transferencia",
                columns: table => new
                {
                    idtransferencia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    idcontaorigem = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    idcontadestino = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    datamovimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencia", x => x.idtransferencia);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    senhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    criadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_movimento_conta",
                table: "movimento",
                column: "idcontacorrente");

            migrationBuilder.CreateIndex(
                name: "idx_movimento_data",
                table: "movimento",
                column: "datamovimento");

            migrationBuilder.CreateIndex(
                name: "idx_movimento_idempotencia",
                table: "movimento",
                column: "chave_idempotencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_transferencia_destino",
                table: "transferencia",
                column: "idcontadestino");

            migrationBuilder.CreateIndex(
                name: "idx_transferencia_origem",
                table: "transferencia",
                column: "idcontaorigem");

            migrationBuilder.CreateIndex(
                name: "idx_usuarios_cpf",
                table: "usuarios",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacorrente");

            migrationBuilder.DropTable(
                name: "idempotencia");

            migrationBuilder.DropTable(
                name: "movimento");

            migrationBuilder.DropTable(
                name: "transferencia");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
