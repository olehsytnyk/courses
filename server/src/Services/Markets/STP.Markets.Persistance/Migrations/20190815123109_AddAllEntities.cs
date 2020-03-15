using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Markets.Persistance.Migrations
{
    public partial class AddAllEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "markets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    TickSize = table.Column<double>(nullable: false),
                    MinPrice = table.Column<double>(nullable: false),
                    MaxPrice = table.Column<double>(nullable: false),
                    Kind = table.Column<int>(nullable: false),
                    Currency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "watchlists",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketWatchlists",
                columns: table => new
                {
                    MarketId = table.Column<long>(nullable: false),
                    WatchlistId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketWatchlists", x => new { x.MarketId, x.WatchlistId });
                    table.ForeignKey(
                        name: "FK_MarketWatchlists_markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketWatchlists_watchlists_WatchlistId",
                        column: x => x.WatchlistId,
                        principalTable: "watchlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketWatchlists_WatchlistId",
                table: "MarketWatchlists",
                column: "WatchlistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketWatchlists");

            migrationBuilder.DropTable(
                name: "markets");

            migrationBuilder.DropTable(
                name: "watchlists");
        }
    }
}
