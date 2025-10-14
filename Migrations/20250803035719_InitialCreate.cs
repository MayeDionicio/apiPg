using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 4, 3, 57, 18, 882, DateTimeKind.Utc).AddTicks(8235), "Full system access", true, "Administrator" },
                    { 2, new DateTime(2025, 6, 4, 3, 57, 18, 882, DateTimeKind.Utc).AddTicks(8946), "Acceso de participante / beneficiario", true, "Participante" },
                    { 3, new DateTime(2025, 6, 4, 3, 57, 18, 882, DateTimeKind.Utc).AddTicks(8953), "Read-only access", true, "Viewer" },
                    { 4, new DateTime(2025, 7, 4, 3, 57, 18, 882, DateTimeKind.Utc).AddTicks(8955), "Management access", true, "Manager" },
                    { 5, new DateTime(2025, 7, 4, 3, 57, 18, 882, DateTimeKind.Utc).AddTicks(9000), "Coordinación y supervisión", true, "Coordinador" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "RoleId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 4, 3, 57, 18, 893, DateTimeKind.Utc).AddTicks(8309), "admin@apipg.com", "Admin", true, "System", "b/7s5TnSLbUEMwpw1dfZFqDzDuwr/6CKBhTF4K+K5KQ=", 1, null, "admin" },
                    { 2, new DateTime(2025, 7, 19, 3, 57, 18, 893, DateTimeKind.Utc).AddTicks(9118), "juan.perez@email.com", "Juan", true, "Pérez", "5koThLVJCEZ3mjd4R2huzmFy7+Otmfgvnes9PHPaN0U=", 2, null, "jperez" },
                    { 3, new DateTime(2025, 7, 24, 3, 57, 18, 893, DateTimeKind.Utc).AddTicks(9150), "maria.garcia@email.com", "María", true, "García", "5koThLVJCEZ3mjd4R2huzmFy7+Otmfgvnes9PHPaN0U=", 3, null, "mgarcia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
