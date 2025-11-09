using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarActividadesMontessoriPersonalizadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActividadesMontessoriPersonalizadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdActividadMontessoriBase = table.Column<int>(type: "integer", nullable: true),
                    IdEstudiante = table.Column<int>(type: "integer", nullable: false),
                    IdNivel = table.Column<int>(type: "integer", nullable: true),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AreaPedagogica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCompletada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HoraActividad = table.Column<TimeSpan>(type: "interval", nullable: true),
                    DuracionMinutosEstimada = table.Column<int>(type: "integer", nullable: false),
                    DuracionMinutosReal = table.Column<int>(type: "integer", nullable: true),
                    ObjetivoPersonalizado = table.Column<string>(type: "text", nullable: false),
                    MaterialesNecesarios = table.Column<string>(type: "text", nullable: false),
                    PresentacionPersonalizada = table.Column<string>(type: "text", nullable: true),
                    AdaptacionesEspeciales = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    ProgresosPorcentaje = table.Column<int>(type: "integer", nullable: true),
                    ObservacionesProfesor = table.Column<string>(type: "text", nullable: true),
                    LogrosAlcanzados = table.Column<string>(type: "text", nullable: true),
                    DificultadesEncontradas = table.Column<string>(type: "text", nullable: true),
                    SugerenciasParaSeguimiento = table.Column<string>(type: "text", nullable: true),
                    NivelDesempeno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RequiereRefuerzo = table.Column<bool>(type: "boolean", nullable: false),
                    EvidenciaUrlFoto = table.Column<string>(type: "text", nullable: true),
                    EvidenciaUrlVideo = table.Column<string>(type: "text", nullable: true),
                    NotasAdicionales = table.Column<string>(type: "text", nullable: true),
                    EnviarRecordatorio = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRecordatorio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecordatorioEnviado = table.Column<bool>(type: "boolean", nullable: false),
                    Prioridad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Normal"),
                    AsignadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EsRecurrente = table.Column<bool>(type: "boolean", nullable: false),
                    FrecuenciaRecurrencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FechaFinRecurrencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesMontessoriPersonalizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesMontessoriPersonalizadas_ActividadesMontessori_I~",
                        column: x => x.IdActividadMontessoriBase,
                        principalTable: "ActividadesMontessori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ActividadesMontessoriPersonalizadas_Niveles_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "Niveles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ActividadesMontessoriPersonalizadas_Usuarios_AsignadoPorIdU~",
                        column: x => x.AsignadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActividadesMontessoriPersonalizadas_Usuarios_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_AsignadoPorIdUsuario",
                table: "ActividadesMontessoriPersonalizadas",
                column: "AsignadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_Estado",
                table: "ActividadesMontessoriPersonalizadas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_FechaAsignacion",
                table: "ActividadesMontessoriPersonalizadas",
                column: "FechaAsignacion");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_FechaCompletada",
                table: "ActividadesMontessoriPersonalizadas",
                column: "FechaCompletada");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_IdActividadMontessoriBa~",
                table: "ActividadesMontessoriPersonalizadas",
                column: "IdActividadMontessoriBase");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_IdEstudiante",
                table: "ActividadesMontessoriPersonalizadas",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_IdEstudiante_Estado",
                table: "ActividadesMontessoriPersonalizadas",
                columns: new[] { "IdEstudiante", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_IdNivel",
                table: "ActividadesMontessoriPersonalizadas",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesMontessoriPersonalizadas_Prioridad",
                table: "ActividadesMontessoriPersonalizadas",
                column: "Prioridad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadesMontessoriPersonalizadas");
        }
    }
}
