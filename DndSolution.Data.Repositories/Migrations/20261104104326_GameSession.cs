using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class GameSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_sessions",
                columns: table => new
                {
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_hp = table.Column<int>(type: "integer", nullable: false),
                    temp_hp = table.Column<int>(type: "integer", nullable: false),
                    hit_dice_spent = table.Column<int>(type: "integer", nullable: false),
                    death_successes = table.Column<int>(type: "integer", nullable: false),
                    death_failures = table.Column<int>(type: "integer", nullable: false),
                    is_stable = table.Column<bool>(type: "boolean", nullable: false),
                    is_dead = table.Column<bool>(type: "boolean", nullable: false),
                    conditions = table.Column<int[]>(type: "integer[]", nullable: false),
                    exhaustion = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_sessions", x => x.character_id);
                    table.ForeignKey(
                        name: "fk_character_sessions_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    action = table.Column<string>(type: "json", nullable: false),
                    result = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_session_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_events_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_session_events_character_id_received_at",
                table: "session_events",
                columns: new[] { "character_id", "received_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_sessions");

            migrationBuilder.DropTable(
                name: "session_events");
        }
    }
}
