using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarLogrosParaFacilitadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogrosFacilitador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PuntosValor = table.Column<int>(type: "integer", nullable: false),
                    CriteriosObtencion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TipoLogro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Manual"),
                    CantidadActividadesRequeridas = table.Column<int>(type: "integer", nullable: true),
                    CantidadEstudiantesRequeridos = table.Column<int>(type: "integer", nullable: true),
                    DiasConsecutivosRequeridos = table.Column<int>(type: "integer", nullable: true),
                    CreadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogrosFacilitador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogrosFacilitador_Usuarios_CreadoPorIdUsuario",
                        column: x => x.CreadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogrosObtenidosFacilitador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLogro = table.Column<int>(type: "integer", nullable: false),
                    IdFacilitador = table.Column<int>(type: "integer", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OtorgadoPorIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdActividadRelacionada = table.Column<int>(type: "integer", nullable: true),
                    IdActividadMontessoriRelacionada = table.Column<int>(type: "integer", nullable: true),
                    IdActividadPersonalizadaRelacionada = table.Column<int>(type: "integer", nullable: true),
                    Justificacion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ComentarioCoordinador = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogrosObtenidosFacilitador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_ActividadesMontessoriPersonaliza~",
                        column: x => x.IdActividadPersonalizadaRelacionada,
                        principalTable: "ActividadesMontessoriPersonalizadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_ActividadesMontessori_IdActivida~",
                        column: x => x.IdActividadMontessoriRelacionada,
                        principalTable: "ActividadesMontessori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_Actividades_IdActividadRelaciona~",
                        column: x => x.IdActividadRelacionada,
                        principalTable: "Actividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_LogrosFacilitador_IdLogro",
                        column: x => x.IdLogro,
                        principalTable: "LogrosFacilitador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_Usuarios_IdFacilitador",
                        column: x => x.IdFacilitador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogrosObtenidosFacilitador_Usuarios_OtorgadoPorIdUsuario",
                        column: x => x.OtorgadoPorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogrosFacilitador_Categoria",
                table: "LogrosFacilitador",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosFacilitador_CreadoPorIdUsuario",
                table: "LogrosFacilitador",
                column: "CreadoPorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosFacilitador_EstaActivo",
                table: "LogrosFacilitador",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosFacilitador_TipoLogro",
                table: "LogrosFacilitador",
                column: "TipoLogro");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidos_Facilitador_Logro_Unique",
                table: "LogrosObtenidosFacilitador",
                columns: new[] { "IdFacilitador", "IdLogro" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_FechaObtencion",
                table: "LogrosObtenidosFacilitador",
                column: "FechaObtencion");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_IdActividadMontessoriRelacionada",
                table: "LogrosObtenidosFacilitador",
                column: "IdActividadMontessoriRelacionada");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_IdActividadPersonalizadaRelacion~",
                table: "LogrosObtenidosFacilitador",
                column: "IdActividadPersonalizadaRelacionada");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_IdActividadRelacionada",
                table: "LogrosObtenidosFacilitador",
                column: "IdActividadRelacionada");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_IdFacilitador",
                table: "LogrosObtenidosFacilitador",
                column: "IdFacilitador");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_IdLogro",
                table: "LogrosObtenidosFacilitador",
                column: "IdLogro");

            migrationBuilder.CreateIndex(
                name: "IX_LogrosObtenidosFacilitador_OtorgadoPorIdUsuario",
                table: "LogrosObtenidosFacilitador",
                column: "OtorgadoPorIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogrosObtenidosFacilitador");

            migrationBuilder.DropTable(
                name: "LogrosFacilitador");
        }
    }
}
