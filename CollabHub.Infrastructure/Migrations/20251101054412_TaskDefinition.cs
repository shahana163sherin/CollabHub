using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskDefinition",
                columns: table => new
                {
                    TaskDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskHeadId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true),
                    AssignedById = table.Column<int>(type: "int", nullable: true),
                    ExpectedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtendedTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDefinition", x => x.TaskDefinitionId);
                    table.ForeignKey(
                        name: "FK_TaskDefinition_TaskHeads_TaskHeadId",
                        column: x => x.TaskHeadId,
                        principalTable: "TaskHeads",
                        principalColumn: "TaskHeadId");
                    table.ForeignKey(
                        name: "FK_TaskDefinition_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TaskDefinition_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TaskDefinition_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TaskDefinition_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TaskDefinition_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_AssignedById",
                table: "TaskDefinition",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_AssignedUserId",
                table: "TaskDefinition",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_CreatedBy",
                table: "TaskDefinition",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_DeletedBy",
                table: "TaskDefinition",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_ModifiedBy",
                table: "TaskDefinition",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinition_TaskHeadId",
                table: "TaskDefinition",
                column: "TaskHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskDefinition");
        }
    }
}
