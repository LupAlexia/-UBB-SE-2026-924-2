using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                column: "Password_Hash",
                value: "AQAAAAIAAYagAAAAEI8DQJ8c6E7I7Z+pyUP0ianpOzmRoTCf45gXzPw34WPUT9ad+8pkg4R3q4LpwC4tGA==");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 2,
                column: "Password_Hash",
                value: "AQAAAAIAAYagAAAAEO6TmleYFEq86ASriOSZZBse/92WKkFnlxHJ3XjPXpAGS+60rKv4t2hPXIH4HPDT4w==");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 3,
                column: "Password_Hash",
                value: "AQAAAAIAAYagAAAAEO6cunSJet2Y5zhA5WsI3toMUd70WoG1Vft6r+W9qGYHMv57vTUtxaEB8+SAX7iPKw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                column: "Password_Hash",
                value: "passhash");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 2,
                column: "Password_Hash",
                value: "passhash2");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 3,
                column: "Password_Hash",
                value: "passhash3");
        }
    }
}
