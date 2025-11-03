using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskDefinitionCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_TaskHeads_TaskHeadId",
                table: "TaskDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_Users_AssignedById",
                table: "TaskDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_Users_AssignedUserId",
                table: "TaskDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_Users_CreatedBy",
                table: "TaskDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_Users_DeletedBy",
                table: "TaskDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinition_Users_ModifiedBy",
                table: "TaskDefinition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskDefinition",
                table: "TaskDefinition");

            migrationBuilder.RenameTable(
                name: "TaskDefinition",
                newName: "TaskDefinitions");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_TaskHeadId",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_TaskHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_ModifiedBy",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_DeletedBy",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_CreatedBy",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_AssignedUserId",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinition_AssignedById",
                table: "TaskDefinitions",
                newName: "IX_TaskDefinitions_AssignedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskDefinitions",
                table: "TaskDefinitions",
                column: "TaskDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_TaskHeads_TaskHeadId",
                table: "TaskDefinitions",
                column: "TaskHeadId",
                principalTable: "TaskHeads",
                principalColumn: "TaskHeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedById",
                table: "TaskDefinitions",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedUserId",
                table: "TaskDefinitions",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_CreatedBy",
                table: "TaskDefinitions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_DeletedBy",
                table: "TaskDefinitions",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinitions_Users_ModifiedBy",
                table: "TaskDefinitions",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_TaskHeads_TaskHeadId",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedById",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_AssignedUserId",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_CreatedBy",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_DeletedBy",
                table: "TaskDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskDefinitions_Users_ModifiedBy",
                table: "TaskDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskDefinitions",
                table: "TaskDefinitions");

            migrationBuilder.RenameTable(
                name: "TaskDefinitions",
                newName: "TaskDefinition");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_TaskHeadId",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_TaskHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_ModifiedBy",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_DeletedBy",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_CreatedBy",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_AssignedUserId",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskDefinitions_AssignedById",
                table: "TaskDefinition",
                newName: "IX_TaskDefinition_AssignedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskDefinition",
                table: "TaskDefinition",
                column: "TaskDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_TaskHeads_TaskHeadId",
                table: "TaskDefinition",
                column: "TaskHeadId",
                principalTable: "TaskHeads",
                principalColumn: "TaskHeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_Users_AssignedById",
                table: "TaskDefinition",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_Users_AssignedUserId",
                table: "TaskDefinition",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_Users_CreatedBy",
                table: "TaskDefinition",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_Users_DeletedBy",
                table: "TaskDefinition",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDefinition_Users_ModifiedBy",
                table: "TaskDefinition",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
