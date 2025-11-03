using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GitRepositories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitRepositories",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TaskHeadId = table.Column<int>(type: "int", nullable: false),
                    RepoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCommitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCommitMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastCommitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_GitRepositories", x => x.RepositoryId);
                    table.ForeignKey(
                        name: "FK_GitRepositories_TaskHeads_TaskHeadId",
                        column: x => x.TaskHeadId,
                        principalTable: "TaskHeads",
                        principalColumn: "TaskHeadId");
                    table.ForeignKey(
                        name: "FK_GitRepositories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitRepositories_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitRepositories_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GitRepositories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_CreatedBy",
                table: "GitRepositories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_DeletedBy",
                table: "GitRepositories",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_ModifiedBy",
                table: "GitRepositories",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_TaskHeadId",
                table: "GitRepositories",
                column: "TaskHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_UserId",
                table: "GitRepositories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitRepositories");
        }
    }
}
