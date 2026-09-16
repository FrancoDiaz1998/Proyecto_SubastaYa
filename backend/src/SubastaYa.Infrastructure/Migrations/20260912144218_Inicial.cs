using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    url_icono = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    fecha_registro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "billeteras",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saldo_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    saldo_retenido = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billeteras", x => x.id);
                    table.CheckConstraint("ck_billeteras_saldos", "saldo_total >= 0 AND saldo_retenido >= 0 AND saldo_retenido <= saldo_total");
                    table.ForeignKey(
                        name: "FK_billeteras_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registros_auditoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_entidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidad_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    detalles_json = table.Column<string>(type: "jsonb", nullable: false),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_auditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_registros_auditoria_usuarios_usuario_actor_id",
                        column: x => x.usuario_actor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subastas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vendedor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_id = table.Column<int>(type: "integer", nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    url_imagen = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    precio_base = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    incremento_minimo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha_inicio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subastas", x => x.id);
                    table.CheckConstraint("ck_subastas_fechas", "fecha_fin > fecha_inicio");
                    table.CheckConstraint("ck_subastas_precios_positivos", "precio_base > 0 AND incremento_minimo > 0");
                    table.ForeignKey(
                        name: "FK_subastas_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subastas_usuarios_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_billetera",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    billetera_id = table.Column<int>(type: "integer", nullable: false),
                    subasta_id = table.Column<int>(type: "integer", nullable: true),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    operacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos_billetera", x => x.id);
                    table.CheckConstraint("ck_movimientos_billetera_monto", "monto > 0");
                    table.ForeignKey(
                        name: "FK_movimientos_billetera_billeteras_billetera_id",
                        column: x => x.billetera_id,
                        principalTable: "billeteras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_movimientos_billetera_subastas_subasta_id",
                        column: x => x.subasta_id,
                        principalTable: "subastas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pujas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subasta_id = table.Column<int>(type: "integer", nullable: false),
                    postor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pujas", x => x.id);
                    table.CheckConstraint("ck_pujas_monto", "monto > 0");
                    table.ForeignKey(
                        name: "FK_pujas_subastas_subasta_id",
                        column: x => x.subasta_id,
                        principalTable: "subastas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pujas_usuarios_postor_id",
                        column: x => x.postor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subasta_id = table.Column<int>(type: "integer", nullable: false),
                    comprador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendedor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    precio_final = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.id);
                    table.CheckConstraint("ck_ventas_precio_final", "precio_final > 0");
                    table.ForeignKey(
                        name: "FK_ventas_subastas_subasta_id",
                        column: x => x.subasta_id,
                        principalTable: "subastas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ventas_usuarios_comprador_id",
                        column: x => x.comprador_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ventas_usuarios_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ux_billeteras_usuario_id",
                table: "billeteras",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_categorias_nombre",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_billetera_fecha",
                table: "movimientos_billetera",
                columns: new[] { "billetera_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_movimientos_billetera_subasta_id",
                table: "movimientos_billetera",
                column: "subasta_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_operacion_id",
                table: "movimientos_billetera",
                column: "operacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_pujas_postor_id",
                table: "pujas",
                column: "postor_id");

            migrationBuilder.CreateIndex(
                name: "ix_pujas_subasta_fecha",
                table: "pujas",
                columns: new[] { "subasta_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "ix_pujas_subasta_monto",
                table: "pujas",
                columns: new[] { "subasta_id", "monto" });

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_entidad_fecha",
                table: "registros_auditoria",
                columns: new[] { "tipo_entidad", "entidad_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_registros_auditoria_usuario_actor_id",
                table: "registros_auditoria",
                column: "usuario_actor_id");

            migrationBuilder.CreateIndex(
                name: "ix_subastas_categoria_estado",
                table: "subastas",
                columns: new[] { "categoria_id", "estado" });

            migrationBuilder.CreateIndex(
                name: "ix_subastas_estado",
                table: "subastas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "ix_subastas_fecha_fin",
                table: "subastas",
                column: "fecha_fin");

            migrationBuilder.CreateIndex(
                name: "IX_subastas_vendedor_id",
                table: "subastas",
                column: "vendedor_id");

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ventas_comprador_id",
                table: "ventas",
                column: "comprador_id");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_vendedor_id",
                table: "ventas",
                column: "vendedor_id");

            migrationBuilder.CreateIndex(
                name: "ux_ventas_subasta_id",
                table: "ventas",
                column: "subasta_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimientos_billetera");

            migrationBuilder.DropTable(
                name: "pujas");

            migrationBuilder.DropTable(
                name: "registros_auditoria");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "billeteras");

            migrationBuilder.DropTable(
                name: "subastas");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
