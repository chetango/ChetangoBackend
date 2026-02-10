using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarModeloCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Comentamos los cambios que ya existen en BD para evitar errores
            // migrationBuilder.AlterColumn<Guid>(
            //     name: "IdAlumno",
            //     table: "Pagos",
            //     type: "uniqueidentifier",
            //     nullable: true,
            //     oldClrType: typeof(Guid),
            //     oldType: "uniqueidentifier");

            // migrationBuilder.AddColumn<string>(
            //     name: "TipoAudiencia",
            //     table: "Eventos",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<bool>(
            //     name: "Confirmado",
            //     table: "Asistencias",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<DateTime>(
            //     name: "FechaConfirmacion",
            //     table: "Asistencias",
            //     type: "datetime2",
            //     nullable: true);

            migrationBuilder.CreateTable(
                name: "CodigosReferido",
                columns: table => new
                {
                    IdCodigo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VecesUsado = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BeneficioReferidor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BeneficioNuevoAlumno = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosReferido", x => x.IdCodigo);
                    table.ForeignKey(
                        name: "FK_CodigosReferido_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesClasePrivada",
                columns: table => new
                {
                    IdSolicitud = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTipoClaseDeseado = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoClaseDeseado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaPreferida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraPreferida = table.Column<TimeSpan>(type: "time", nullable: true),
                    ObservacionesAlumno = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuarioRespondio = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MensajeRespuesta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdClaseCreada = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesClasePrivada", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_SolicitudesClasePrivada_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesRenovacionPaquete",
                columns: table => new
                {
                    IdSolicitud = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPaqueteActual = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdTipoPaqueteDeseado = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoPaqueteDeseado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MensajeAlumno = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuarioRespondio = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MensajeRespuesta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdPaqueteCreado = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesRenovacionPaquete", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_SolicitudesRenovacionPaquete_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsosCodigoReferido",
                columns: table => new
                {
                    IdUso = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCodigoReferido = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumnoReferidor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumnoNuevo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaUso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    BeneficioAplicadoReferidor = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaBeneficioReferidor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BeneficioAplicadoNuevo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaBeneficioNuevo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsosCodigoReferido", x => x.IdUso);
                    table.ForeignKey(
                        name: "FK_UsosCodigoReferido_Alumnos_IdAlumnoNuevo",
                        column: x => x.IdAlumnoNuevo,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsosCodigoReferido_Alumnos_IdAlumnoReferidor",
                        column: x => x.IdAlumnoReferidor,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsosCodigoReferido_CodigosReferido_IdCodigoReferido",
                        column: x => x.IdCodigoReferido,
                        principalTable: "CodigosReferido",
                        principalColumn: "IdCodigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodigosReferido_Activo",
                table: "CodigosReferido",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CodigosReferido_Codigo",
                table: "CodigosReferido",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodigosReferido_IdAlumno",
                table: "CodigosReferido",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesClasePrivada_Estado",
                table: "SolicitudesClasePrivada",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesClasePrivada_FechaSolicitud",
                table: "SolicitudesClasePrivada",
                column: "FechaSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesClasePrivada_IdAlumno",
                table: "SolicitudesClasePrivada",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRenovacionPaquete_Estado",
                table: "SolicitudesRenovacionPaquete",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRenovacionPaquete_FechaSolicitud",
                table: "SolicitudesRenovacionPaquete",
                column: "FechaSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRenovacionPaquete_IdAlumno",
                table: "SolicitudesRenovacionPaquete",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_UsosCodigoReferido_Estado",
                table: "UsosCodigoReferido",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_UsosCodigoReferido_FechaUso",
                table: "UsosCodigoReferido",
                column: "FechaUso");

            migrationBuilder.CreateIndex(
                name: "IX_UsosCodigoReferido_IdAlumnoNuevo",
                table: "UsosCodigoReferido",
                column: "IdAlumnoNuevo");

            migrationBuilder.CreateIndex(
                name: "IX_UsosCodigoReferido_IdAlumnoReferidor",
                table: "UsosCodigoReferido",
                column: "IdAlumnoReferidor");

            migrationBuilder.CreateIndex(
                name: "IX_UsosCodigoReferido_IdCodigoReferido",
                table: "UsosCodigoReferido",
                column: "IdCodigoReferido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesClasePrivada");

            migrationBuilder.DropTable(
                name: "SolicitudesRenovacionPaquete");

            migrationBuilder.DropTable(
                name: "UsosCodigoReferido");

            migrationBuilder.DropTable(
                name: "CodigosReferido");

            // Comentamos los cambios que ya existen en BD
            // migrationBuilder.DropColumn(
            //     name: "TipoAudiencia",
            //     table: "Eventos");

            // migrationBuilder.DropColumn(
            //     name: "Confirmado",
            //     table: "Asistencias");

            // migrationBuilder.DropColumn(
            //     name: "FechaConfirmacion",
            //     table: "Asistencias");

            // migrationBuilder.AlterColumn<Guid>(
            //     name: "IdAlumno",
            //     table: "Pagos",
            //     type: "uniqueidentifier",
            //     nullable: false,
            //     defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            //     oldClrType: typeof(Guid),
            //     oldType: "uniqueidentifier",
            //     oldNullable: true);
        }
    }
}
