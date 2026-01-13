using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pilarski.PlayerManager.DAOSQL.Migrations
{
    /// <inheritdoc />
    public partial class RelativePathFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    FoundingYear = table.Column<int>(type: "INTEGER", nullable: false),
                    RivalClubId = table.Column<int>(type: "INTEGER", nullable: true),
                    Stadium = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ClubId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nationality = table.Column<int>(type: "INTEGER", nullable: false),
                    BirthYear = table.Column<int>(type: "INTEGER", nullable: false),
                    MarketValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Foot = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillMoves = table.Column<int>(type: "INTEGER", nullable: false),
                    WeakFoot = table.Column<int>(type: "INTEGER", nullable: false),
                    Overall = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Clubs");
        }
    }
}
