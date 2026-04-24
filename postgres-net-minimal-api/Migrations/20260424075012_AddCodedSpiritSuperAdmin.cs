using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace postgres_net_minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class AddCodedSpiritSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "HashedPassword", "LastName", "MiddleName", "MotherMaidenName", "RefreshToken", "RefreshTokenExpiryTime", "RoleId", "UserName" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000001"), new DateOnly(2000, 1, 1), "codedspirit@admin.com", "Coded", "$2a$11$dae4NmZDBgaxCPjLIhX.u.apR40SCBsd9sm0PCvNdmcu4JdKldbTG", "Spirit", null, null, null, null, new Guid("10000000-0000-0000-0000-000000000000"), "codedspirit" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));
        }
    }
}
