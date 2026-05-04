using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddOns",
                columns: table => new
                {
                    AddOn_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Base_Price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOns", x => x.AddOn_Id);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Airport_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Airport_Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Airport_Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Company_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Company_Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Employee_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email_Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Department = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Employee_Id);
                });

            migrationBuilder.CreateTable(
                name: "FAQNode",
                columns: table => new
                {
                    node_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_final_answer = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQNode", x => x.node_id);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQ_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer_Text = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    View_Count = table.Column<int>(type: "int", nullable: false),
                    Helpful_Votes = table.Column<int>(type: "int", nullable: false),
                    Not_Helpful_Votes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.FAQ_Id);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Membership_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flight_Discount_Percentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Membership_Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketCategories",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Default_Urgency_Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCategories", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email_Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Gates",
                columns: table => new
                {
                    Gate_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gate_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Airport_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.Gate_Id);
                    table.ForeignKey(
                        name: "FK_Gates_Airports_Airport_Id",
                        column: x => x.Airport_Id,
                        principalTable: "Airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Route_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company_Id = table.Column<int>(type: "int", nullable: false),
                    Airport_Id = table.Column<int>(type: "int", nullable: false),
                    Route_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Departure_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arrival_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Route_Id);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_Airport_Id",
                        column: x => x.Airport_Id,
                        principalTable: "Airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routes_Companies_Company_Id",
                        column: x => x.Company_Id,
                        principalTable: "Companies",
                        principalColumn: "Company_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FAQOption",
                columns: table => new
                {
                    node_id = table.Column<int>(type: "int", nullable: false),
                    label = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    next_option_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQOption", x => new { x.node_id, x.label });
                    table.ForeignKey(
                        name: "FK_FAQOption_FAQNode_node_id",
                        column: x => x.node_id,
                        principalTable: "FAQNode",
                        principalColumn: "node_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Customer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password_Hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Membership_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_Id);
                    table.ForeignKey(
                        name: "FK_Customers_Memberships_Membership_Id",
                        column: x => x.Membership_Id,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Membership_Addon_Discounts",
                columns: table => new
                {
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    AddOnId = table.Column<int>(type: "int", nullable: false),
                    Discount_Percentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membership_Addon_Discounts", x => new { x.MembershipId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_AddOns_AddOnId",
                        column: x => x.AddOnId,
                        principalTable: "AddOns",
                        principalColumn: "AddOn_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSubcategories",
                columns: table => new
                {
                    Subcategory_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subcategory_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    External_Reference_Id = table.Column<int>(type: "int", nullable: false),
                    Parent_Category_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSubcategories", x => x.Subcategory_Id);
                    table.ForeignKey(
                        name: "FK_TicketSubcategories_TicketCategories_Parent_Category_Id",
                        column: x => x.Parent_Category_Id,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Chat_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Chat_Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Chat_Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Duty_Free_Rating = table.Column<int>(type: "int", nullable: false),
                    Flight_Experience_Rating = table.Column<int>(type: "int", nullable: false),
                    Staff_Friendliness_Rating = table.Column<int>(type: "int", nullable: false),
                    Cleanliness_Rating = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Flight_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Route_Id = table.Column<int>(type: "int", nullable: false),
                    Gate_Id = table.Column<int>(type: "int", nullable: false),
                    Departure_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Flight_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Flight_Id);
                    table.ForeignKey(
                        name: "FK_Flights_Gates_Gate_Id",
                        column: x => x.Gate_Id,
                        principalTable: "Gates",
                        principalColumn: "Gate_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Routes_Route_Id",
                        column: x => x.Route_Id,
                        principalTable: "Routes",
                        principalColumn: "Route_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Creation_Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Urgency_Level = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Creator_Id = table.Column<int>(type: "int", nullable: false),
                    Category_Id = table.Column<int>(type: "int", nullable: false),
                    Subcategory_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Ticket_Id);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketCategories_Category_Id",
                        column: x => x.Category_Id,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketSubcategories_Subcategory_Id",
                        column: x => x.Subcategory_Id,
                        principalTable: "TicketSubcategories",
                        principalColumn: "Subcategory_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_Creator_Id",
                        column: x => x.Creator_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message_Text = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Chat_Id = table.Column<int>(type: "int", nullable: false),
                    Sender_User_Id = table.Column<int>(type: "int", nullable: true),
                    Sender_Employee_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Message_Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_Chat_Id",
                        column: x => x.Chat_Id,
                        principalTable: "Chats",
                        principalColumn: "Chat_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Employees_Sender_Employee_Id",
                        column: x => x.Sender_Employee_Id,
                        principalTable: "Employees",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_Sender_User_Id",
                        column: x => x.Sender_User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightTickets",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Flight_Id = table.Column<int>(type: "int", nullable: false),
                    Seat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Passenger_First_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Passenger_Last_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Passenger_Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Passenger_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTickets", x => x.Ticket_Id);
                    table.ForeignKey(
                        name: "FK_FlightTickets_Customers_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Customers",
                        principalColumn: "Customer_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTickets_Flights_Flight_Id",
                        column: x => x.Flight_Id,
                        principalTable: "Flights",
                        principalColumn: "Flight_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightTicket_AddOns",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false),
                    AddOn_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTicket_AddOns", x => new { x.Ticket_Id, x.AddOn_Id });
                    table.ForeignKey(
                        name: "FK_FlightTicketAddOns_AddOn",
                        column: x => x.AddOn_Id,
                        principalTable: "AddOns",
                        principalColumn: "AddOn_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTicketAddOns_FlightTicket",
                        column: x => x.Ticket_Id,
                        principalTable: "FlightTickets",
                        principalColumn: "Ticket_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AddOns",
                columns: new[] { "AddOn_Id", "Base_Price", "Name" },
                values: new object[,]
                {
                    { 1, 30f, "Extra Baggage" },
                    { 2, 15f, "Priority Boarding" }
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Airport_Id", "Airport_Code", "City" },
                values: new object[,]
                {
                    { 1, "LAX", "Los Angeles" },
                    { 2, "JFK", "New York" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Company_Id", "Name" },
                values: new object[,]
                {
                    { 1, "Acme Airlines" },
                    { 2, "Contoso Air" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Employee_Id", "Department", "Email_Address", "Full_Name" },
                values: new object[,]
                {
                    { 1, 5, "alice@acme.com", "Alice Smith" },
                    { 2, 3, "bob@contoso.com", "Bob Johnson" }
                });

            migrationBuilder.InsertData(
                table: "FAQNode",
                columns: new[] { "node_id", "is_final_answer", "question_text" },
                values: new object[,]
                {
                    { 1, false, "Welcome! How can I help you today?" },
                    { 2, true, "Flights information: You can search and book flights." },
                    { 3, true, "Membership information: View plans and discounts." }
                });

            migrationBuilder.InsertData(
                table: "FAQs",
                columns: new[] { "FAQ_Id", "Answer_Text", "Category", "Helpful_Votes", "Not_Helpful_Votes", "Question_Text", "View_Count" },
                values: new object[,]
                {
                    { 1, "Click on the 'Forgot Password' link on the login page.", 1, 120, 5, "How do I reset my password?", 150 },
                    { 2, "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", 2, 180, 10, "What is the baggage allowance?", 200 },
                    { 3, "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", 4, 160, 8, "Can I change my flight?", 180 }
                });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Membership_Id", "Flight_Discount_Percentage", "Name" },
                values: new object[,]
                {
                    { 1, 5f, "Silver" },
                    { 2, 15f, "Gold" }
                });

            migrationBuilder.InsertData(
                table: "TicketCategories",
                columns: new[] { "Category_Id", "Category_Name", "Default_Urgency_Level" },
                values: new object[,]
                {
                    { 1, "Booking Issues", 2 },
                    { 2, "General Inquiry", 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "User_Id", "Email_Address", "Full_Name" },
                values: new object[,]
                {
                    { 1, "alice@bot.com", "Alice Bot" },
                    { 2, "bob@chat.com", "Bob Chat" }
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Chat_Id", "Chat_Status", "User_Id" },
                values: new object[,]
                {
                    { 1, 0, 1 },
                    { 2, 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Customer_Id", "Email", "Membership_Id", "Password_Hash", "Phone", "Username" },
                values: new object[,]
                {
                    { 1, "user1@example.com", 1, "passhash1", "", "user1" },
                    { 2, "user2@example.com", 2, "passhash2", "", "user2" }
                });

            migrationBuilder.InsertData(
                table: "FAQOption",
                columns: new[] { "label", "node_id", "next_option_id" },
                values: new object[,]
                {
                    { "Flights", 1, 2 },
                    { "Memberships", 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Gate_Id", "Airport_Id", "Gate_Name" },
                values: new object[,]
                {
                    { 1, 1, "A1" },
                    { 2, 2, "B2" }
                });

            migrationBuilder.InsertData(
                table: "Membership_Addon_Discounts",
                columns: new[] { "AddOnId", "MembershipId", "Discount_Percentage" },
                values: new object[,]
                {
                    { 1, 1, 10f },
                    { 2, 1, 10f },
                    { 1, 2, 20f },
                    { 2, 2, 20f }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "Airport_Id", "Arrival_Time", "Capacity", "Company_Id", "Departure_Time", "Route_Type" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), 180, 1, new DateTime(2026, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "Departure" },
                    { 2, 2, new DateTime(2026, 5, 5, 12, 45, 0, 0, DateTimeKind.Unspecified), 150, 2, new DateTime(2026, 5, 5, 9, 30, 0, 0, DateTimeKind.Unspecified), "Arrival" }
                });

            migrationBuilder.InsertData(
                table: "TicketSubcategories",
                columns: new[] { "Subcategory_Id", "Parent_Category_Id", "External_Reference_Id", "Subcategory_Name" },
                values: new object[,]
                {
                    { 1, 1, 101, "Booking Error" },
                    { 2, 1, 102, "Cancellation" },
                    { 3, 2, 201, "Flight Info" }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "Gate_Id", "Route_Id" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "AC100", 1, 1 },
                    { 2, new DateTime(2026, 5, 5, 9, 30, 0, 0, DateTimeKind.Unspecified), "CT200", 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Message_Id", "Chat_Id", "Sender_Employee_Id", "Sender_User_Id", "Message_Text", "Timestamp" },
                values: new object[,]
                {
                    { 1, 1, null, 1, "Hello, I need help finding flights.", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, 2, null, 2, "Is there a baggage allowance?", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 5, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Ticket_Id", "Category_Id", "Creation_Timestamp", "Creator_Id", "Status", "Description", "Subcategory_Id", "Subject", "Urgency_Level" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "System error when attempting to book", 1, "Cannot book flight", 2 },
                    { 2, 2, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, "What is the baggage allowance?", 3, "Question about baggage", 0 }
                });

            migrationBuilder.InsertData(
                table: "FlightTickets",
                columns: new[] { "Ticket_Id", "Flight_Id", "Passenger_Email", "Passenger_First_Name", "Passenger_Last_Name", "Passenger_Phone", "Price", "Seat", "Status", "User_Id" },
                values: new object[,]
                {
                    { 1, 1, "johndoe@example.com", "John", "Doe", "", 199f, "12A", "Booked", 1 },
                    { 2, 2, "janeroe@example.com", "Jane", "Roe", "", 149f, "14C", "Booked", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_User_Id",
                table: "Chats",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Membership_Id",
                table: "Customers",
                column: "Membership_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_Gate_Id",
                table: "Flights",
                column: "Gate_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_Route_Id",
                table: "Flights",
                column: "Route_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTicket_AddOns_AddOn_Id",
                table: "FlightTicket_AddOns",
                column: "AddOn_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_Flight_Id",
                table: "FlightTickets",
                column: "Flight_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_User_Id",
                table: "FlightTickets",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Gates_Airport_Id",
                table: "Gates",
                column: "Airport_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Addon_Discounts_AddOnId",
                table: "Membership_Addon_Discounts",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Chat_Id",
                table: "Messages",
                column: "Chat_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Sender_Employee_Id",
                table: "Messages",
                column: "Sender_Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Sender_User_Id",
                table: "Messages",
                column: "Sender_User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_User_Id",
                table: "Reviews",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Airport_Id",
                table: "Routes",
                column: "Airport_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Company_Id",
                table: "Routes",
                column: "Company_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Category_Id",
                table: "Tickets",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Creator_Id",
                table: "Tickets",
                column: "Creator_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Subcategory_Id",
                table: "Tickets",
                column: "Subcategory_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSubcategories_Parent_Category_Id",
                table: "TicketSubcategories",
                column: "Parent_Category_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQOption");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "FlightTicket_AddOns");

            migrationBuilder.DropTable(
                name: "Membership_Addon_Discounts");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "FAQNode");

            migrationBuilder.DropTable(
                name: "FlightTickets");

            migrationBuilder.DropTable(
                name: "AddOns");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "TicketSubcategories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TicketCategories");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "Gates");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
