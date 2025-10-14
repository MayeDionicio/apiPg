using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class CreateDevocionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devocionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "date", nullable: true),
                    VoluntarioAsignadoId = table.Column<int>(type: "integer", nullable: true),
                    VoluntarioAsignado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Pasaje = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TextoClave = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Objetivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Idea = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PuntosPrincipales = table.Column<string>(type: "text", nullable: true),
                    Aplicacion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Reto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Oracion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Recursos = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devocionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devocionales_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devocionales_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devocionales_Users_VoluntarioAsignadoId",
                        column: x => x.VoluntarioAsignadoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "RoleId", "UpdatedAt", "Username" },
                values: new object[] { 4, new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "coordinador@apipg.com", "Carlos", true, "Coordinador", "QSNk6NjkEQY5MPIFVC9cEVVPerlnGk2oA7V5puh9dew=", 5, null, "coordinador" });

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_CreatedAt",
                table: "Devocionales",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_CreatedByUserId",
                table: "Devocionales",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_Estado",
                table: "Devocionales",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_FechaProgramada",
                table: "Devocionales",
                column: "FechaProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_IsActive",
                table: "Devocionales",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_Titulo",
                table: "Devocionales",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_UpdatedByUserId",
                table: "Devocionales",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Devocionales_VoluntarioAsignadoId",
                table: "Devocionales",
                column: "VoluntarioAsignadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devocionales");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
