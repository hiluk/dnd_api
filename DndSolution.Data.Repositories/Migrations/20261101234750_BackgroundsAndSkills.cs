using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class BackgroundsAndSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // У персонажа появляется обязательная предыстория и выбор навыков
            // (см. docs/specs/2026-11-01-backgrounds-skills-design.md)
            migrationBuilder.Sql("DELETE FROM characters;");

            migrationBuilder.AddColumn<int>(
                name: "skill_choices",
                table: "races",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "skills",
                table: "races",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "expertise_choices",
                table: "classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skill_choices",
                table: "classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "skill_options",
                table: "classes",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "background",
                table: "characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "background_skills",
                table: "characters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "class_skills",
                table: "characters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "expertise",
                table: "characters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "race_skills",
                table: "characters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.CreateTable(
                name: "backgrounds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    skills = table.Column<int[]>(type: "integer[]", nullable: false),
                    skill_choices = table.Column<int>(type: "integer", nullable: false),
                    tools = table.Column<string>(type: "text", nullable: false),
                    languages = table.Column<string>(type: "text", nullable: false),
                    equipment = table.Column<string>(type: "text", nullable: false),
                    feature = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_backgrounds", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_backgrounds_type",
                table: "backgrounds",
                column: "type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "backgrounds");

            migrationBuilder.DropColumn(
                name: "skill_choices",
                table: "races");

            migrationBuilder.DropColumn(
                name: "skills",
                table: "races");

            migrationBuilder.DropColumn(
                name: "expertise_choices",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "skill_choices",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "skill_options",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "background",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "background_skills",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "class_skills",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "expertise",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "race_skills",
                table: "characters");
        }
    }
}
