using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GitActivityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RepoUrl",
                table: "GitRepositories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CommitHash",
                table: "GitActivities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PullRequestAction",
                table: "GitActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PullRequestId",
                table: "GitActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceBranch",
                table: "GitActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetBranch",
                table: "GitActivities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitRepositories_RepoUrl",
                table: "GitRepositories",
                column: "RepoUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitActivities_CommitHash",
                table: "GitActivities",
                column: "CommitHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GitRepositories_RepoUrl",
                table: "GitRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitActivities_CommitHash",
                table: "GitActivities");

            migrationBuilder.DropColumn(
                name: "PullRequestAction",
                table: "GitActivities");

            migrationBuilder.DropColumn(
                name: "PullRequestId",
                table: "GitActivities");

            migrationBuilder.DropColumn(
                name: "SourceBranch",
                table: "GitActivities");

            migrationBuilder.DropColumn(
                name: "TargetBranch",
                table: "GitActivities");

            migrationBuilder.AlterColumn<string>(
                name: "RepoUrl",
                table: "GitRepositories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CommitHash",
                table: "GitActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
