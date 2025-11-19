using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangingTheUniquenessInTeamName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamName",
                table: "Teams");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamName",
                table: "Teams",
                column: "TeamName",
                unique: true,
                filter: "[IsDeleted]=0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamName",
                table: "Teams");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamName",
                table: "Teams",
                column: "TeamName",
                unique: true);
        }
    }
}
