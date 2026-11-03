using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AbilityGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Старые персонажи хранили произвольные итоговые значения без способа генерации,
            // по правилам их не пересчитать (см. docs/specs/2026-10-31-ability-generation-design.md)
            migrationBuilder.Sql("DELETE FROM characters;");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "races",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "hit_die",
                table: "classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "saving_throws",
                table: "classes",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "bonus_choices",
                table: "characters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "generation_method",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ability",
                table: "asi",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "choices",
                table: "asi",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "races");

            migrationBuilder.DropColumn(
                name: "hit_die",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "saving_throws",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "type",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "bonus_choices",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "generation_method",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "ability",
                table: "asi");

            migrationBuilder.DropColumn(
                name: "choices",
                table: "asi");
        }
    }
}
