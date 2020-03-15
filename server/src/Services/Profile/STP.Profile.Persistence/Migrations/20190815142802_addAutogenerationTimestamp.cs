using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Profile.Persistence.Migrations
{
    public partial class addAutogenerationTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "position",
                nullable: false,
                defaultValueSql: "current_timestamp(6) ON UPDATE current_timestamp(6)",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "orders",
                nullable: false,
                defaultValueSql: "current_timestamp(6) ON UPDATE current_timestamp(6)",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "orders",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "position",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "current_timestamp(6) ON UPDATE current_timestamp(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "orders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "current_timestamp(6) ON UPDATE current_timestamp(6)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "orders",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
