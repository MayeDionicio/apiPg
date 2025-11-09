using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRangoDeEdadANiveles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EdadMaxima",
                table: "Niveles",
                type: "numeric(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EdadMinima",
                table: "Niveles",
                type: "numeric(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ActividadesMontessori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    AreaPedagogica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaActividad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraActividad = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EdadMinima = table.Column<decimal>(type: "numeric(3,1)", nullable: false),
                    EdadMaxima = table.Column<decimal>(type: "numeric(3,1)", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: false),
                    ObjetivoEspecifico = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MaterialesNecesarios = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    MontajeAmbiente = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Prerequisitos = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PresentacionPasoAPaso = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    ControlDelError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AspectosAutonomia = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LimitesYNormas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IndicadoresDeLogro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AdaptacionesVariaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NivelDificultad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EvidenciaUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ObservacionesAdicionales = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    ChecklistMontessori = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesMontessori", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesMontessori_Usuarios_CreadoPorIdUsuario",
                        column: x => x.CreadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogrosMontessori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsObtenido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaObtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActividadMontessoriId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogrosMontessori", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogrosMontessori_ActividadesMontessori_ActividadMontessoriId",
                        column: x => x.ActividadMontessoriId,
                        principalTable: "ActividadesMontessori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogrosMontessori_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Niveles_EdadMaxima",
                table: "Niveles",
                column: "EdadMaxima");

            migrationBuilder.CreateIndex(
                name: "IX_Niveles_EdadMinima",
                table: "Niveles",
                column: "EdadMinima");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessori_AreaPedagogica",
                table: "ActividadesMontessori",
                column: "AreaPedagogica");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessori_CreadoPorIdUsuario",
                table: "ActividadesMontessori",
                column: "CreadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessori_EstaActivo",
                table: "ActividadesMontessori",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessori_FechaActividad",
                table: "ActividadesMontessori",
                column: "FechaActividad");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessori_Nombre",
                table: "ActividadesMontessori",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosMontessori_ActividadMontessoriId",
                table: "LogrosMontessori",
                column: "ActividadMontessoriId");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosMontessori_EsObtenido",
                table: "LogrosMontessori",
                column: "EsObtenido");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosMontessori_FechaObtencion",
                table: "LogrosMontessori",
                column: "FechaObtencion");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosMontessori_UsuarioId",
                table: "LogrosMontessori",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogrosMontessori");

            migrationBuilder.DropTable(
                name: "ActividadesMontessori");

            migrationBuilder.DropIndex(
                name: "IX_Niveles_EdadMaxima",
                table: "Niveles");

            migrationBuilder.DropIndex(
                name: "IX_Niveles_EdadMinima",
                table: "Niveles");

            migrationBuilder.DropColumn(
                name: "EdadMaxima",
                table: "Niveles");

            migrationBuilder.DropColumn(
                name: "EdadMinima",
                table: "Niveles");
        }
    }
}
