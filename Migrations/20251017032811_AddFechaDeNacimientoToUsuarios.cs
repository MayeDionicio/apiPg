using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaDeNacimientoToUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDeNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaDeNacimiento",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaDeNacimiento",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaDeNacimiento",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaDeNacimiento",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaDeNacimiento",
                table: "Usuarios");
        }
    }
}
