using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AplicarSeedYModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_billeteras_Usuarios_UsuarioId1",
                table: "billeteras");

            migrationBuilder.DropIndex(
                name: "IX_billeteras_UsuarioId1",
                table: "billeteras");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "billeteras");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId1",
                table: "billeteras",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 1,
                column: "UsuarioId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 2,
                column: "UsuarioId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 3,
                column: "UsuarioId1",
                value: null);

            migrationBuilder.UpdateData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 4,
                column: "UsuarioId1",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_billeteras_UsuarioId1",
                table: "billeteras",
                column: "UsuarioId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_billeteras_Usuarios_UsuarioId1",
                table: "billeteras",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
