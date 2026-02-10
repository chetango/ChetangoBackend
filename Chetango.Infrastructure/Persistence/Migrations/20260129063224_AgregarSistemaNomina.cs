using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaNomina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClasesProfesores",
                columns: table => new
                {
                    IdClaseProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRolEnClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TarifaProgramada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorAdicional = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ConceptoAdicional = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadoPorIdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasesProfesores", x => x.IdClaseProfesor);
                    table.ForeignKey(
                        name: "FK_ClasesProfesores_Clases_IdClase",
                        column: x => x.IdClase,
                        principalTable: "Clases",
                        principalColumn: "IdClase",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClasesProfesores_Profesores_IdProfesor",
                        column: x => x.IdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "IdProfesor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClasesProfesores_RolesEnClase_IdRolEnClase",
                        column: x => x.IdRolEnClase,
                        principalTable: "RolesEnClase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClasesProfesores_Usuarios_AprobadoPorIdUsuario",
                        column: x => x.AprobadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiquidacionesMensuales",
                columns: table => new
                {
                    IdLiquidacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Año = table.Column<int>(type: "int", nullable: false),
                    TotalClases = table.Column<int>(type: "int", nullable: false),
                    TotalHoras = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAdicionales = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TotalPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "EnProceso"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreadoPorIdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidacionesMensuales", x => x.IdLiquidacion);
                    table.ForeignKey(
                        name: "FK_LiquidacionesMensuales_Profesores_IdProfesor",
                        column: x => x.IdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "IdProfesor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiquidacionesMensuales_Usuarios_CreadoPorIdUsuario",
                        column: x => x.CreadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_AprobadoPorIdUsuario",
                table: "ClasesProfesores",
                column: "AprobadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_EstadoPago",
                table: "ClasesProfesores",
                column: "EstadoPago");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_FechaAprobacion",
                table: "ClasesProfesores",
                column: "FechaAprobacion");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_IdClase_IdProfesor",
                table: "ClasesProfesores",
                columns: new[] { "IdClase", "IdProfesor" });

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_IdProfesor",
                table: "ClasesProfesores",
                column: "IdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesProfesores_IdRolEnClase",
                table: "ClasesProfesores",
                column: "IdRolEnClase");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionesMensuales_CreadoPorIdUsuario",
                table: "LiquidacionesMensuales",
                column: "CreadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionesMensuales_Estado",
                table: "LiquidacionesMensuales",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionesMensuales_IdProfesor_Mes_Año",
                table: "LiquidacionesMensuales",
                columns: new[] { "IdProfesor", "Mes", "Año" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClasesProfesores");

            migrationBuilder.DropTable(
                name: "LiquidacionesMensuales");
        }
    }
}
