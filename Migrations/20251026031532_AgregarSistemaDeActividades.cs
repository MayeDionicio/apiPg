using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaDeActividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AreaDeEnfoque = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DescripcionDetallada = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DuracionEstimadaMinutos = table.Column<int>(type: "integer", nullable: false),
                    MaterialesNecesarios = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaDelEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actividades_Usuarios_CreadoPorIdUsuario",
                        column: x => x.CreadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesDeActividad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActividadId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    AsignadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesDeActividad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesDeActividad_Actividades_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "Actividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignacionesDeActividad_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_AreaDeEnfoque",
                table: "Actividades",
                column: "AreaDeEnfoque");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_CreadoEn",
                table: "Actividades",
                column: "CreadoEn");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_CreadoPorIdUsuario",
                table: "Actividades",
                column: "CreadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_EstaActivo",
                table: "Actividades",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_FechaDelEvento",
                table: "Actividades",
                column: "FechaDelEvento");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_Titulo",
                table: "Actividades",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeActividad_ActividadId",
                table: "AsignacionesDeActividad",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeActividad_ActividadId_UsuarioId",
                table: "AsignacionesDeActividad",
                columns: new[] { "ActividadId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeActividad_AsignadoEn",
                table: "AsignacionesDeActividad",
                column: "AsignadoEn");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeActividad_UsuarioId",
                table: "AsignacionesDeActividad",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesDeActividad");

            migrationBuilder.DropTable(
                name: "Actividades");
        }
    }
}
