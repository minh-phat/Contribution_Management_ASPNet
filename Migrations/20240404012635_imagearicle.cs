using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolProject1640.Migrations
{
    /// <inheritdoc />
    public partial class imagearicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Article",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1934), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1945) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1947), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1948) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1949), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1950) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1951), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1952) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "5",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1953), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1953) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "6",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1955), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1955) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "7",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1957), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1957) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "8",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1958), new DateTime(2024, 4, 4, 8, 26, 34, 899, DateTimeKind.Local).AddTicks(1959) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Article");

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7170), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7180) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7182), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7183) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7184), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7184) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7186), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7186) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "5",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7187), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7188) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "6",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7189), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7189) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "7",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7191), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7191) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "8",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7192), new DateTime(2024, 4, 3, 20, 7, 31, 423, DateTimeKind.Local).AddTicks(7193) });
        }
    }
}
