using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Profile.Persistence.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MarketId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MarketId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Kind = table.Column<int>(nullable: false),
                    Quantity = table.Column<long>(nullable: false),
                    AveragePrice = table.Column<double>(nullable: false),
                    ProfitLoss = table.Column<double>(nullable: false),
                    UnrealizedProfitLoss = table.Column<double>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    EntryOrderId = table.Column<long>(nullable: false),
                    ExitOrderId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "position");
        }
    }
}
