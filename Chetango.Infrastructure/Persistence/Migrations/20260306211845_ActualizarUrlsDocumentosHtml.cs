using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUrlsDocumentosHtml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000001-0000-0000-0000-000000000001"),
                column: "UrlDocumento",
                value: "/docs/terminos-v1.0.html");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000002-0000-0000-0000-000000000002"),
                column: "UrlDocumento",
                value: "/docs/dpa-v1.0.html");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000003-0000-0000-0000-000000000003"),
                column: "UrlDocumento",
                value: "/docs/politica-privacidad-v1.0.html");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000004-0000-0000-0000-000000000004"),
                column: "UrlDocumento",
                value: "/docs/aviso-privacidad-v1.0.html");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000001-0000-0000-0000-000000000001"),
                column: "UrlDocumento",
                value: "/docs/terminos-v1.0.pdf");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000002-0000-0000-0000-000000000002"),
                column: "UrlDocumento",
                value: "/docs/dpa-v1.0.pdf");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000003-0000-0000-0000-000000000003"),
                column: "UrlDocumento",
                value: "/docs/politica-privacidad-v1.0.pdf");

            migrationBuilder.UpdateData(
                table: "VersionesDocumentoLegal",
                keyColumn: "Id",
                keyValue: new Guid("bb000004-0000-0000-0000-000000000004"),
                column: "UrlDocumento",
                value: "/docs/aviso-privacidad-v1.0.pdf");
        }
    }
}
