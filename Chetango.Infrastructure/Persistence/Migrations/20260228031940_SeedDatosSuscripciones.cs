using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatosSuscripciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar Tenant de Corporación Chetango (primer cliente)
            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { 
                    "Id", "Nombre", "Subdomain", "Plan", "Estado", 
                    "FechaRegistro", "FechaVencimientoPlan",
                    "MaxSedes", "MaxAlumnos", "MaxProfesores", "MaxStorageMB",
                    "EmailContacto", "TelefonoContacto",
                    "CreadoPor"
                },
                values: new object[] { 
                    Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
                    "Corporación Chetango",
                    "corporacionchetango",
                    "Enterprise",
                    "Activo",
                    new DateTime(2024, 1, 1),
                    new DateTime(2026, 12, 31), // Vencimiento de prueba
                    99999, // Ilimitado
                    99999, // Ilimitado
                    99999, // Ilimitado
                    999999, // Ilimitado
                    "chetango.corporacion@corporacionchetango.com",
                    "+57 300 123 4567",
                    "MIGRATION_SEED"
                });
            
            // Insertar configuración de pago inicial (datos bancarios de Aphellion)
            migrationBuilder.InsertData(
                table: "ConfiguracionPagos",
                columns: new[] { 
                    "Banco", "TipoCuenta", "NumeroCuenta", "Titular", "NIT",
                    "Activo", "MostrarEnPortal", "CreadoPor"
                },
                values: new object[] { 
                    "Bancolombia",
                    "Ahorros",
                    "123-456-789", // Actualizar con cuenta real
                    "Aphellion SAS",
                    "900.123.456-7", // Actualizar con NIT real
                    true,
                    true,
                    "MIGRATION_SEED"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar datos semilla
            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"));
            
            migrationBuilder.Sql("DELETE FROM ConfiguracionPagos WHERE CreadoPor = 'MIGRATION_SEED'");
        }
    }
}
