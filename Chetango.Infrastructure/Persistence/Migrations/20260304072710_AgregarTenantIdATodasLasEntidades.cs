using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTenantIdATodasLasEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Profesores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "OtrosIngresos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "OtrosGastos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "LiquidacionesMensuales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Alumnos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_TenantId",
                table: "Profesores",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosIngresos_TenantId",
                table: "OtrosIngresos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OtrosGastos_TenantId",
                table: "OtrosGastos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionesMensuales_TenantId",
                table: "LiquidacionesMensuales",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_TenantId",
                table: "Alumnos",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alumnos_Tenants_TenantId",
                table: "Alumnos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LiquidacionesMensuales_Tenants_TenantId",
                table: "LiquidacionesMensuales",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OtrosGastos_Tenants_TenantId",
                table: "OtrosGastos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OtrosIngresos_Tenants_TenantId",
                table: "OtrosIngresos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profesores_Tenants_TenantId",
                table: "Profesores",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            // ================================================================
            // SEED: Asignar todos los registros existentes a Corporación Chetango
            // Todos los datos previos a multi-tenancy pertenecen a este tenant
            // ================================================================
            var chetangoTenantId = "A1B2C3D4-E5F6-7890-ABCD-EF1234567890";

            migrationBuilder.Sql($"UPDATE Usuarios SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
            migrationBuilder.Sql($"UPDATE Alumnos SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
            migrationBuilder.Sql($"UPDATE Profesores SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
            migrationBuilder.Sql($"UPDATE OtrosIngresos SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
            migrationBuilder.Sql($"UPDATE OtrosGastos SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
            migrationBuilder.Sql($"UPDATE LiquidacionesMensuales SET TenantId = '{chetangoTenantId}' WHERE TenantId IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alumnos_Tenants_TenantId",
                table: "Alumnos");

            migrationBuilder.DropForeignKey(
                name: "FK_LiquidacionesMensuales_Tenants_TenantId",
                table: "LiquidacionesMensuales");

            migrationBuilder.DropForeignKey(
                name: "FK_OtrosGastos_Tenants_TenantId",
                table: "OtrosGastos");

            migrationBuilder.DropForeignKey(
                name: "FK_OtrosIngresos_Tenants_TenantId",
                table: "OtrosIngresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Profesores_Tenants_TenantId",
                table: "Profesores");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Profesores_TenantId",
                table: "Profesores");

            migrationBuilder.DropIndex(
                name: "IX_OtrosIngresos_TenantId",
                table: "OtrosIngresos");

            migrationBuilder.DropIndex(
                name: "IX_OtrosGastos_TenantId",
                table: "OtrosGastos");

            migrationBuilder.DropIndex(
                name: "IX_LiquidacionesMensuales_TenantId",
                table: "LiquidacionesMensuales");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_TenantId",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "OtrosIngresos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "OtrosGastos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LiquidacionesMensuales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Alumnos");
        }
    }
}
