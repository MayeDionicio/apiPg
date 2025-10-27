using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaDeTareas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tareas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    NotasCoordinador = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    NotasVoluntario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RazonRechazo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tareas_Usuarios_CreadoPorIdUsuario",
                        column: x => x.CreadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesDeTarea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TareaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EstadoVoluntario = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AsignadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    VistaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IniciadaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletadaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesDeTarea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesDeTarea_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignacionesDeTarea_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeTarea_AsignadoEn",
                table: "AsignacionesDeTarea",
                column: "AsignadoEn");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeTarea_EstadoVoluntario",
                table: "AsignacionesDeTarea",
                column: "EstadoVoluntario");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeTarea_TareaId",
                table: "AsignacionesDeTarea",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeTarea_TareaId_UsuarioId",
                table: "AsignacionesDeTarea",
                columns: new[] { "TareaId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDeTarea_UsuarioId",
                table: "AsignacionesDeTarea",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_CreadoEn",
                table: "Tareas",
                column: "CreadoEn");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_CreadoPorIdUsuario",
                table: "Tareas",
                column: "CreadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_EstaActivo",
                table: "Tareas",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_Estado",
                table: "Tareas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_FechaFin",
                table: "Tareas",
                column: "FechaFin");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_FechaInicio",
                table: "Tareas",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_Titulo",
                table: "Tareas",
                column: "Titulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesDeTarea");

            migrationBuilder.DropTable(
                name: "Tareas");
        }
    }
}
