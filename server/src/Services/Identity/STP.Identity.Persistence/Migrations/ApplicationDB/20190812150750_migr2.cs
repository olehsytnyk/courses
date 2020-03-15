using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Identity.Persistence.Migrations.ApplicationDB
{
    public partial class migr2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CopyCount",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CopyCount",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
