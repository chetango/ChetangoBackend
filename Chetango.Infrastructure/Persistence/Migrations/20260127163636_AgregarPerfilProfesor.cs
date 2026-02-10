using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPerfilProfesor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertasCambios",
                table: "Profesores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Biografia",
                table: "Profesores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Especialidades",
                table: "Profesores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionesEmail",
                table: "Profesores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecordatoriosClase",
                table: "Profesores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertasCambios",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "Biografia",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "Especialidades",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "NotificacionesEmail",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "RecordatoriosClase",
                table: "Profesores");
        }
    }
}
