using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMultiTenancy : Migration
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
                table: "SolicitudesRenovacionPaquete",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SolicitudesClasePrivada",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Paquetes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Pagos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Eventos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Clases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Asistencias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRenovacionPaquete_TenantId",
                table: "SolicitudesRenovacionPaquete",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesClasePrivada_TenantId",
                table: "SolicitudesClasePrivada",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_TenantId",
                table: "Paquetes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TenantId",
                table: "Pagos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_TenantId",
                table: "Eventos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_TenantId",
                table: "Clases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_TenantId",
                table: "Asistencias",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Tenants_TenantId",
                table: "Asistencias",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clases_Tenants_TenantId",
                table: "Clases",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Tenants_TenantId",
                table: "Eventos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Tenants_TenantId",
                table: "Pagos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Paquetes_Tenants_TenantId",
                table: "Paquetes",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesClasePrivada_Tenants_TenantId",
                table: "SolicitudesClasePrivada",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesRenovacionPaquete_Tenants_TenantId",
                table: "SolicitudesRenovacionPaquete",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            // Asignar TenantId de Corporación Chetango a todos los registros existentes
            var corporacionChetangoId = "A1B2C3D4-E5F6-7890-ABCD-EF1234567890";
            
            migrationBuilder.Sql($@"
                -- Asignar TenantId a Usuarios existentes
                UPDATE Usuarios 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Clases existentes
                UPDATE Clases 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Pagos existentes
                UPDATE Pagos 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Paquetes existentes
                UPDATE Paquetes 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Asistencias existentes
                UPDATE Asistencias 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Eventos existentes
                UPDATE Eventos 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Solicitudes de Clase Privada existentes
                UPDATE SolicitudesClasePrivada 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;

                -- Asignar TenantId a Solicitudes de Renovación existentes
                UPDATE SolicitudesRenovacionPaquete 
                SET TenantId = '{corporacionChetangoId}' 
                WHERE TenantId IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Tenants_TenantId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Clases_Tenants_TenantId",
                table: "Clases");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Tenants_TenantId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Tenants_TenantId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Paquetes_Tenants_TenantId",
                table: "Paquetes");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesClasePrivada_Tenants_TenantId",
                table: "SolicitudesClasePrivada");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesRenovacionPaquete_Tenants_TenantId",
                table: "SolicitudesRenovacionPaquete");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Tenants_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TenantId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesRenovacionPaquete_TenantId",
                table: "SolicitudesRenovacionPaquete");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesClasePrivada_TenantId",
                table: "SolicitudesClasePrivada");

            migrationBuilder.DropIndex(
                name: "IX_Paquetes_TenantId",
                table: "Paquetes");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TenantId",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_TenantId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Clases_TenantId",
                table: "Clases");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_TenantId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SolicitudesRenovacionPaquete");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SolicitudesClasePrivada");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Clases");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Asistencias");
        }
    }
}
