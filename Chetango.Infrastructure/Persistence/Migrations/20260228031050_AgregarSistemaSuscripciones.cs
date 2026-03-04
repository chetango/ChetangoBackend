using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaSuscripciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionPagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Banco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titular = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NIT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MostrarEnPortal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, defaultValueSql: "SUSER_SNAME()"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionPagos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaVencimientoPlan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxSedes = table.Column<int>(type: "int", nullable: false),
                    MaxAlumnos = table.Column<int>(type: "int", nullable: false),
                    MaxProfesores = table.Column<int>(type: "int", nullable: false),
                    MaxStorageMB = table.Column<int>(type: "int", nullable: false),
                    EmailContacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ColorPrimario = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    ColorSecundario = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    ColorAccent = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    NombreComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FaviconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WompiSubscriptionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StripeCustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MetodoPagoPreferido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ActualizadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagosSuscripcion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComprobanteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NombreArchivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TamanoArchivo = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    AprobadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransaccionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstadoTransaccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, defaultValueSql: "SUSER_SNAME()"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosSuscripcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosSuscripcion_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosSuscripcion_Estado",
                table: "PagosSuscripcion",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_PagosSuscripcion_FechaPago",
                table: "PagosSuscripcion",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosSuscripcion_Referencia",
                table: "PagosSuscripcion",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagosSuscripcion_TenantId",
                table: "PagosSuscripcion",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosSuscripcion_TenantId_FechaPago",
                table: "PagosSuscripcion",
                columns: new[] { "TenantId", "FechaPago" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_EmailContacto",
                table: "Tenants",
                column: "EmailContacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Subdomain",
                table: "Tenants",
                column: "Subdomain",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionPagos");

            migrationBuilder.DropTable(
                name: "PagosSuscripcion");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
