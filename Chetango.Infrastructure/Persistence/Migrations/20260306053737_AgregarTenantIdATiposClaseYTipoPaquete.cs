using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTenantIdATiposClaseYTipoPaquete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TiposPaquete_Nombre",
                table: "TiposPaquete");

            migrationBuilder.DropIndex(
                name: "IX_TiposClase_Nombre",
                table: "TiposClase");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TiposPaquete",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TiposClase",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposClase",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "TenantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposClase",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "TenantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposClase",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "TenantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposPaquete",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "TenantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposPaquete",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "TenantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposPaquete",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"),
                column: "TenantId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPaquete_TenantId_Nombre",
                table: "TiposPaquete",
                columns: new[] { "TenantId", "Nombre" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TiposClase_TenantId_Nombre",
                table: "TiposClase",
                columns: new[] { "TenantId", "Nombre" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TiposPaquete_TenantId_Nombre",
                table: "TiposPaquete");

            migrationBuilder.DropIndex(
                name: "IX_TiposClase_TenantId_Nombre",
                table: "TiposClase");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TiposPaquete");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TiposClase");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPaquete_Nombre",
                table: "TiposPaquete",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposClase_Nombre",
                table: "TiposClase",
                column: "Nombre",
                unique: true);
        }
    }
}
