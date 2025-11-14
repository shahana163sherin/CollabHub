using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserAndFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImg",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "FileData",
                table: "Files",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImg",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
