using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace postgres_net_minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000000"),
                columns: new[] { "HashedPassword", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "$2a$12$AIyvBPaFAwg.HVFlbCXTMugw8A4hU6LKlJCCbqxMiFvobB3eu0JYW", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                columns: new[] { "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000000"),
                column: "HashedPassword",
                value: "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Ke");
        }
    }
}
