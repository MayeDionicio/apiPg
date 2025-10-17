using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Levels_LevelId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_UserId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Users_CreatedByUserId",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_Devocionales_Users_UpdatedByUserId",
                table: "Devocionales");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelParticipants_Levels_LevelId",
                table: "LevelParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelParticipants_Users_UserId",
                table: "LevelParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Users_TutorId",
                table: "Levels");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Resources_ResourceId",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Users_AssignedByUserId",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAssignments_Users_VolunteerId",
                table: "ResourceAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_ResourceAssignments_ResourceAssignmentId",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_Users_ReportedByUserId",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceUsageLogs_Users_ResolvedByUserId",
                table: "ResourceUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "NombreDeUsuario");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "ActualizadoEn");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Users",
                newName: "IdRol");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "HashDeContrasena");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "PrimerNombre");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "CorreoElectronico");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "Users",
                newName: "IX_Users_NombreDeUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                newName: "IX_Users_IdRol");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_CorreoElectronico");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Roles",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Roles",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Roles",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Roles",
                newName: "CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                newName: "IX_Roles_Nombre");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ResourceUsageLogs",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "ResourceAssignmentId",
                table: "ResourceUsageLogs",
                newName: "TipoDeEvento");

            migrationBuilder.RenameColumn(
                name: "ResolvedByUserId",
                table: "ResourceUsageLogs",
                newName: "ResueltoPorIdUsuario");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "ResourceUsageLogs",
                newName: "ResueltoEn");

            migrationBuilder.RenameColumn(
                name: "ResolutionNotes",
                table: "ResourceUsageLogs",
                newName: "Recomendaciones");

            migrationBuilder.RenameColumn(
                name: "RequiresFollowUp",
                table: "ResourceUsageLogs",
                newName: "RequiereSeguimiento");

            migrationBuilder.RenameColumn(
                name: "ReportedByUserId",
                table: "ResourceUsageLogs",
                newName: "ReportadoPorIdUsuario");

            migrationBuilder.RenameColumn(
                name: "Recommendations",
                table: "ResourceUsageLogs",
                newName: "NotasDeResolucion");

            migrationBuilder.RenameColumn(
                name: "QuantityAffected",
                table: "ResourceUsageLogs",
                newName: "CondicionDespues");

            migrationBuilder.RenameColumn(
                name: "PhotoUrls",
                table: "ResourceUsageLogs",
                newName: "UrlsDeFotos");

            migrationBuilder.RenameColumn(
                name: "IsResolved",
                table: "ResourceUsageLogs",
                newName: "EstaResuelto");

            migrationBuilder.RenameColumn(
                name: "EventType",
                table: "ResourceUsageLogs",
                newName: "IdAsignacionDeRecurso");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ResourceUsageLogs",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ResourceUsageLogs",
                newName: "CreadoEn");

            migrationBuilder.RenameColumn(
                name: "ConditionBefore",
                table: "ResourceUsageLogs",
                newName: "CondicionAntes");

            migrationBuilder.RenameColumn(
                name: "ConditionAfter",
                table: "ResourceUsageLogs",
                newName: "CantidadAfectada");

            migrationBuilder.RenameColumn(
                name: "ActionsTaken",
                table: "ResourceUsageLogs",
                newName: "AccionesTomadas");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ResourceAssignmentId",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_TipoDeEvento");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ResolvedByUserId",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ResueltoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ReportedByUserId",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ReportadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_IsResolved",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_EstaResuelto");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_EventType",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_IdAsignacionDeRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_CreatedAt",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_CreadoEn");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Resources",
                newName: "ActualizadoEn");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "Resources",
                newName: "Unidad");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Resources",
                newName: "CantidadDisponible");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Resources",
                newName: "Notas");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Resources",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Resources",
                newName: "Ubicacion");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Resources",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "EstimatedValue",
                table: "Resources",
                newName: "ValorEstimado");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Resources",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Resources",
                newName: "CreadoEn");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Resources",
                newName: "Categoria");

            migrationBuilder.RenameColumn(
                name: "AvailableQuantity",
                table: "Resources",
                newName: "Cantidad");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Name",
                table: "Resources",
                newName: "IX_Resources_Nombre");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_IsActive",
                table: "Resources",
                newName: "IX_Resources_EstaActivo");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Category",
                table: "Resources",
                newName: "IX_Resources_Categoria");

            migrationBuilder.RenameColumn(
                name: "VolunteerNotes",
                table: "ResourceAssignments",
                newName: "NotasIniciales");

            migrationBuilder.RenameColumn(
                name: "VolunteerId",
                table: "ResourceAssignments",
                newName: "IdVoluntario");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ResourceAssignments",
                newName: "IdRecurso");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "ResourceAssignments",
                newName: "IniciadoEn");

            migrationBuilder.RenameColumn(
                name: "ReturnedAt",
                table: "ResourceAssignments",
                newName: "FechaEsperadaDeDevolucion");

            migrationBuilder.RenameColumn(
                name: "ResourceId",
                table: "ResourceAssignments",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "QuantityAssigned",
                table: "ResourceAssignments",
                newName: "CantidadAsignada");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ResourceAssignments",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "InitialNotes",
                table: "ResourceAssignments",
                newName: "NotasDelVoluntario");

            migrationBuilder.RenameColumn(
                name: "ExpectedReturnDate",
                table: "ResourceAssignments",
                newName: "DevueltoEn");

            migrationBuilder.RenameColumn(
                name: "ConfirmedAt",
                table: "ResourceAssignments",
                newName: "ConfirmadoEn");

            migrationBuilder.RenameColumn(
                name: "AssignedByUserId",
                table: "ResourceAssignments",
                newName: "AsignadoPorIdUsuario");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "ResourceAssignments",
                newName: "AsignadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_VolunteerId",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_IdVoluntario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_Status",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_IdRecurso");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_ResourceId",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_Estado");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AssignedByUserId",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AsignadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AssignedAt",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AsignadoEn");

            migrationBuilder.RenameColumn(
                name: "TutorId",
                table: "Levels",
                newName: "IdVoluntario");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Levels",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Levels",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Levels",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Levels",
                newName: "CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Levels_TutorId",
                table: "Levels",
                newName: "IX_Levels_IdVoluntario");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "LevelParticipants",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "LevelParticipants",
                newName: "AsignadoEn");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "LevelParticipants",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "LevelParticipants",
                newName: "IdNivel");

            migrationBuilder.RenameIndex(
                name: "IX_LevelParticipants_UserId",
                table: "LevelParticipants",
                newName: "IX_LevelParticipants_IdUsuario");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Devocionales",
                newName: "ActualizadoPorIdUsuario");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Devocionales",
                newName: "ActualizadoEn");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Devocionales",
                newName: "EstaActivo");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Devocionales",
                newName: "CreadoPorIdUsuario");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Devocionales",
                newName: "CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_UpdatedByUserId",
                table: "Devocionales",
                newName: "IX_Devocionales_ActualizadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_IsActive",
                table: "Devocionales",
                newName: "IX_Devocionales_EstaActivo");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_CreatedByUserId",
                table: "Devocionales",
                newName: "IX_Devocionales_CreadoPorIdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_CreatedAt",
                table: "Devocionales",
                newName: "IX_Devocionales_CreadoEn");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Attendances",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Attendances",
                newName: "ActualizadoEn");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "Attendances",
                newName: "Observaciones");

            migrationBuilder.RenameColumn(
                name: "Present",
                table: "Attendances",
                newName: "Presente");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "Attendances",
                newName: "IdNivel");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Attendances",
                newName: "EstaEliminado");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Attendances",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Attendances",
                newName: "CreadoEn");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                newName: "IX_Attendances_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_LevelId",
                table: "Attendances",
                newName: "IX_Attendances_IdNivel");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Apellido", "PrimerNombre" },
                values: new object[] { "System", "Admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Apellido", "PrimerNombre" },
                values: new object[] { "Pérez", "Juan" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Apellido", "PrimerNombre" },
                values: new object[] { "García", "María" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Apellido", "PrimerNombre" },
                values: new object[] { "Coordinador", "Carlos" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "PrimerNombre",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "NombreDeUsuario",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "Users",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "HashDeContrasena",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CorreoElectronico",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "ActualizadoEn",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Users_NombreDeUsuario",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.RenameIndex(
                name: "IX_Users_IdRol",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CorreoElectronico",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "Roles",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Roles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Roles",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                newName: "IX_Roles_Name");

            migrationBuilder.RenameColumn(
                name: "UrlsDeFotos",
                table: "ResourceUsageLogs",
                newName: "PhotoUrls");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "ResourceUsageLogs",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TipoDeEvento",
                table: "ResourceUsageLogs",
                newName: "ResourceAssignmentId");

            migrationBuilder.RenameColumn(
                name: "ResueltoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "ResolvedByUserId");

            migrationBuilder.RenameColumn(
                name: "ResueltoEn",
                table: "ResourceUsageLogs",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "RequiereSeguimiento",
                table: "ResourceUsageLogs",
                newName: "RequiresFollowUp");

            migrationBuilder.RenameColumn(
                name: "ReportadoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "ReportedByUserId");

            migrationBuilder.RenameColumn(
                name: "Recomendaciones",
                table: "ResourceUsageLogs",
                newName: "ResolutionNotes");

            migrationBuilder.RenameColumn(
                name: "NotasDeResolucion",
                table: "ResourceUsageLogs",
                newName: "Recommendations");

            migrationBuilder.RenameColumn(
                name: "IdAsignacionDeRecurso",
                table: "ResourceUsageLogs",
                newName: "EventType");

            migrationBuilder.RenameColumn(
                name: "EstaResuelto",
                table: "ResourceUsageLogs",
                newName: "IsResolved");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "ResourceUsageLogs",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "ResourceUsageLogs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CondicionDespues",
                table: "ResourceUsageLogs",
                newName: "QuantityAffected");

            migrationBuilder.RenameColumn(
                name: "CondicionAntes",
                table: "ResourceUsageLogs",
                newName: "ConditionBefore");

            migrationBuilder.RenameColumn(
                name: "CantidadAfectada",
                table: "ResourceUsageLogs",
                newName: "ConditionAfter");

            migrationBuilder.RenameColumn(
                name: "AccionesTomadas",
                table: "ResourceUsageLogs",
                newName: "ActionsTaken");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_TipoDeEvento",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ResourceAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ResueltoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ResolvedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_ReportadoPorIdUsuario",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_ReportedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_IdAsignacionDeRecurso",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_EventType");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_EstaResuelto",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_IsResolved");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceUsageLogs_CreadoEn",
                table: "ResourceUsageLogs",
                newName: "IX_ResourceUsageLogs_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ValorEstimado",
                table: "Resources",
                newName: "EstimatedValue");

            migrationBuilder.RenameColumn(
                name: "Unidad",
                table: "Resources",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "Ubicacion",
                table: "Resources",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "Notas",
                table: "Resources",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Resources",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "Resources",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Resources",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Resources",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Categoria",
                table: "Resources",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "CantidadDisponible",
                table: "Resources",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Cantidad",
                table: "Resources",
                newName: "AvailableQuantity");

            migrationBuilder.RenameColumn(
                name: "ActualizadoEn",
                table: "Resources",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Nombre",
                table: "Resources",
                newName: "IX_Resources_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_EstaActivo",
                table: "Resources",
                newName: "IX_Resources_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_Categoria",
                table: "Resources",
                newName: "IX_Resources_Category");

            migrationBuilder.RenameColumn(
                name: "NotasIniciales",
                table: "ResourceAssignments",
                newName: "VolunteerNotes");

            migrationBuilder.RenameColumn(
                name: "NotasDelVoluntario",
                table: "ResourceAssignments",
                newName: "InitialNotes");

            migrationBuilder.RenameColumn(
                name: "IniciadoEn",
                table: "ResourceAssignments",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "IdVoluntario",
                table: "ResourceAssignments",
                newName: "VolunteerId");

            migrationBuilder.RenameColumn(
                name: "IdRecurso",
                table: "ResourceAssignments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "FechaEsperadaDeDevolucion",
                table: "ResourceAssignments",
                newName: "ReturnedAt");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "ResourceAssignments",
                newName: "ResourceId");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "ResourceAssignments",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "DevueltoEn",
                table: "ResourceAssignments",
                newName: "ExpectedReturnDate");

            migrationBuilder.RenameColumn(
                name: "ConfirmadoEn",
                table: "ResourceAssignments",
                newName: "ConfirmedAt");

            migrationBuilder.RenameColumn(
                name: "CantidadAsignada",
                table: "ResourceAssignments",
                newName: "QuantityAssigned");

            migrationBuilder.RenameColumn(
                name: "AsignadoPorIdUsuario",
                table: "ResourceAssignments",
                newName: "AssignedByUserId");

            migrationBuilder.RenameColumn(
                name: "AsignadoEn",
                table: "ResourceAssignments",
                newName: "AssignedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_IdVoluntario",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_VolunteerId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_IdRecurso",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_Status");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_Estado",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AsignadoPorIdUsuario",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AssignedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceAssignments_AsignadoEn",
                table: "ResourceAssignments",
                newName: "IX_ResourceAssignments_AssignedAt");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Levels",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "IdVoluntario",
                table: "Levels",
                newName: "TutorId");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "Levels",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Levels",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Levels",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Levels_IdVoluntario",
                table: "Levels",
                newName: "IX_Levels_TutorId");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "LevelParticipants",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "AsignadoEn",
                table: "LevelParticipants",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "LevelParticipants",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "IdNivel",
                table: "LevelParticipants",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_LevelParticipants_IdUsuario",
                table: "LevelParticipants",
                newName: "IX_LevelParticipants_UserId");

            migrationBuilder.RenameColumn(
                name: "EstaActivo",
                table: "Devocionales",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "CreadoPorIdUsuario",
                table: "Devocionales",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Devocionales",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ActualizadoPorIdUsuario",
                table: "Devocionales",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ActualizadoEn",
                table: "Devocionales",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_EstaActivo",
                table: "Devocionales",
                newName: "IX_Devocionales_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_CreadoPorIdUsuario",
                table: "Devocionales",
                newName: "IX_Devocionales_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_CreadoEn",
                table: "Devocionales",
                newName: "IX_Devocionales_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Devocionales_ActualizadoPorIdUsuario",
                table: "Devocionales",
                newName: "IX_Devocionales_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Presente",
                table: "Attendances",
                newName: "Present");

            migrationBuilder.RenameColumn(
                name: "Observaciones",
                table: "Attendances",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Attendances",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "IdNivel",
                table: "Attendances",
                newName: "LevelId");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Attendances",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "EstaEliminado",
                table: "Attendances",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreadoEn",
                table: "Attendances",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ActualizadoEn",
                table: "Attendances",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_IdUsuario",
                table: "Attendances",
                newName: "IX_Attendances_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_IdNivel",
                table: "Attendances",
                newName: "IX_Attendances_LevelId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Admin", "System" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Juan", "Pérez" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "María", "García" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Carlos", "Coordinador" });

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Levels_LevelId",
                table: "Attendances",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_UserId",
                table: "Attendances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Users_CreatedByUserId",
                table: "Devocionales",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devocionales_Users_UpdatedByUserId",
                table: "Devocionales",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelParticipants_Levels_LevelId",
                table: "LevelParticipants",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelParticipants_Users_UserId",
                table: "LevelParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Users_TutorId",
                table: "Levels",
                column: "TutorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Resources_ResourceId",
                table: "ResourceAssignments",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Users_AssignedByUserId",
                table: "ResourceAssignments",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAssignments_Users_VolunteerId",
                table: "ResourceAssignments",
                column: "VolunteerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_ResourceAssignments_ResourceAssignmentId",
                table: "ResourceUsageLogs",
                column: "ResourceAssignmentId",
                principalTable: "ResourceAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_Users_ReportedByUserId",
                table: "ResourceUsageLogs",
                column: "ReportedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceUsageLogs_Users_ResolvedByUserId",
                table: "ResourceUsageLogs",
                column: "ResolvedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
