using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSedeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SedeConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SedeValor = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Orden = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SedeConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SedeConfigs_TenantId",
                table: "SedeConfigs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SedeConfigs_TenantId_SedeValor",
                table: "SedeConfigs",
                columns: new[] { "TenantId", "SedeValor" },
                unique: true);

            // Seed: sedes iniciales de Chetango (tenant principal de la plataforma).
            // El TenantId de Chetango es el GUID configurado en producción.
            // SedeValor 1 = Medellin, SedeValor 2 = Manizales (mapeo al enum Sede existente).
            var chetangoTenantId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
            migrationBuilder.InsertData(
                table: "SedeConfigs",
                columns: new[] { "Id", "TenantId", "SedeValor", "Nombre", "Activa", "EsDefault", "Orden" },
                values: new object[,]
                {
                    { Guid.NewGuid(), chetangoTenantId, 1, "Medellín",  true, true,  1 },
                    { Guid.NewGuid(), chetangoTenantId, 2, "Manizales", true, false, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SedeConfigs");
        }
    }
}
