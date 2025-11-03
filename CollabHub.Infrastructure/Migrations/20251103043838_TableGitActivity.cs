using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TableGitActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitActivities",
                columns: table => new
                {
                    GitActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TaskDefinitionId = table.Column<int>(type: "int", nullable: true),
                    CommitHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommitMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TriggeredNotification = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_GitActivities", x => x.GitActivityId);
                    table.ForeignKey(
                        name: "FK_GitActivities_GitRepositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "GitRepositories",
                        principalColumn: "RepositoryId");
                    table.ForeignKey(
                        name: "FK_GitActivities_TaskDefinitions_TaskDefinitionId",
                        column: x => x.TaskDefinitionId,
                        principalTable: "TaskDefinitions",
                        principalColumn: "TaskDefinitionId");
                    table.ForeignKey(
                        name: "FK_GitActivities_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitActivities_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitActivities_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_CreatedBy",
                table: "GitActivities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_DeletedBy",
                table: "GitActivities",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_ModifiedBy",
                table: "GitActivities",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_RepositoryId",
                table: "GitActivities",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_TaskDefinitionId",
                table: "GitActivities",
                column: "TaskDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_UserId",
                table: "GitActivities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitActivities");
        }
    }
}
