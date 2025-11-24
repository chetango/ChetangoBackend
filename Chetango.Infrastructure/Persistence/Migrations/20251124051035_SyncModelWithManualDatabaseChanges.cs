using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithManualDatabaseChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Paquetes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Paquetes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacion",
                table: "Paquetes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "SUSER_SNAME()");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioModificacion",
                table: "Paquetes",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Pagos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacion",
                table: "Pagos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "SUSER_SNAME()");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioModificacion",
                table: "Pagos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Asistencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Asistencias",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacion",
                table: "Asistencias",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValueSql: "SUSER_SNAME()");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioModificacion",
                table: "Asistencias",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInscripcion",
                table: "Alumnos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "IdEstado",
                table: "Alumnos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "EstadosAlumno",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosAlumno", x => x.IdEstado);
                });

            migrationBuilder.InsertData(
                table: "EstadosAlumno",
                columns: new[] { "IdEstado", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Alumno activo asistiendo a clases", "Activo" },
                    { 2, "Alumno que dejó de asistir temporalmente", "Inactivo" },
                    { 3, "Alumno suspendido por razones administrativas", "Suspendido" },
                    { 4, "Alumno que se retiró definitivamente", "Retirado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_IdEstado",
                table: "Alumnos",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosAlumno_Nombre",
                table: "EstadosAlumno",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alumnos_EstadosAlumno_IdEstado",
                table: "Alumnos",
                column: "IdEstado",
                principalTable: "EstadosAlumno",
                principalColumn: "IdEstado",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alumnos_EstadosAlumno_IdEstado",
                table: "Alumnos");

            migrationBuilder.DropTable(
                name: "EstadosAlumno");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_IdEstado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacion",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacion",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacion",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacion",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "FechaInscripcion",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "IdEstado",
                table: "Alumnos");
        }
    }
}
