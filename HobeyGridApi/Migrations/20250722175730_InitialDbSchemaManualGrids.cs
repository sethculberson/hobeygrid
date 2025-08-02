using System;
using HobeyGridApi.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HobeyGridApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbSchemaManualGrids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "awards",
                columns: table => new
                {
                    award_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    award_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_awards", x => x.award_id);
                });

            migrationBuilder.CreateTable(
                name: "college_teams",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    team_name = table.Column<string>(type: "text", nullable: false),
                    abbreviation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_college_teams", x => x.team_id);
                });

            migrationBuilder.CreateTable(
                name: "grids",
                columns: table => new
                {
                    grid_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grid_date = table.Column<DateOnly>(type: "date", nullable: false),
                    RowCategory1 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    RowCategory2 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    RowCategory3 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    ColCategory1 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    ColCategory2 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    ColCategory3 = table.Column<GridCategory>(type: "jsonb", nullable: false),
                    correct_answers_json = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grids", x => x.grid_id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    pos = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.player_id);
                });

            migrationBuilder.CreateTable(
                name: "player_awards",
                columns: table => new
                {
                    player_award_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    award_id = table.Column<int>(type: "integer", nullable: false),
                    season_year = table.Column<short>(type: "smallint", nullable: true),
                    team_id = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_awards", x => x.player_award_id);
                    table.ForeignKey(
                        name: "FK_player_awards_awards_award_id",
                        column: x => x.award_id,
                        principalTable: "awards",
                        principalColumn: "award_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_awards_college_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "college_teams",
                        principalColumn: "team_id");
                    table.ForeignKey(
                        name: "FK_player_awards_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_season_stats",
                columns: table => new
                {
                    stat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    season_year = table.Column<short>(type: "smallint", nullable: false),
                    gp = table.Column<short>(type: "smallint", nullable: false),
                    g = table.Column<short>(type: "smallint", nullable: false),
                    a = table.Column<short>(type: "smallint", nullable: false),
                    tp = table.Column<short>(type: "smallint", nullable: false),
                    ppg = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    pim = table.Column<short>(type: "smallint", nullable: false),
                    pm = table.Column<short>(type: "smallint", nullable: false),
                    is_captain = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_season_stats", x => x.stat_id);
                    table.ForeignKey(
                        name: "FK_player_season_stats_college_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "college_teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_season_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_awards_award_name",
                table: "awards",
                column: "award_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_college_teams_team_name",
                table: "college_teams",
                column: "team_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grids_grid_date",
                table: "grids",
                column: "grid_date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_awards_award_id",
                table: "player_awards",
                column: "award_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_awards_player_id",
                table: "player_awards",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_awards_team_id",
                table: "player_awards",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_season_stats_player_id_team_id_season_year",
                table: "player_season_stats",
                columns: new[] { "player_id", "team_id", "season_year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_season_stats_team_id",
                table: "player_season_stats",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grids");

            migrationBuilder.DropTable(
                name: "player_awards");

            migrationBuilder.DropTable(
                name: "player_season_stats");

            migrationBuilder.DropTable(
                name: "awards");

            migrationBuilder.DropTable(
                name: "college_teams");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
