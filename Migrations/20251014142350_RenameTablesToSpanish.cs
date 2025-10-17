using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToSpanish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Levels_IdNivel",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_IdUsuario",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Users_ActualizadoPorIdUsuario",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Users_CreadoPorIdUsuario",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Users_VoluntarioAsignadoId",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelParticipants_Levels_IdNivel",
                table: "LevelParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelParticipants_Users_IdUsuario",
                table: "LevelParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Users_IdVoluntario",
                table: "Levels");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Resources_IdRecurso",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Users_AsignadoPorIdUsuario",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Users_IdVoluntario",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_ResourceAssignments_IdAsignacionDeRecurso",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_Users_ReportadoPorIdUsuario",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_Users_ResueltoPorIdUsuario",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_IdRol",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceUsageLogs",
                table: "ResourceUsageLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resources",
                table: "Resources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceAssignments",
                table: "ResourceAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Levels",
                table: "Levels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LevelParticipants",
                table: "LevelParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "ResourceUsageLogs",
                newName: "RegistrosDeUsoDeRecurso");

            migrationBuilder.RenameTable(
                name: "Resources",
                newName: "Recursos");

            migrationBuilder.RenameTable(
                name: "ResourceAssignments",
                newName: "AsignacionesDeRecurso");

            migrationBuilder.RenameTable(
                name: "Levels",
                newName: "Niveles");

            migrationBuilder.RenameTable(
                name: "LevelParticipants",
                newName: "NivelesDeParticipantes");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "Asistencias");

            migrationBuilder.RenameIndex(
                name: "IX_Users_NombreDeUsuario",
                table: "Usuarios",
                newName: "IX_Usuarios_NombreDeUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Users_IdRol",
                table: "Usuarios",
                newName: "IX_Usuarios_IdRol");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CorreoElectronico",
                table: "Usuarios",
                newName: "IX_Usuarios_CorreoElectronico");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_TipoDeEvento",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_TipoDeEvento");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ResueltoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_ResueltoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ReportadoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_ReportadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_IdAsignacionDeRecurso",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_IdAsignacionDeRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_EstaResuelto",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_EstaResuelto");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_CreadoEn",
                table: "RegistrosDeUsoDeRecurso",
                newName: "IX_RegistrosDeUsoDeRecurso_CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Nombre",
                table: "Recursos",
                newName: "IX_Recursos_Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_EstaActivo",
                table: "Recursos",
                newName: "IX_Recursos_EstaActivo");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Categoria",
                table: "Recursos",
                newName: "IX_Recursos_Categoria");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_IdVoluntario",
                table: "AsignacionesDeRecurso",
                newName: "IX_AsignacionesDeRecurso_IdVoluntario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_IdRecurso",
                table: "AsignacionesDeRecurso",
                newName: "IX_AsignacionesDeRecurso_IdRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_Estado",
                table: "AsignacionesDeRecurso",
                newName: "IX_AsignacionesDeRecurso_Estado");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AsignadoPorIdUsuario",
                table: "AsignacionesDeRecurso",
                newName: "IX_AsignacionesDeRecurso_AsignadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AsignadoEn",
                table: "AsignacionesDeRecurso",
                newName: "IX_AsignacionesDeRecurso_AsignadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Levels_IdVoluntario",
                table: "Niveles",
                newName: "IX_Niveles_IdVoluntario");

            migrationBuilder.RenameIndex(
                name: "IX_LevelParticipants_IdUsuario",
                table: "NivelesDeParticipantes",
                newName: "IX_NivelesDeParticipantes_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_IdUsuario",
                table: "Asistencias",
                newName: "IX_Asistencias_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_IdNivel",
                table: "Asistencias",
                newName: "IX_Asistencias_IdNivel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrosDeUsoDeRecurso",
                table: "RegistrosDeUsoDeRecurso",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recursos",
                table: "Recursos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AsignacionesDeRecurso",
                table: "AsignacionesDeRecurso",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Niveles",
                table: "Niveles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NivelesDeParticipantes",
                table: "NivelesDeParticipantes",
                columns: new[] { "IdNivel", "IdUsuario" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asistencias",
                table: "Asistencias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AsignacionesDeRecurso_Recursos_IdRecurso",
                table: "AsignacionesDeRecurso",
                column: "IdRecurso",
                principalTable: "Recursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AsignacionesDeRecurso_Usuarios_AsignadoPorIdUsuario",
                table: "AsignacionesDeRecurso",
                column: "AsignadoPorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AsignacionesDeRecurso_Usuarios_IdVoluntario",
                table: "AsignacionesDeRecurso",
                column: "IdVoluntario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Niveles_IdNivel",
                table: "Asistencias",
                column: "IdNivel",
                principalTable: "Niveles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_IdUsuario",
                table: "Asistencias",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Usuarios_ActualizadoPorIdUsuario",
                table: "Devocionales",
                column: "ActualizadoPorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Usuarios_CreadoPorIdUsuario",
                table: "Devocionales",
                column: "CreadoPorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Usuarios_VoluntarioAsignadoId",
                table: "Devocionales",
                column: "VoluntarioAsignadoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Niveles_Usuarios_IdVoluntario",
                table: "Niveles",
                column: "IdVoluntario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NivelesDeParticipantes_Niveles_IdNivel",
                table: "NivelesDeParticipantes",
                column: "IdNivel",
                principalTable: "Niveles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NivelesDeParticipantes_Usuarios_IdUsuario",
                table: "NivelesDeParticipantes",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_AsignacionesDeRecurso_IdAsignacionD~",
                table: "RegistrosDeUsoDeRecurso",
                column: "IdAsignacionDeRecurso",
                principalTable: "AsignacionesDeRecurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_Usuarios_ReportadoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso",
                column: "ReportadoPorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_Usuarios_ResueltoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso",
                column: "ResueltoPorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AsignacionesDeRecurso_Recursos_IdRecurso",
                table: "AsignacionesDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_AsignacionesDeRecurso_Usuarios_AsignadoPorIdUsuario",
                table: "AsignacionesDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_AsignacionesDeRecurso_Usuarios_IdVoluntario",
                table: "AsignacionesDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Niveles_IdNivel",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_IdUsuario",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Usuarios_ActualizadoPorIdUsuario",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Usuarios_CreadoPorIdUsuario",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Usuarios_VoluntarioAsignadoId",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Niveles_Usuarios_IdVoluntario",
                table: "Niveles");

            migrationBuilder.DropForeignKey(
                name: "FK_NivelesDeParticipantes_Niveles_IdNivel",
                table: "NivelesDeParticipantes");

            migrationBuilder.DropForeignKey(
                name: "FK_NivelesDeParticipantes_Usuarios_IdUsuario",
                table: "NivelesDeParticipantes");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_AsignacionesDeRecurso_IdAsignacionD~",
                table: "RegistrosDeUsoDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_Usuarios_ReportadoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosDeUsoDeRecurso_Usuarios_ResueltoPorIdUsuario",
                table: "RegistrosDeUsoDeRecurso");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrosDeUsoDeRecurso",
                table: "RegistrosDeUsoDeRecurso");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recursos",
                table: "Recursos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NivelesDeParticipantes",
                table: "NivelesDeParticipantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Niveles",
                table: "Niveles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asistencias",
                table: "Asistencias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AsignacionesDeRecurso",
                table: "AsignacionesDeRecurso");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "RegistrosDeUsoDeRecurso",
                newName: "ResourceUsageLogs");

            migrationBuilder.RenameTable(
                name: "Recursos",
                newName: "Resources");

            migrationBuilder.RenameTable(
                name: "NivelesDeParticipantes",
                newName: "LevelParticipants");

            migrationBuilder.RenameTable(
                name: "Niveles",
                newName: "Levels");

            migrationBuilder.RenameTable(
                name: "Asistencias",
                newName: "Attendances");

            migrationBuilder.RenameTable(
                name: "AsignacionesDeRecurso",
                newName: "ResourceAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_NombreDeUsuario",
                table: "Users",
                newName: "IX_Users_NombreDeUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdRol",
                table: "Users",
                newName: "IX_Users_IdRol");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_CorreoElectronico",
                table: "Users",
                newName: "IX_Users_CorreoElectronico");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_TipoDeEvento",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_TipoDeEvento");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_ResueltoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ResueltoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_ReportadoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ReportadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_IdAsignacionDeRecurso",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_IdAsignacionDeRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_EstaResuelto",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_EstaResuelto");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosDeUsoDeRecurso_CreadoEn",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Recursos_Nombre",
                table: "Resources",
                newName: "IX_Resources_Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Recursos_EstaActivo",
                table: "Resources",
                newName: "IX_Resources_EstaActivo");

            migrationBuilder.RenameIndex(
                name: "IX_Recursos_Categoria",
                table: "Resources",
                newName: "IX_Resources_Categoria");

            migrationBuilder.RenameIndex(
                name: "IX_NivelesDeParticipantes_IdUsuario",
                table: "LevelParticipants",
                newName: "IX_LevelParticipants_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Niveles_IdVoluntario",
                table: "Levels",
                newName: "IX_Levels_IdVoluntario");

            migrationBuilder.RenameIndex(
                name: "IX_Asistencias_IdUsuario",
                table: "Attendances",
                newName: "IX_Attendances_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Asistencias_IdNivel",
                table: "Attendances",
                newName: "IX_Attendances_IdNivel");

            migrationBuilder.RenameIndex(
                name: "IX_AsignacionesDeRecurso_IdVoluntario",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_IdVoluntario");

            migrationBuilder.RenameIndex(
                name: "IX_AsignacionesDeRecurso_IdRecurso",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_IdRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_AsignacionesDeRecurso_Estado",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_Estado");

            migrationBuilder.RenameIndex(
                name: "IX_AsignacionesDeRecurso_AsignadoPorIdUsuario",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AsignadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_AsignacionesDeRecurso_AsignadoEn",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AsignadoEn");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceUsageLogs",
                table: "ResourceUsageLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resources",
                table: "Resources",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LevelParticipants",
                table: "LevelParticipants",
                columns: new[] { "IdNivel", "IdUsuario" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Levels",
                table: "Levels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceAssignments",
                table: "ResourceAssignments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Levels_IdNivel",
                table: "Attendances",
                column: "IdNivel",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_IdUsuario",
                table: "Attendances",
                column: "IdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Users_ActualizadoPorIdUsuario",
                table: "Devocionales",
                column: "ActualizadoPorIdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Users_CreadoPorIdUsuario",
                table: "Devocionales",
                column: "CreadoPorIdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Users_VoluntarioAsignadoId",
                table: "Devocionales",
                column: "VoluntarioAsignadoId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelParticipants_Levels_IdNivel",
                table: "LevelParticipants",
                column: "IdNivel",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelParticipants_Users_IdUsuario",
                table: "LevelParticipants",
                column: "IdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Users_IdVoluntario",
                table: "Levels",
                column: "IdVoluntario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Resources_IdRecurso",
                table: "ResourceAssignments",
                column: "IdRecurso",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Users_AsignadoPorIdUsuario",
                table: "ResourceAssignments",
                column: "AsignadoPorIdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Users_IdVoluntario",
                table: "ResourceAssignments",
                column: "IdVoluntario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_ResourceAssignments_IdAsignacionDeRecurso",
                table: "ResourceUsageLogs",
                column: "IdAsignacionDeRecurso",
                principalTable: "ResourceAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_Users_ReportadoPorIdUsuario",
                table: "ResourceUsageLogs",
                column: "ReportadoPorIdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_Users_ResueltoPorIdUsuario",
                table: "ResourceUsageLogs",
                column: "ResueltoPorIdUsuario",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_IdRol",
                table: "Users",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
