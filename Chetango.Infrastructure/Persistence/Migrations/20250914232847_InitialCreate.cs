using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesNotificaciones",
                columns: table => new
                {
                    IdConfig = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnticipacionAlerta = table.Column<int>(type: "int", nullable: false),
                    TextoVencimiento = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TextoAgotamiento = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesNotificaciones", x => x.IdConfig);
                });

            migrationBuilder.CreateTable(
                name: "EstadosAsistencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosAsistencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosNotificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosNotificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPaquete",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPaquete", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesEnClase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesEnClase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposClase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposClase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposPaquete",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPaquete", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposProfesor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposProfesor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdTipoDocumento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdEstadoUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_EstadosUsuario_IdEstadoUsuario",
                        column: x => x.IdEstadoUsuario,
                        principalTable: "EstadosUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_TiposDocumento_IdTipoDocumento",
                        column: x => x.IdTipoDocumento,
                        principalTable: "TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.IdAlumno);
                    table.ForeignKey(
                        name: "FK_Alumnos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.IdAuditoria);
                    table.ForeignKey(
                        name: "FK_Auditorias_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    IdNotificacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Leida = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.IdNotificacion);
                    table.ForeignKey(
                        name: "FK_Notificaciones_EstadosNotificacion_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "EstadosNotificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profesores",
                columns: table => new
                {
                    IdProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTipoProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesores", x => x.IdProfesor);
                    table.ForeignKey(
                        name: "FK_Profesores_TiposProfesor_IdTipoProfesor",
                        column: x => x.IdTipoProfesor,
                        principalTable: "TiposProfesor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Profesores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRol = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => new { x.IdUsuario, x.IdRol });
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdMetodoPago = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nota = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pagos_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_MetodosPago_IdMetodoPago",
                        column: x => x.IdMetodoPago,
                        principalTable: "MetodosPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clases",
                columns: table => new
                {
                    IdClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdTipoClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    IdProfesorPrincipal = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clases", x => x.IdClase);
                    table.ForeignKey(
                        name: "FK_Clases_Profesores_IdProfesorPrincipal",
                        column: x => x.IdProfesorPrincipal,
                        principalTable: "Profesores",
                        principalColumn: "IdProfesor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clases_TiposClase_IdTipoClase",
                        column: x => x.IdTipoClase,
                        principalTable: "TiposClase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TarifasProfesor",
                columns: table => new
                {
                    IdTarifa = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTipoProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRolEnClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValorPorClase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfesorIdProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarifasProfesor", x => x.IdTarifa);
                    table.ForeignKey(
                        name: "FK_TarifasProfesor_Profesores_ProfesorIdProfesor",
                        column: x => x.ProfesorIdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "IdProfesor");
                    table.ForeignKey(
                        name: "FK_TarifasProfesor_RolesEnClase_IdRolEnClase",
                        column: x => x.IdRolEnClase,
                        principalTable: "RolesEnClase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TarifasProfesor_TiposProfesor_IdTipoProfesor",
                        column: x => x.IdTipoProfesor,
                        principalTable: "TiposProfesor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    IdPaquete = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPago = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClasesDisponibles = table.Column<int>(type: "int", nullable: false),
                    ClasesUsadas = table.Column<int>(type: "int", nullable: false),
                    FechaActivacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdTipoPaquete = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValorPaquete = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquetes", x => x.IdPaquete);
                    table.ForeignKey(
                        name: "FK_Paquetes_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Paquetes_EstadosPaquete_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "EstadosPaquete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Paquetes_Pagos_IdPago",
                        column: x => x.IdPago,
                        principalTable: "Pagos",
                        principalColumn: "IdPago",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Paquetes_TiposPaquete_IdTipoPaquete",
                        column: x => x.IdTipoPaquete,
                        principalTable: "TiposPaquete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonitoresClase",
                columns: table => new
                {
                    IdMonitorClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfesor = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoresClase", x => x.IdMonitorClase);
                    table.ForeignKey(
                        name: "FK_MonitoresClase_Clases_IdClase",
                        column: x => x.IdClase,
                        principalTable: "Clases",
                        principalColumn: "IdClase",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoresClase_Profesores_IdProfesor",
                        column: x => x.IdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "IdProfesor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    IdAsistencia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdClase = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPaqueteUsado = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.IdAsistencia);
                    table.ForeignKey(
                        name: "FK_Asistencias_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asistencias_Clases_IdClase",
                        column: x => x.IdClase,
                        principalTable: "Clases",
                        principalColumn: "IdClase",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asistencias_EstadosAsistencia_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "EstadosAsistencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_Paquetes_IdPaqueteUsado",
                        column: x => x.IdPaqueteUsado,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CongelacionesPaquete",
                columns: table => new
                {
                    IdCongelacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPaquete = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongelacionesPaquete", x => x.IdCongelacion);
                    table.ForeignKey(
                        name: "FK_CongelacionesPaquete_Paquetes_IdPaquete",
                        column: x => x.IdPaquete,
                        principalTable: "Paquetes",
                        principalColumn: "IdPaquete",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EstadosAsistencia",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Presente" },
                    { 2, "Ausente" },
                    { 3, "Justificada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosNotificacion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "Enviada" },
                    { 3, "Leida" }
                });

            migrationBuilder.InsertData(
                table: "EstadosPaquete",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Activo" },
                    { 2, "Vencido" },
                    { 3, "Congelado" },
                    { 4, "Agotado" }
                });

            migrationBuilder.InsertData(
                table: "EstadosUsuario",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Activo" },
                    { 2, "Inactivo" },
                    { 3, "Bloqueado" }
                });

            migrationBuilder.InsertData(
                table: "MetodosPago",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("10101010-1010-1010-1010-101010101010"), "Efectivo" },
                    { new Guid("20202020-2020-2020-2020-202020202020"), "Transferencia" },
                    { new Guid("30303030-3030-3030-3030-303030303030"), "Tarjeta" },
                    { new Guid("40404040-4040-4040-4040-404040404040"), "Bono" },
                    { new Guid("50505050-5050-5050-5050-505050505050"), "Cortesia" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Administrador" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Alumno" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Profesor" }
                });

            migrationBuilder.InsertData(
                table: "RolesEnClase",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("12121212-1212-1212-1212-121212121212"), "Monitor" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Principal" }
                });

            migrationBuilder.InsertData(
                table: "TiposClase",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Regular" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Taller" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Evento" }
                });

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "CC" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "CE" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "PAS" }
                });

            migrationBuilder.InsertData(
                table: "TiposPaquete",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Mensual" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "BonoClases" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "Ilimitado" }
                });

            migrationBuilder.InsertData(
                table: "TiposProfesor",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Principal" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Monitor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_IdUsuario",
                table: "Alumnos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdAlumno",
                table: "Asistencias",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdClase_IdAlumno",
                table: "Asistencias",
                columns: new[] { "IdClase", "IdAlumno" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdEstado",
                table: "Asistencias",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdPaqueteUsado",
                table: "Asistencias",
                column: "IdPaqueteUsado");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_FechaHora",
                table: "Auditorias",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IdUsuario",
                table: "Auditorias",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_Fecha_IdTipoClase",
                table: "Clases",
                columns: new[] { "Fecha", "IdTipoClase" });

            migrationBuilder.CreateIndex(
                name: "IX_Clases_IdProfesorPrincipal",
                table: "Clases",
                column: "IdProfesorPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_IdTipoClase",
                table: "Clases",
                column: "IdTipoClase");

            migrationBuilder.CreateIndex(
                name: "IX_CongelacionesPaquete_IdPaquete_FechaInicio_FechaFin",
                table: "CongelacionesPaquete",
                columns: new[] { "IdPaquete", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_EstadosAsistencia_Nombre",
                table: "EstadosAsistencia",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosNotificacion_Nombre",
                table: "EstadosNotificacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosPaquete_Nombre",
                table: "EstadosPaquete",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosUsuario_Nombre",
                table: "EstadosUsuario",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetodosPago_Nombre",
                table: "MetodosPago",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoresClase_IdClase_IdProfesor",
                table: "MonitoresClase",
                columns: new[] { "IdClase", "IdProfesor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoresClase_IdProfesor",
                table: "MonitoresClase",
                column: "IdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_IdEstado",
                table: "Notificaciones",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_IdUsuario_Leida",
                table: "Notificaciones",
                columns: new[] { "IdUsuario", "Leida" });

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdAlumno",
                table: "Pagos",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdMetodoPago",
                table: "Pagos",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_IdAlumno_FechaVencimiento",
                table: "Paquetes",
                columns: new[] { "IdAlumno", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_IdEstado",
                table: "Paquetes",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_IdPago",
                table: "Paquetes",
                column: "IdPago");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_IdTipoPaquete",
                table: "Paquetes",
                column: "IdTipoPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_IdTipoProfesor",
                table: "Profesores",
                column: "IdTipoProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_IdUsuario",
                table: "Profesores",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesEnClase_Nombre",
                table: "RolesEnClase",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarifasProfesor_IdRolEnClase",
                table: "TarifasProfesor",
                column: "IdRolEnClase");

            migrationBuilder.CreateIndex(
                name: "IX_TarifasProfesor_IdTipoProfesor_IdRolEnClase",
                table: "TarifasProfesor",
                columns: new[] { "IdTipoProfesor", "IdRolEnClase" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarifasProfesor_ProfesorIdProfesor",
                table: "TarifasProfesor",
                column: "ProfesorIdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_TiposClase_Nombre",
                table: "TiposClase",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumento_Nombre",
                table: "TiposDocumento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPaquete_Nombre",
                table: "TiposPaquete",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposProfesor_Nombre",
                table: "TiposProfesor",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEstadoUsuario",
                table: "Usuarios",
                column: "IdEstadoUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdTipoDocumento",
                table: "Usuarios",
                column: "IdTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NumeroDocumento",
                table: "Usuarios",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_IdRol",
                table: "UsuariosRoles",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "ConfiguracionesNotificaciones");

            migrationBuilder.DropTable(
                name: "CongelacionesPaquete");

            migrationBuilder.DropTable(
                name: "MonitoresClase");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "TarifasProfesor");

            migrationBuilder.DropTable(
                name: "UsuariosRoles");

            migrationBuilder.DropTable(
                name: "EstadosAsistencia");

            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "Clases");

            migrationBuilder.DropTable(
                name: "EstadosNotificacion");

            migrationBuilder.DropTable(
                name: "RolesEnClase");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "EstadosPaquete");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "TiposPaquete");

            migrationBuilder.DropTable(
                name: "Profesores");

            migrationBuilder.DropTable(
                name: "TiposClase");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "TiposProfesor");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "EstadosUsuario");

            migrationBuilder.DropTable(
                name: "TiposDocumento");
        }
    }
}
