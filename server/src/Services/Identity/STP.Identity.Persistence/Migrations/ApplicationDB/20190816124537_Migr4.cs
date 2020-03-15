using Microsoft.EntityFrameworkCore.Migrations;

namespace STP.Identity.Persistence.Migrations.ApplicationDb
{
    public partial class Migr4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "aspnetuseravatars",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "aspnetuseravatars",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);
        }
    }
}
