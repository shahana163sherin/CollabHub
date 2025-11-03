using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Complaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Users_AssignedAdminId",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Users_CreatedBy",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Users_DeletedBy",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Users_ModifiedBy",
                table: "Complaint");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaint_Users_UserId",
                table: "Complaint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint");

            migrationBuilder.RenameTable(
                name: "Complaint",
                newName: "Complaints");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_UserId",
                table: "Complaints",
                newName: "IX_Complaints_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_ModifiedBy",
                table: "Complaints",
                newName: "IX_Complaints_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_DeletedBy",
                table: "Complaints",
                newName: "IX_Complaints_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_CreatedBy",
                table: "Complaints",
                newName: "IX_Complaints_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaint_AssignedAdminId",
                table: "Complaints",
                newName: "IX_Complaints_AssignedAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints",
                column: "ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_AssignedAdminId",
                table: "Complaints",
                column: "AssignedAdminId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_CreatedBy",
                table: "Complaints",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_DeletedBy",
                table: "Complaints",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_ModifiedBy",
                table: "Complaints",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_UserId",
                table: "Complaints",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_AssignedAdminId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_CreatedBy",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_DeletedBy",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_ModifiedBy",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_UserId",
                table: "Complaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints");

            migrationBuilder.RenameTable(
                name: "Complaints",
                newName: "Complaint");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_UserId",
                table: "Complaint",
                newName: "IX_Complaint_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_ModifiedBy",
                table: "Complaint",
                newName: "IX_Complaint_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_DeletedBy",
                table: "Complaint",
                newName: "IX_Complaint_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_CreatedBy",
                table: "Complaint",
                newName: "IX_Complaint_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_AssignedAdminId",
                table: "Complaint",
                newName: "IX_Complaint_AssignedAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaint",
                table: "Complaint",
                column: "ComplaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Users_AssignedAdminId",
                table: "Complaint",
                column: "AssignedAdminId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Users_CreatedBy",
                table: "Complaint",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Users_DeletedBy",
                table: "Complaint",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Users_ModifiedBy",
                table: "Complaint",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaint_Users_UserId",
                table: "Complaint",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
