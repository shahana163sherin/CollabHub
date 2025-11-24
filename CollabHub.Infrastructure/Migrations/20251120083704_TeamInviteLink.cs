using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamInviteLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteLink",
                table: "Teams");

            migrationBuilder.AddColumn<Guid>(
                name: "InviteToken",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteToken",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "InviteLink",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
