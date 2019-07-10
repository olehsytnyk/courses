using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCoreSample.Persistance.Migrations
{
    public partial class PostV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_employee_ReportsToId",
                schema: "efcoresample",
                table: "employee");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DeleteData(
                schema: "efcoresample",
                table: "Project",
                keyColumn: "Id",
                keyValue: new Guid("325f62e8-da6d-4ec5-a6d5-1b8bc8da6add"));

            migrationBuilder.DeleteData(
                schema: "efcoresample",
                table: "Project",
                keyColumn: "Id",
                keyValue: new Guid("9186a008-7f40-4005-b246-8c6cb2319719"));

            migrationBuilder.AlterColumn<long>(
                name: "ReportsToId",
                schema: "efcoresample",
                table: "employee",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.InsertData(
                schema: "efcoresample",
                table: "Project",
                columns: new[] { "Id", "Discription", "EmployeeId", "EndTime", "LastUpdateTime", "StartTime", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("3c5afe8a-b85d-41d9-918f-326de14232bc"), "Discription", 0L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending", "Title" },
                    { new Guid("51b5a781-8d4c-411c-a8f9-0926eb1d17a3"), "Discription2", 0L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending", "Title2" }
                });

            migrationBuilder.UpdateData(
                schema: "efcoresample",
                table: "employee",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ReportsToId",
                value: 0L);

            migrationBuilder.UpdateData(
                schema: "efcoresample",
                table: "employee",
                keyColumn: "Id",
                keyValue: 2L,
                column: "ReportsToId",
                value: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_employee_ReportsToId",
                schema: "efcoresample",
                table: "employee",
                column: "ReportsToId",
                principalSchema: "efcoresample",
                principalTable: "employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_employee_ReportsToId",
                schema: "efcoresample",
                table: "employee");

            migrationBuilder.DeleteData(
                schema: "efcoresample",
                table: "Project",
                keyColumn: "Id",
                keyValue: new Guid("3c5afe8a-b85d-41d9-918f-326de14232bc"));

            migrationBuilder.DeleteData(
                schema: "efcoresample",
                table: "Project",
                keyColumn: "Id",
                keyValue: new Guid("51b5a781-8d4c-411c-a8f9-0926eb1d17a3"));

            migrationBuilder.AlterColumn<long>(
                name: "ReportsToId",
                schema: "efcoresample",
                table: "employee",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "efcoresample",
                table: "Project",
                columns: new[] { "Id", "Discription", "EmployeeId", "EndTime", "LastUpdateTime", "StartTime", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("325f62e8-da6d-4ec5-a6d5-1b8bc8da6add"), "Discription", 0L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending", "Title" },
                    { new Guid("9186a008-7f40-4005-b246-8c6cb2319719"), "Discription2", 0L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending", "Title2" }
                });

            migrationBuilder.UpdateData(
                schema: "efcoresample",
                table: "employee",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ReportsToId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "efcoresample",
                table: "employee",
                keyColumn: "Id",
                keyValue: 2L,
                column: "ReportsToId",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_employee_ReportsToId",
                schema: "efcoresample",
                table: "employee",
                column: "ReportsToId",
                principalSchema: "efcoresample",
                principalTable: "employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
