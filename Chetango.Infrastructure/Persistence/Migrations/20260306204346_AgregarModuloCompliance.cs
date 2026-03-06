using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActivacion",
                table: "Tenants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompletado",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereReaceptacion",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DocumentosLegales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Destinatario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Admin"),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiereReaceptacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosLegales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VersionesDocumentoLegal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentoLegalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroVersion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UrlDocumento = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ResumenCambios = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsCambioSignificativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionesDocumentoLegal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VersionesDocumentoLegal_DocumentosLegales_DocumentoLegalId",
                        column: x => x.DocumentoLegalId,
                        principalTable: "DocumentosLegales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AceptacionesDocumento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionDocumentoLegalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAceptacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpOrigen = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Contexto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Onboarding")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AceptacionesDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AceptacionesDocumento_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AceptacionesDocumento_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AceptacionesDocumento_VersionesDocumentoLegal_VersionDocumentoLegalId",
                        column: x => x.VersionDocumentoLegalId,
                        principalTable: "VersionesDocumentoLegal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DocumentosLegales",
                columns: new[] { "Id", "Activo", "Codigo", "CreadoPor", "Descripcion", "Destinatario", "EsObligatorio", "FechaCreacion", "Nombre", "RequiereReaceptacion" },
                values: new object[,]
                {
                    { new Guid("aa000001-0000-0000-0000-000000000001"), true, "TERMINOS", "SISTEMA", "Contrato SaaS entre Aphellion y la academia cliente.", "Admin", true, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Términos y Condiciones del Servicio", true },
                    { new Guid("aa000002-0000-0000-0000-000000000002"), true, "DPA", "SISTEMA", "Define las responsabilidades de Aphellion como encargado y de la academia como responsable del tratamiento.", "Admin", true, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Acuerdo de Tratamiento de Datos (DPA)", true }
                });

            migrationBuilder.InsertData(
                table: "DocumentosLegales",
                columns: new[] { "Id", "Activo", "Codigo", "CreadoPor", "Descripcion", "Destinatario", "EsObligatorio", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("aa000003-0000-0000-0000-000000000003"), true, "POLITICA_PRIVACIDAD", "SISTEMA", "Cómo Aphellion maneja los datos comerciales del cliente.", "Admin", true, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Política de Privacidad" },
                    { new Guid("aa000004-0000-0000-0000-000000000004"), true, "AVISO_PRIVACIDAD", "SISTEMA", "Aviso corto para usuarios finales (profesores y alumnos) sobre el tratamiento de sus datos.", "Todos", true, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Aviso de Privacidad" }
                });

            migrationBuilder.InsertData(
                table: "VersionesDocumentoLegal",
                columns: new[] { "Id", "Activa", "CreadoPor", "DocumentoLegalId", "FechaCreacion", "FechaVigencia", "NumeroVersion", "ResumenCambios", "UrlDocumento" },
                values: new object[,]
                {
                    { new Guid("bb000001-0000-0000-0000-000000000001"), true, "SISTEMA", new Guid("aa000001-0000-0000-0000-000000000001"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "1.0", "Versión inicial.", "/docs/terminos-v1.0.pdf" },
                    { new Guid("bb000002-0000-0000-0000-000000000002"), true, "SISTEMA", new Guid("aa000002-0000-0000-0000-000000000002"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "1.0", "Versión inicial.", "/docs/dpa-v1.0.pdf" },
                    { new Guid("bb000003-0000-0000-0000-000000000003"), true, "SISTEMA", new Guid("aa000003-0000-0000-0000-000000000003"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "1.0", "Versión inicial.", "/docs/politica-privacidad-v1.0.pdf" },
                    { new Guid("bb000004-0000-0000-0000-000000000004"), true, "SISTEMA", new Guid("aa000004-0000-0000-0000-000000000004"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "1.0", "Versión inicial.", "/docs/aviso-privacidad-v1.0.pdf" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumento_IdUsuario",
                table: "AceptacionesDocumento",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumento_TenantId",
                table: "AceptacionesDocumento",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumento_TenantId_VersionDocumentoLegalId",
                table: "AceptacionesDocumento",
                columns: new[] { "TenantId", "VersionDocumentoLegalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumento_VersionDocumentoLegalId",
                table: "AceptacionesDocumento",
                column: "VersionDocumentoLegalId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosLegales_Codigo",
                table: "DocumentosLegales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersionesDocumentoLegal_DocumentoLegalId_NumeroVersion",
                table: "VersionesDocumentoLegal",
                columns: new[] { "DocumentoLegalId", "NumeroVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AceptacionesDocumento");

            migrationBuilder.DropTable(
                name: "VersionesDocumentoLegal");

            migrationBuilder.DropTable(
                name: "DocumentosLegales");

            migrationBuilder.DropColumn(
                name: "FechaActivacion",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletado",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "RequiereReaceptacion",
                table: "Tenants");
        }
    }
}
