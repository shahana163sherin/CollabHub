using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameAssignedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedUserId",
                table: "TaskDefinitions");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "TaskDefinitions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_AssignedUserId",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_UserId");

            migrationBuilder.AddColumn<int>(
                name: "AssignedMemberId",
                table: "TaskDefinitions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskDefinitions_AssignedMemberId",
                table: "TaskDefinitions",
                column: "AssignedMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_TeamMembers_AssignedMemberId",
                table: "TaskDefinitions",
                column: "AssignedMemberId",
                principalTable: "TeamMembers",
                principalColumn: "TeamMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_UserId",
                table: "TaskDefinitions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_TeamMembers_AssignedMemberId",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_UserId",
                table: "TaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_TaskDefinitions_AssignedMemberId",
                table: "TaskDefinitions");

            migrationBuilder.DropColumn(
                name: "AssignedMemberId",
                table: "TaskDefinitions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TaskDefinitions",
                newName: "AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_UserId",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedUserId",
                table: "TaskDefinitions",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
