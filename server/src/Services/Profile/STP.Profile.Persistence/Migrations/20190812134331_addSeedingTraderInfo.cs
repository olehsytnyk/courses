using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Profile.Persistence.Migrations
{
    public partial class addSeedingTraderInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "traderinfo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ProfitLoss = table.Column<double>(nullable: false),
                    LastChanged = table.Column<DateTime>(nullable: false, defaultValueSql: "current_timestamp(6) ON UPDATE current_timestamp(6)"),
                    CopyCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traderinfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "traderinfo",
                columns: new[] { "Id", "CopyCount", "ProfitLoss" },
                values: new object[] { "UserID 1", 5, 55555.0 });

            migrationBuilder.InsertData(
                table: "traderinfo",
                columns: new[] { "Id", "CopyCount", "ProfitLoss" },
                values: new object[] { "UserID 2", 6700, 1.0 });

            migrationBuilder.InsertData(
                table: "traderinfo",
                columns: new[] { "Id", "CopyCount", "ProfitLoss" },
                values: new object[] { "UserID 3", 0, 900.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "traderinfo");
        }
    }
}
