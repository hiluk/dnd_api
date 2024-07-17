using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_class_entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    hit_dice = table.Column<string>(type: "text", nullable: false),
                    hp_at1st_level = table.Column<string>(type: "text", nullable: false),
                    hp_at_higher_levels = table.Column<string>(type: "text", nullable: false),
                    prof_armor = table.Column<string>(type: "text", nullable: false),
                    prof_weapon = table.Column<string>(type: "text", nullable: false),
                    prof_tool = table.Column<string>(type: "text", nullable: false),
                    prof_saving_throws = table.Column<string>(type: "text", nullable: false),
                    prof_skills = table.Column<string>(type: "text", nullable: false),
                    equipment = table.Column<string>(type: "text", nullable: false),
                    spell_casting_ability = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_class_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<byte>(type: "smallint", nullable: false),
                    character_class = table.Column<int>(type: "integer", nullable: false),
                    character_race = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "race_entity",
                columns: table => new
                {
                    race_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    age = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<string>(type: "text", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false),
                    vision = table.Column<string>(type: "text", nullable: false),
                    traits = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_race_entity", x => x.race_id);
                });

            migrationBuilder.CreateTable(
                name: "user_entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_stats",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "integer", nullable: false),
                    strength = table.Column<int>(type: "integer", nullable: false),
                    intelligence = table.Column<int>(type: "integer", nullable: false),
                    wisdom = table.Column<int>(type: "integer", nullable: false),
                    constitution = table.Column<int>(type: "integer", nullable: false),
                    dexterity = table.Column<int>(type: "integer", nullable: false),
                    charisma = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_stats", x => x.character_id);
                    table.ForeignKey(
                        name: "fk_character_stats_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asi_entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    race_id = table.Column<long>(type: "bigint", nullable: false),
                    stat = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asi_entity", x => x.id);
                    table.ForeignKey(
                        name: "fk_asi_entity_race_entity_race_id",
                        column: x => x.race_id,
                        principalTable: "race_entity",
                        principalColumn: "race_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "speed_entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    race_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_speed_entity", x => x.id);
                    table.ForeignKey(
                        name: "fk_speed_entity_race_entity_race_id",
                        column: x => x.race_id,
                        principalTable: "race_entity",
                        principalColumn: "race_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asi_entity_race_id",
                table: "asi_entity",
                column: "race_id");

            migrationBuilder.CreateIndex(
                name: "ix_speed_entity_race_id",
                table: "speed_entity",
                column: "race_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asi_entity");

            migrationBuilder.DropTable(
                name: "character_class_entity");

            migrationBuilder.DropTable(
                name: "character_stats");

            migrationBuilder.DropTable(
                name: "speed_entity");

            migrationBuilder.DropTable(
                name: "user_entity");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "race_entity");
        }
    }
}
