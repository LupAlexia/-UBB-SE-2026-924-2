using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                columns: new[] { "Email", "Username" },
                values: new object[] { "alice@bot.com", "alice" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 2,
                columns: new[] { "Email", "Username" },
                values: new object[] { "bob@chat.com", "bob" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 3,
                columns: new[] { "Email", "Username" },
                values: new object[] { "mia@example.com", "mia" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 1,
                columns: new[] { "Email", "Username" },
                values: new object[] { "user1@example.com", "user1" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 2,
                columns: new[] { "Email", "Username" },
                values: new object[] { "user2@example.com", "user2" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 3,
                columns: new[] { "Email", "Username" },
                values: new object[] { "user3@example.com", "user3" });
        }
    }
}
