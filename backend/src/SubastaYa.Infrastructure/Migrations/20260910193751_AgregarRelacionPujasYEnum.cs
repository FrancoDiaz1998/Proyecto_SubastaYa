using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionPujasYEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pujas_Usuarios_UsuarioId",
                table: "pujas");

            migrationBuilder.DropIndex(
                name: "IX_pujas_UsuarioId",
                table: "pujas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "pujas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "pujas",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 1L,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 2L,
                column: "UsuarioId",
                value: null);

            migrationBuilder.UpdateData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 3L,
                column: "UsuarioId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_pujas_UsuarioId",
                table: "pujas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_pujas_Usuarios_UsuarioId",
                table: "pujas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
