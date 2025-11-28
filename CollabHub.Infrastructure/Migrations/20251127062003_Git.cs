using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Git : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitRepositories_TaskHeads_TaskHeadId",
                table: "GitRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitRepositories_TaskHeadId",
                table: "GitRepositories");

            migrationBuilder.DropColumn(
                name: "TaskHeadId",
                table: "GitRepositories");

            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "GitRepositories",
                newName: "DefaultBranch");

            migrationBuilder.RenameColumn(
                name: "TargetBranch",
                table: "GitActivities",
                newName: "PullRequestUrl");

            migrationBuilder.RenameColumn(
                name: "SourceBranch",
                table: "GitActivities",
                newName: "DetectedTaskCode");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "GitActivities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TaskIdExtracted",
                table: "GitActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "GitActivities");

            migrationBuilder.DropColumn(
                name: "TaskIdExtracted",
                table: "GitActivities");

            migrationBuilder.RenameColumn(
                name: "DefaultBranch",
                table: "GitRepositories",
                newName: "BranchName");

            migrationBuilder.RenameColumn(
                name: "PullRequestUrl",
                table: "GitActivities",
                newName: "TargetBranch");

            migrationBuilder.RenameColumn(
                name: "DetectedTaskCode",
                table: "GitActivities",
                newName: "SourceBranch");

            migrationBuilder.AddColumn<int>(
                name: "TaskHeadId",
                table: "GitRepositories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_TaskHeadId",
                table: "GitRepositories",
                column: "TaskHeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_GitRepositories_TaskHeads_TaskHeadId",
                table: "GitRepositories",
                column: "TaskHeadId",
                principalTable: "TaskHeads",
                principalColumn: "TaskHeadId");
        }
    }
}
