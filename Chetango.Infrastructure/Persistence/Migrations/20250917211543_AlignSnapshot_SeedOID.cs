using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignSnapshot_SeedOID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure OID document type exists (idempotent)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [TiposDocumento] WHERE [Nombre] = N'OID')
BEGIN
    INSERT INTO [TiposDocumento] ([Id], [Nombre]) VALUES ('44444444-1111-2222-3333-555555555555', N'OID');
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove only the row we inserted (safe down)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [TiposDocumento] WHERE [Id] = '44444444-1111-2222-3333-555555555555' AND [Nombre] = N'OID')
BEGIN
    DELETE FROM [TiposDocumento] WHERE [Id] = '44444444-1111-2222-3333-555555555555';
END
");
        }
    }
}
