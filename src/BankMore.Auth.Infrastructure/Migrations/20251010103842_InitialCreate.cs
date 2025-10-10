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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacorrente");
        }
    }
}
