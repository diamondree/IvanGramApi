using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanGram.Migrations
{
    /// <inheritdoc />
    public partial class DropUserAvatarIdColumnInUserAvatarEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvatarId",
                table: "Avatars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserAvatarId",
                table: "Avatars",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
