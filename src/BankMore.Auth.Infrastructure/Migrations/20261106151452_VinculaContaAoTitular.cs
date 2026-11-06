using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VinculaContaAoTitular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "idusuario",
                table: "contacorrente",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_contacorrente_numero",
                table: "contacorrente",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_contacorrente_usuario",
                table: "contacorrente",
                column: "idusuario",
                unique: true,
                filter: "[idusuario] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_contacorrente_numero",
                table: "contacorrente");

            migrationBuilder.DropIndex(
                name: "idx_contacorrente_usuario",
                table: "contacorrente");

            migrationBuilder.DropColumn(
                name: "idusuario",
                table: "contacorrente");
        }
    }
}
