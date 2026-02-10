using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilAlumnoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertasPaquete",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmergenciaNombre",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmergenciaRelacion",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmergenciaTelefono",
                table: "Alumnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionesEmail",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecordatoriosClase",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertasPaquete",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "ContactoEmergenciaNombre",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "ContactoEmergenciaRelacion",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "ContactoEmergenciaTelefono",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "NotificacionesEmail",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "RecordatoriosClase",
                table: "Alumnos");
        }
    }
}
