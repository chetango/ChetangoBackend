using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTarifaActualProfesor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TarifaActual",
                table: "Profesores",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TarifaActual",
                table: "Profesores");
        }
    }
}
