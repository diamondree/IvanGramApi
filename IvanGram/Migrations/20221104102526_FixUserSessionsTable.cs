using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanGram.Migrations
{
    /// <inheritdoc />
    public partial class FixUserSessionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSession_Users_UserId",
                table: "UserSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSession",
                table: "UserSession");

            migrationBuilder.RenameTable(
                name: "UserSession",
                newName: "Sessions");

            migrationBuilder.RenameIndex(
                name: "IX_UserSession_UserId",
                table: "Sessions",
                newName: "IX_Sessions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserId",
                table: "Sessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserId",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "UserSession");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_UserId",
                table: "UserSession",
                newName: "IX_UserSession_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSession",
                table: "UserSession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSession_Users_UserId",
                table: "UserSession",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
