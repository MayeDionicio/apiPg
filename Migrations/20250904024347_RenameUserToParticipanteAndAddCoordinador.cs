using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserToParticipanteAndAddCoordinador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizar el rol existente con Id = 2
            migrationBuilder.Sql("UPDATE \"Roles\" SET \"Name\" = 'Participante', \"Description\" = 'Acceso de participante / beneficiario' WHERE \"Id\" = 2;");

            // Insertar nuevo rol Coordinador con Id = 5 (si no existe)
            migrationBuilder.Sql(@"INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""CreatedAt"", ""IsActive"")
                SELECT 5, 'Coordinador', 'Coordinación y supervisión', NOW(), true
                WHERE NOT EXISTS (SELECT 1 FROM ""Roles"" WHERE ""Id"" = 5);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar el rol Coordinador si fue añadido
            migrationBuilder.Sql("DELETE FROM \"Roles\" WHERE \"Id\" = 5;");

            // Revertir el nombre del rol Id = 2 a 'User'
            migrationBuilder.Sql("UPDATE \"Roles\" SET \"Name\" = 'User', \"Description\" = 'Regular user access' WHERE \"Id\" = 2;");
        }
    }
}
