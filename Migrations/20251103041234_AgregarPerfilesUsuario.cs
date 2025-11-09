using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPerfilesUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfilesUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    FotoPerfil = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Pais = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactoEmergenciaNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactoEmergenciaTelefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContactoEmergenciaRelacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NombrePadre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NombreMadre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NombreTutor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Alergias = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CondicionesMedicas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Medicamentos = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Biografia = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Intereses = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Facebook = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Twitter = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstaActivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilesUsuario_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesUsuario_Ciudad",
                table: "PerfilesUsuario",
                column: "Ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesUsuario_IdUsuario",
                table: "PerfilesUsuario",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesUsuario_Telefono",
                table: "PerfilesUsuario",
                column: "Telefono");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfilesUsuario");
        }
    }
}
