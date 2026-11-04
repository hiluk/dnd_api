using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class CharacterAvatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_file_name",
                table: "characters",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "avatar_has_transparency",
                table: "characters",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "avatar_mode",
                table: "characters",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "avatar_offset_x",
                table: "characters",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "avatar_offset_y",
                table: "characters",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "avatar_scale",
                table: "characters",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_file_name",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "avatar_has_transparency",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "avatar_mode",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "avatar_offset_x",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "avatar_offset_y",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "avatar_scale",
                table: "characters");
        }
    }
}
