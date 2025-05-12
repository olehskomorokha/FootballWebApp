using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class REMOVE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChampionshipTeams_Championships_ChampionshipId",
                table: "ChampionshipTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Championships_ChampionshipId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Championships",
                table: "Championships");

            migrationBuilder.RenameTable(
                name: "Championships",
                newName: "Championship");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Championship",
                table: "Championship",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChampionshipTeams_Championship_ChampionshipId",
                table: "ChampionshipTeams",
                column: "ChampionshipId",
                principalTable: "Championship",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Championship_ChampionshipId",
                table: "Games",
                column: "ChampionshipId",
                principalTable: "Championship",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChampionshipTeams_Championship_ChampionshipId",
                table: "ChampionshipTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Championship_ChampionshipId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Championship",
                table: "Championship");

            migrationBuilder.RenameTable(
                name: "Championship",
                newName: "Championships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Championships",
                table: "Championships",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChampionshipTeams_Championships_ChampionshipId",
                table: "ChampionshipTeams",
                column: "ChampionshipId",
                principalTable: "Championships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Championships_ChampionshipId",
                table: "Games",
                column: "ChampionshipId",
                principalTable: "Championships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
