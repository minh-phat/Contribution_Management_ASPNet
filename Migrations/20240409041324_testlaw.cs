using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolProject1640.Migrations
{
    /// <inheritdoc />
    public partial class testlaw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermAndCon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TermsAndCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermAndCon", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2009), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2024) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2026), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2027) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2028), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2029) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2030), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2031) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "5",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2032), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2032) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "6",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2034), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2034) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "7",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2035), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2036) });

            migrationBuilder.UpdateData(
                table: "Faculty",
                keyColumn: "Id",
                keyValue: "8",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2037), new DateTime(2024, 4, 9, 11, 13, 24, 261, DateTimeKind.Local).AddTicks(2038) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermAndCon");

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
    }
}
