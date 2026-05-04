using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AddOns",
                columns: new[] { "AddOn_Id", "Base_Price", "Name" },
                values: new object[,]
                {
                    { 3, 12f, "Seat Selection" },
                    { 4, 45f, "Lounge Access" }
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Airport_Id", "Airport_Code", "City" },
                values: new object[] { 3, "ORD", "Chicago" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Company_Id", "Name" },
                values: new object[] { 3, "SkyLink Airways" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Employee_Id", "Department", "Email_Address", "Full_Name" },
                values: new object[,]
                {
                    { 3, 0, "clara@skylink.com", "Clara Davis" },
                    { 4, 2, "daniel@acme.com", "Daniel Green" }
                });

            migrationBuilder.InsertData(
                table: "FAQNode",
                columns: new[] { "node_id", "is_final_answer", "question_text" },
                values: new object[,]
                {
                    { 4, true, "Baggage information: Learn what is included and what costs extra." },
                    { 5, true, "Payments information: Find out which payment methods are accepted." },
                    { 6, true, "Support information: Contact our team for help with bookings or accounts." }
                });

            migrationBuilder.InsertData(
                table: "FAQOption",
                columns: new[] { "label", "node_id", "next_option_id" },
                values: new object[,]
                {
                    { "Baggage", 1, 4 },
                    { "Contact support", 1, 6 },
                    { "Payments", 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "FAQs",
                columns: new[] { "FAQ_Id", "Answer_Text", "Category", "Helpful_Votes", "Not_Helpful_Votes", "Question_Text", "View_Count" },
                values: new object[,]
                {
                    { 4, "We accept major credit cards and selected digital wallets.", 4, 80, 2, "Which payment methods are accepted?", 95 },
                    { 5, "Use the chat assistant or submit a support ticket from your account.", 4, 110, 3, "How do I contact support?", 120 }
                });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Gate_Id", "Airport_Id", "Gate_Name" },
                values: new object[] { 4, 1, "D4" });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Membership_Id", "Flight_Discount_Percentage", "Name" },
                values: new object[] { 3, 25f, "Platinum" });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "Airport_Id", "Arrival_Time", "Capacity", "Company_Id", "Departure_Time", "Route_Type" },
                values: new object[] { 4, 1, new DateTime(2026, 5, 7, 17, 10, 0, 0, DateTimeKind.Unspecified), 160, 1, new DateTime(2026, 5, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), "Arrival" });

            migrationBuilder.InsertData(
                table: "TicketCategories",
                columns: new[] { "Category_Id", "Category_Name", "Default_Urgency_Level" },
                values: new object[] { 3, "Payment Problems", 2 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "User_Id", "Email_Address", "Full_Name" },
                values: new object[] { 3, "mia@example.com", "Mia Passenger" });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Chat_Id", "Chat_Status", "User_Id" },
                values: new object[] { 3, 0, 3 });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Customer_Id", "Email", "Membership_Id", "Password_Hash", "Phone", "Username" },
                values: new object[] { 3, "user3@example.com", 3, "passhash3", "", "user3" });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "Gate_Id", "Route_Id" },
                values: new object[] { 4, new DateTime(2026, 5, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), "AC400", 4, 4 });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Gate_Id", "Airport_Id", "Gate_Name" },
                values: new object[] { 3, 3, "C3" });

            migrationBuilder.InsertData(
                table: "Membership_Addon_Discounts",
                columns: new[] { "AddOnId", "MembershipId", "Discount_Percentage" },
                values: new object[,]
                {
                    { 1, 3, 30f },
                    { 2, 3, 30f },
                    { 3, 3, 30f },
                    { 4, 3, 35f }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "Airport_Id", "Arrival_Time", "Capacity", "Company_Id", "Departure_Time", "Route_Type" },
                values: new object[] { 3, 3, new DateTime(2026, 5, 6, 10, 5, 0, 0, DateTimeKind.Unspecified), 220, 3, new DateTime(2026, 5, 6, 7, 15, 0, 0, DateTimeKind.Unspecified), "Departure" });

            migrationBuilder.InsertData(
                table: "TicketSubcategories",
                columns: new[] { "Subcategory_Id", "Parent_Category_Id", "External_Reference_Id", "Subcategory_Name" },
                values: new object[,]
                {
                    { 4, 3, 301, "Card Declined" },
                    { 5, 3, 302, "Refund Status" }
                });

            migrationBuilder.InsertData(
                table: "FlightTickets",
                columns: new[] { "Ticket_Id", "Flight_Id", "Passenger_Email", "Passenger_First_Name", "Passenger_Last_Name", "Passenger_Phone", "Price", "Seat", "Status", "User_Id" },
                values: new object[] { 4, 4, "liamstone@example.com", "Liam", "Stone", "", 179f, "5B", "Booked", 1 });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "Gate_Id", "Route_Id" },
                values: new object[] { 3, new DateTime(2026, 5, 6, 7, 15, 0, 0, DateTimeKind.Unspecified), "SK300", 3, 3 });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Message_Id", "Chat_Id", "Sender_Employee_Id", "Sender_User_Id", "Message_Text", "Timestamp" },
                values: new object[,]
                {
                    { 3, 3, null, 3, "Can I change my booking after checkout?", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 8, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, 3, 1, null, "Yes, you can request a change from your account page.", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 9, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Ticket_Id", "Category_Id", "Creation_Timestamp", "Creator_Id", "Status", "Description", "Subcategory_Id", "Subject", "Urgency_Level" },
                values: new object[,]
                {
                    { 3, 3, new DateTime(2026, 5, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "My card was declined during checkout", 4, "Payment failed", 2 },
                    { 4, 3, new DateTime(2026, 5, 4, 12, 30, 0, 0, DateTimeKind.Unspecified), 1, 2, "How long until my refund appears?", 5, "Refund pending", 1 }
                });

            migrationBuilder.InsertData(
                table: "FlightTickets",
                columns: new[] { "Ticket_Id", "Flight_Id", "Passenger_Email", "Passenger_First_Name", "Passenger_Last_Name", "Passenger_Phone", "Price", "Seat", "Status", "User_Id" },
                values: new object[] { 3, 3, "mialane@example.com", "Mia", "Lane", "", 249f, "8F", "Booked", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Employee_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Employee_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FAQNode",
                keyColumn: "node_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FAQNode",
                keyColumn: "node_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FAQNode",
                keyColumn: "node_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FAQOption",
                keyColumns: new[] { "label", "node_id" },
                keyValues: new object[] { "Baggage", 1 });

            migrationBuilder.DeleteData(
                table: "FAQOption",
                keyColumns: new[] { "label", "node_id" },
                keyValues: new object[] { "Contact support", 1 });

            migrationBuilder.DeleteData(
                table: "FAQOption",
                keyColumns: new[] { "label", "node_id" },
                keyValues: new object[] { "Payments", 1 });

            migrationBuilder.DeleteData(
                table: "FAQs",
                keyColumn: "FAQ_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FAQs",
                keyColumn: "FAQ_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FlightTickets",
                keyColumn: "Ticket_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FlightTickets",
                keyColumn: "Ticket_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Membership_Addon_Discounts",
                keyColumns: new[] { "AddOnId", "MembershipId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "Membership_Addon_Discounts",
                keyColumns: new[] { "AddOnId", "MembershipId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "Membership_Addon_Discounts",
                keyColumns: new[] { "AddOnId", "MembershipId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Membership_Addon_Discounts",
                keyColumns: new[] { "AddOnId", "MembershipId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Message_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Message_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Ticket_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Ticket_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AddOns",
                keyColumn: "AddOn_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AddOns",
                keyColumn: "AddOn_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Chats",
                keyColumn: "Chat_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Customer_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Flight_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Flights",
                keyColumn: "Flight_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TicketSubcategories",
                keyColumn: "Subcategory_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TicketSubcategories",
                keyColumn: "Subcategory_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Gate_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gates",
                keyColumn: "Gate_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Memberships",
                keyColumn: "Membership_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumn: "Route_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumn: "Route_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TicketCategories",
                keyColumn: "Category_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "User_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Airports",
                keyColumn: "Airport_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Company_Id",
                keyValue: 3);
        }
    }
}
