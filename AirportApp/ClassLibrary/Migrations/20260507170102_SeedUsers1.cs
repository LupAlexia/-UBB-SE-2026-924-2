using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                column: "Password_Hash",
                value: "passhash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                column: "Password_Hash",
                value: "passhash1");
        }
    }
}
