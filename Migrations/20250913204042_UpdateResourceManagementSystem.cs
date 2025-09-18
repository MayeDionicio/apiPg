using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiPG.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResourceManagementSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceId = table.Column<int>(type: "integer", nullable: false),
                    VolunteerId = table.Column<int>(type: "integer", nullable: false),
                    QuantityAssigned = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReturnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpectedReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InitialNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VolunteerNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AssignedByUserId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceAssignments_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceAssignments_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceAssignments_Users_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceAssignmentId = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ConditionBefore = table.Column<int>(type: "integer", nullable: true),
                    ConditionAfter = table.Column<int>(type: "integer", nullable: true),
                    QuantityAffected = table.Column<int>(type: "integer", nullable: true),
                    ActionsTaken = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Recommendations = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RequiresFollowUp = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReportedByUserId = table.Column<int>(type: "integer", nullable: false),
                    PhotoUrls = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceUsageLogs_ResourceAssignments_ResourceAssignmentId",
                        column: x => x.ResourceAssignmentId,
                        principalTable: "ResourceAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceUsageLogs_Users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceUsageLogs_Users_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_AssignedAt",
                table: "ResourceAssignments",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_AssignedByUserId",
                table: "ResourceAssignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_ResourceId",
                table: "ResourceAssignments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_Status",
                table: "ResourceAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAssignments_VolunteerId",
                table: "ResourceAssignments",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Category",
                table: "Resources",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_IsActive",
                table: "Resources",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Name",
                table: "Resources",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_CreatedAt",
                table: "ResourceUsageLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_EventType",
                table: "ResourceUsageLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_IsResolved",
                table: "ResourceUsageLogs",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_ReportedByUserId",
                table: "ResourceUsageLogs",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_ResolvedByUserId",
                table: "ResourceUsageLogs",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUsageLogs_ResourceAssignmentId",
                table: "ResourceUsageLogs",
                column: "ResourceAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceUsageLogs");

            migrationBuilder.DropTable(
                name: "ResourceAssignments");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
