using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOtrosIngresosGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasGasto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasGasto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasIngreso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasIngreso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtrosGastos",
                columns: table => new
                {
                    IdOtroGasto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sede = table.Column<int>(type: "int", nullable: false),
                    IdCategoriaGasto = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UrlFactura = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumeroFactura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValueSql: "SUSER_SNAME()"),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtrosGastos", x => x.IdOtroGasto);
                    table.ForeignKey(
                        name: "FK_OtrosGastos_CategoriasGasto_IdCategoriaGasto",
                        column: x => x.IdCategoriaGasto,
                        principalTable: "CategoriasGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OtrosIngresos",
                columns: table => new
                {
                    IdOtroIngreso = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sede = table.Column<int>(type: "int", nullable: false),
                    IdCategoriaIngreso = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UrlComprobante = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, defaultValueSql: "SUSER_SNAME()"),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtrosIngresos", x => x.IdOtroIngreso);
                    table.ForeignKey(
                        name: "FK_OtrosIngresos_CategoriasIngreso_IdCategoriaIngreso",
                        column: x => x.IdCategoriaIngreso,
                        principalTable: "CategoriasIngreso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CategoriasGasto",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("f1111111-0000-0000-0000-000000000001"), "Pago mensual del local o salón", "Arriendo" },
                    { new Guid("f1111111-0000-0000-0000-000000000002"), "Luz, agua, internet, teléfono", "Servicios Públicos" },
                    { new Guid("f1111111-0000-0000-0000-000000000003"), "Reparaciones, mantenimiento de equipos e instalaciones", "Mantenimiento" },
                    { new Guid("f1111111-0000-0000-0000-000000000004"), "Publicidad, redes sociales, diseño gráfico", "Marketing" },
                    { new Guid("f1111111-0000-0000-0000-000000000005"), "Material de oficina, limpieza, consumibles", "Suministros" },
                    { new Guid("f1111111-0000-0000-0000-000000000006"), "Espejos, barras, sonido, iluminación", "Equipamiento" },
                    { new Guid("f1111111-0000-0000-0000-000000000007"), "Impuestos, seguros, trámites legales", "Impuestos y Seguros" },
                    { new Guid("f1111111-0000-0000-0000-000000000008"), "Transporte de profesores o materiales", "Transporte" },
                    { new Guid("f1111111-0000-0000-0000-000000000009"), "Otros gastos no clasificados", "Otros" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasIngreso",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("e1111111-0000-0000-0000-000000000001"), "Ingresos por eventos especiales, shows, presentaciones", "Eventos" },
                    { new Guid("e1111111-0000-0000-0000-000000000002"), "Ingresos por alquiler del salón para eventos externos", "Alquiler de Espacio" },
                    { new Guid("e1111111-0000-0000-0000-000000000003"), "Venta de camisetas, zapatos, accesorios de danza", "Mercancía" },
                    { new Guid("e1111111-0000-0000-0000-000000000004"), "Presentaciones privadas contratadas", "Shows Privados" },
                    { new Guid("e1111111-0000-0000-0000-000000000005"), "Talleres impartidos fuera de la academia", "Talleres Externos" },
                    { new Guid("e1111111-0000-0000-0000-000000000006"), "Ingresos por patrocinios de marcas o empresas", "Patrocinios" },
                    { new Guid("e1111111-0000-0000-0000-000000000007"), "Otros ingresos no clasificados", "Otros" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasGasto_Nombre",
                table: "CategoriasGasto",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasIngreso_Nombre",
                table: "CategoriasIngreso",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtrosGastos_Fecha",
                table: "OtrosGastos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosGastos_Fecha_Sede",
                table: "OtrosGastos",
                columns: new[] { "Fecha", "Sede" });

            migrationBuilder.CreateIndex(
                name: "IX_OtrosGastos_IdCategoriaGasto",
                table: "OtrosGastos",
                column: "IdCategoriaGasto");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosGastos_Sede",
                table: "OtrosGastos",
                column: "Sede");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosIngresos_Fecha",
                table: "OtrosIngresos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosIngresos_Fecha_Sede",
                table: "OtrosIngresos",
                columns: new[] { "Fecha", "Sede" });

            migrationBuilder.CreateIndex(
                name: "IX_OtrosIngresos_IdCategoriaIngreso",
                table: "OtrosIngresos",
                column: "IdCategoriaIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosIngresos_Sede",
                table: "OtrosIngresos",
                column: "Sede");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtrosGastos");

            migrationBuilder.DropTable(
                name: "OtrosIngresos");

            migrationBuilder.DropTable(
                name: "CategoriasGasto");

            migrationBuilder.DropTable(
                name: "CategoriasIngreso");
        }
    }
}
