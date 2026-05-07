using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "Airport_Id", "Arrival_Time", "Capacity", "Company_Id", "Departure_Time", "Route_Type" },
                values: new object[] { 5, 3, new DateTime(2026, 5, 20, 15, 12, 0, 0, DateTimeKind.Unspecified), 50, 2, new DateTime(2026, 5, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), "Arrival" });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "Gate_Id", "Route_Id" },
                values: new object[] { 5, new DateTime(2026, 5, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), "CT500", 4, 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Flight_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumn: "Route_Id",
                keyValue: 5);
        }
    }
}
