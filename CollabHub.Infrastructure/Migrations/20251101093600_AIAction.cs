using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AIAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_CreatedBy",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_DeletedBy",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_ModifiedBy",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_SenderId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_UserId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderId",
                table: "Notifications",
                newName: "IX_Notifications_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_ModifiedBy",
                table: "Notifications",
                newName: "IX_Notifications_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_DeletedBy",
                table: "Notifications",
                newName: "IX_Notifications_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_CreatedBy",
                table: "Notifications",
                newName: "IX_Notifications_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.CreateTable(
                name: "AiActions",
                columns: table => new
                {
                    AiActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskDefinitionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AiActions", x => x.AiActionId);
                    table.ForeignKey(
                        name: "FK_AiActions_TaskDefinitions_TaskDefinitionId",
                        column: x => x.TaskDefinitionId,
                        principalTable: "TaskDefinitions",
                        principalColumn: "TaskDefinitionId");
                    table.ForeignKey(
                        name: "FK_AiActions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_AiActions_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_AiActions_Users_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_AiActions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiActions_CreatedBy",
                table: "AiActions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiActions_DeletedBy",
                table: "AiActions",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiActions_ModifiedBy",
                table: "AiActions",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiActions_TaskDefinitionId",
                table: "AiActions",
                column: "TaskDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AiActions_UserId",
                table: "AiActions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_CreatedBy",
                table: "Notifications",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_DeletedBy",
                table: "Notifications",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_ModifiedBy",
                table: "Notifications",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_CreatedBy",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_DeletedBy",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_ModifiedBy",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_SenderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "AiActions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderId",
                table: "Notification",
                newName: "IX_Notification_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ModifiedBy",
                table: "Notification",
                newName: "IX_Notification_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_DeletedBy",
                table: "Notification",
                newName: "IX_Notification_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CreatedBy",
                table: "Notification",
                newName: "IX_Notification_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_CreatedBy",
                table: "Notification",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_DeletedBy",
                table: "Notification",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_ModifiedBy",
                table: "Notification",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_SenderId",
                table: "Notification",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
