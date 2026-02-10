using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoTipoAsistenciaConDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "IdPaqueteUsado",
                table: "Asistencias",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "IdTipoAsistencia",
                table: "Asistencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TiposAsistencia",
                columns: table => new
                {
                    IdTipoAsistencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequierePaquete = table.Column<bool>(type: "bit", nullable: false),
                    DescontarClase = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAsistencia", x => x.IdTipoAsistencia);
                });

            migrationBuilder.InsertData(
                table: "TiposAsistencia",
                columns: new[] { "IdTipoAsistencia", "Activo", "DescontarClase", "Descripcion", "Nombre", "RequierePaquete" },
                values: new object[,]
                {
                    { 1, true, true, "Asistencia normal con paquete activo", "Normal", true },
                    { 2, true, false, "Clase de cortesía sin descuento de paquete", "Cortesía", false },
                    { 3, true, false, "Clase de prueba para nuevos alumnos", "Clase de Prueba", false },
                    { 4, true, false, "Clase de recuperación por inasistencia justificada", "Recuperación", true }
                });

            // Actualizar registros existentes de Asistencias a tipo Normal (1)
            migrationBuilder.Sql(@"
                UPDATE Asistencias 
                SET IdTipoAsistencia = 1 
                WHERE IdTipoAsistencia = 0 OR IdTipoAsistencia IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdTipoAsistencia",
                table: "Asistencias",
                column: "IdTipoAsistencia");

            migrationBuilder.CreateIndex(
                name: "IX_TiposAsistencia_Nombre",
                table: "TiposAsistencia",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_TiposAsistencia_IdTipoAsistencia",
                table: "Asistencias",
                column: "IdTipoAsistencia",
                principalTable: "TiposAsistencia",
                principalColumn: "IdTipoAsistencia",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_TiposAsistencia_IdTipoAsistencia",
                table: "Asistencias");

            migrationBuilder.DropTable(
                name: "TiposAsistencia");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_IdTipoAsistencia",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "IdTipoAsistencia",
                table: "Asistencias");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdPaqueteUsado",
                table: "Asistencias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
