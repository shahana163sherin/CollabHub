using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Make_ChangesInTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InviteLink",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TeamMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "TeamMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "TeamMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_CreatedBy",
                table: "TeamMembers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_DeletedBy",
                table: "TeamMembers",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ModifiedBy",
                table: "TeamMembers",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_CreatedBy",
                table: "TeamMembers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_DeletedBy",
                table: "TeamMembers",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_ModifiedBy",
                table: "TeamMembers",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_CreatedBy",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_DeletedBy",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_ModifiedBy",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_CreatedBy",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_DeletedBy",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ModifiedBy",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "TeamMembers");

            migrationBuilder.AlterColumn<string>(
                name: "InviteLink",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
