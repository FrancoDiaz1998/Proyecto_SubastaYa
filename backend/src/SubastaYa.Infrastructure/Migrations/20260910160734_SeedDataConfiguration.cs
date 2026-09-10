using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "FechaRegistro", "NombreCompleto", "PasswordHash" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "vendedor@test.com", new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Usuario Vendedor", "HASH_SIMULADO" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "comprador1@test.com", new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador Líder", "HASH_SIMULADO" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "comprador2@test.com", new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador Habilitado", "HASH_SIMULADO" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "sinfondos@test.com", new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Usuario Sin Fondos", "HASH_SIMULADO" }
                });

            migrationBuilder.InsertData(
                table: "categorias",
                columns: new[] { "id", "nombre", "url_icono" },
                values: new object[,]
                {
                    { 1, "Tecnología", null },
                    { 2, "Coleccionables", null },
                    { 3, "Indumentaria", null },
                    { 4, "Vehículos", null }
                });

            migrationBuilder.InsertData(
                table: "billeteras",
                columns: new[] { "id", "saldo_retenido", "saldo_total", "usuario_id", "UsuarioId1", "version" },
                values: new object[,]
                {
                    { 1, 0m, 0m, new Guid("11111111-1111-1111-1111-111111111111"), null, 1L },
                    { 2, 45000m, 150000m, new Guid("22222222-2222-2222-2222-222222222222"), null, 1L },
                    { 3, 0m, 200000m, new Guid("33333333-3333-3333-3333-333333333333"), null, 1L },
                    { 4, 0m, 500m, new Guid("44444444-4444-4444-4444-444444444444"), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "subastas",
                columns: new[] { "id", "categoria_id", "descripcion", "estado", "fecha_fin", "fecha_inicio", "incremento_minimo", "precio_base", "titulo", "url_imagen", "UsuarioId", "vendedor_id", "version" },
                values: new object[,]
                {
                    { 1, 1, "Excelente estado, 16GB RAM", "Activa", new DateTimeOffset(new DateTime(2026, 9, 10, 12, 25, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5000m, 30000m, "Notebook Gamer Pro", "https://via.placeholder.com/150", null, new Guid("11111111-1111-1111-1111-111111111111"), 1L },
                    { 2, 2, "Edición limitada año 1990", "Activa", new DateTimeOffset(new DateTime(2026, 9, 10, 12, 0, 45, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2000m, 10000m, "Figura Coleccionable Rare", "https://via.placeholder.com/150", null, new Guid("11111111-1111-1111-1111-111111111111"), 1L },
                    { 3, 4, "Pocos kilómetros", "Programada", new DateTimeOffset(new DateTime(2026, 9, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 11, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 50000m, 1000000m, "Auto Deportivo 2020", "https://via.placeholder.com/150", null, new Guid("11111111-1111-1111-1111-111111111111"), 1L },
                    { 4, 1, "Nuevo en caja sellada", "Activa", new DateTimeOffset(new DateTime(2026, 9, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 10, 7, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5000m, 50000m, "Smartphone Ultima Generación", "https://via.placeholder.com/150", null, new Guid("11111111-1111-1111-1111-111111111111"), 1L },
                    { 5, 3, "Talle L", "Activa", new DateTimeOffset(new DateTime(2026, 9, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 9, 10, 7, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2000m, 20000m, "Chaqueta de Cuero Vintage", "https://via.placeholder.com/150", null, new Guid("11111111-1111-1111-1111-111111111111"), 1L }
                });

            migrationBuilder.InsertData(
                table: "movimientos_billetera",
                columns: new[] { "id", "billetera_id", "fecha", "monto", "operacion_id", "subasta_id", "tipo" },
                values: new object[,]
                {
                    { 1L, 2, new DateTimeOffset(new DateTime(2026, 9, 8, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 150000m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "Deposito" },
                    { 2L, 2, new DateTimeOffset(new DateTime(2026, 9, 10, 11, 40, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 45000m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 1, "Retencion" }
                });

            migrationBuilder.InsertData(
                table: "pujas",
                columns: new[] { "id", "fecha", "monto", "postor_id", "subasta_id", "UsuarioId" },
                values: new object[,]
                {
                    { 1L, new DateTimeOffset(new DateTime(2026, 9, 10, 11, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 35000m, new Guid("33333333-3333-3333-3333-333333333333"), 1, null },
                    { 2L, new DateTimeOffset(new DateTime(2026, 9, 10, 11, 40, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 45000m, new Guid("22222222-2222-2222-2222-222222222222"), 1, null },
                    { 3L, new DateTimeOffset(new DateTime(2026, 9, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 60000m, new Guid("22222222-2222-2222-2222-222222222222"), 4, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "movimientos_billetera",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "movimientos_billetera",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "pujas",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "subastas",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "subastas",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "subastas",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "billeteras",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categorias",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categorias",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categorias",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "subastas",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "subastas",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "categorias",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
