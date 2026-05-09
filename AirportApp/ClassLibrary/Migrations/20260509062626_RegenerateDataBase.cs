using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirportApp.ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RegenerateDataBase : Migration
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
                name: "Senders",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email_Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.Sender_Id);
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
                name: "Gates",
                columns: table => new
                {
                    Gate_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gate_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AirportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.Gate_Id);
                    table.ForeignKey(
                        name: "FK_Gates_Airports_AirportId",
                        column: x => x.AirportId,
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
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AirportId = table.Column<int>(type: "int", nullable: false),
                    Route_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Departure_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arrival_Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Route_Id);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_AirportId",
                        column: x => x.AirportId,
                        principalTable: "Airports",
                        principalColumn: "Airport_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routes_Companies_CompanyId",
                        column: x => x.CompanyId,
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
                    MembershipId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_Id);
                    table.ForeignKey(
                        name: "FK_Customers_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Membership_Addon_Discounts",
                columns: table => new
                {
                    Membership_Id = table.Column<int>(type: "int", nullable: false),
                    AddOn_Id = table.Column<int>(type: "int", nullable: false),
                    Discount_Percentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membership_Addon_Discounts", x => new { x.Membership_Id, x.AddOn_Id });
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_AddOns_AddOn_Id",
                        column: x => x.AddOn_Id,
                        principalTable: "AddOns",
                        principalColumn: "AddOn_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membership_Addon_Discounts_Memberships_Membership_Id",
                        column: x => x.Membership_Id,
                        principalTable: "Memberships",
                        principalColumn: "Membership_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Employees_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Sender_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Sender_Id);
                    table.ForeignKey(
                        name: "FK_Users_Senders_Sender_Id",
                        column: x => x.Sender_Id,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
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
                    ParentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSubcategories", x => x.Subcategory_Id);
                    table.ForeignKey(
                        name: "FK_TicketSubcategories_TicketCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Flight_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    GateId = table.Column<int>(type: "int", nullable: false),
                    Departure_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Flight_Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Flight_Id);
                    table.ForeignKey(
                        name: "FK_Flights_Gates_GateId",
                        column: x => x.GateId,
                        principalTable: "Gates",
                        principalColumn: "Gate_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Route_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Chat_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Chat_Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Chat_Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
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
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Review_Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
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
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Ticket_Id);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TicketCategories",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TicketSubcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "TicketSubcategories",
                        principalColumn: "Subcategory_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightTickets",
                columns: table => new
                {
                    Ticket_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_FlightTickets_Customers_UserId",
                        column: x => x.UserId,
                        principalTable: "Customers",
                        principalColumn: "Customer_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTickets_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Flight_Id",
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
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Message_Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Chat_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Senders_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Senders",
                        principalColumn: "Sender_Id",
                        onDelete: ReferentialAction.Restrict);
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
                    { 2, 15f, "Priority Boarding" },
                    { 3, 12f, "Seat Selection" },
                    { 4, 45f, "Lounge Access" }
                });

            migrationBuilder.InsertData(
                table: "Airports",
                columns: new[] { "Airport_Id", "Airport_Code", "City" },
                values: new object[,]
                {
                    { 1, "LAX", "Los Angeles" },
                    { 2, "JFK", "New York" },
                    { 3, "ORD", "Chicago" }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Company_Id", "Name" },
                values: new object[,]
                {
                    { 1, "Acme Airlines" },
                    { 2, "Contoso Air" },
                    { 3, "SkyLink Airways" }
                });

            migrationBuilder.InsertData(
                table: "FAQNode",
                columns: new[] { "node_id", "is_final_answer", "question_text" },
                values: new object[,]
                {
                    { 1, false, "Welcome! How can I help you today?" },
                    { 2, true, "Flights information: You can search and book flights." },
                    { 3, true, "Membership information: View plans and discounts." },
                    { 4, true, "Baggage information: Learn what is included and what costs extra." },
                    { 5, true, "Payments information: Find out which payment methods are accepted." },
                    { 6, true, "Support information: Contact our team for help with bookings or accounts." }
                });

            migrationBuilder.InsertData(
                table: "FAQs",
                columns: new[] { "FAQ_Id", "Answer_Text", "Category", "Helpful_Votes", "Not_Helpful_Votes", "Question_Text", "View_Count" },
                values: new object[,]
                {
                    { 1, "Click on the 'Forgot Password' link on the login page.", 1, 120, 5, "How do I reset my password?", 150 },
                    { 2, "Standard baggage allowance is 2 checked bags. See add-ons for additional bags.", 2, 180, 10, "What is the baggage allowance?", 200 },
                    { 3, "Yes, flight changes can be made up to 24 hours before departure, subject to availability.", 4, 160, 8, "Can I change my flight?", 180 },
                    { 4, "We accept major credit cards and selected digital wallets.", 4, 80, 2, "Which payment methods are accepted?", 95 },
                    { 5, "Use the chat assistant or submit a support ticket from your account.", 4, 110, 3, "How do I contact support?", 120 }
                });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Membership_Id", "Flight_Discount_Percentage", "Name" },
                values: new object[,]
                {
                    { 1, 5f, "Silver" },
                    { 2, 15f, "Gold" },
                    { 3, 25f, "Platinum" }
                });

            migrationBuilder.InsertData(
                table: "Senders",
                columns: new[] { "Sender_Id", "Discriminator", "Email_Address", "Full_Name" },
                values: new object[,]
                {
                    { -1, "Bot", "customer-support@cloudspritzers.com", "Carlos" },
                    { 1, "Employee", "alice@acme.com", "Alice Smith" },
                    { 2, "Employee", "bob@contoso.com", "Bob Johnson" },
                    { 3, "Employee", "clara@skylink.com", "Clara Davis" },
                    { 4, "Employee", "daniel@acme.com", "Daniel Green" },
                    { 101, "User", "alice@bot.com", "Alice Bot" },
                    { 102, "User", "bob@chat.com", "Bob Chat" },
                    { 103, "User", "mia@example.com", "Mia Passenger" }
                });

            migrationBuilder.InsertData(
                table: "TicketCategories",
                columns: new[] { "Category_Id", "Category_Name", "Default_Urgency_Level" },
                values: new object[,]
                {
                    { 1, "Booking Issues", 2 },
                    { 2, "General Inquiry", 0 },
                    { 3, "Payment Problems", 2 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Customer_Id", "Email", "MembershipId", "Password_Hash", "Phone", "Username" },
                values: new object[,]
                {
                    { 101, "alice@bot.com", 1, "AQAAAAIAAYagAAAAEDrl9uYd+TsKeS/5rEQm1tMRQuxZA+Fi1GM68HJVp6X0rR54ESH8L7QueqJR+UYfuw==", string.Empty, "alice" },
                    { 102, "bob@chat.com", 2, "AQAAAAIAAYagAAAAEPwJ21d3jG//dvwQ2SxOrZZaX50Yhwp0sLrcSfOFGCjg66Yb4J/nsQzAw8xq5Nc2OA==", string.Empty, "bob" },
                    { 103, "mia@example.com", 3, "AQAAAAIAAYagAAAAEGlhWTQ/H0czyjnsB69Tet7CmjDtwtvBZp7DeEcYeczukF1speM20ohvo2uOZFl9ag==", string.Empty, "mia" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Sender_Id", "Department" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 2, 3 },
                    { 3, 0 },
                    { 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "FAQOption",
                columns: new[] { "label", "node_id", "next_option_id" },
                values: new object[,]
                {
                    { "Baggage", 1, 4 },
                    { "Contact support", 1, 6 },
                    { "Flights", 1, 2 },
                    { "Memberships", 1, 3 },
                    { "Payments", 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "Gates",
                columns: new[] { "Gate_Id", "AirportId", "Gate_Name" },
                values: new object[,]
                {
                    { 1, 1, "A1" },
                    { 2, 2, "B2" },
                    { 3, 3, "C3" },
                    { 4, 1, "D4" }
                });

            migrationBuilder.InsertData(
                table: "Membership_Addon_Discounts",
                columns: new[] { "AddOn_Id", "Membership_Id", "Discount_Percentage" },
                values: new object[,]
                {
                    { 1, 1, 10f },
                    { 2, 1, 10f },
                    { 1, 2, 20f },
                    { 2, 2, 20f },
                    { 1, 3, 30f },
                    { 2, 3, 30f },
                    { 3, 3, 30f },
                    { 4, 3, 35f }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Route_Id", "AirportId", "Arrival_Time", "Capacity", "CompanyId", "Departure_Time", "Route_Type" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), 180, 1, new DateTime(2026, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "Departure" },
                    { 2, 2, new DateTime(2026, 5, 5, 12, 45, 0, 0, DateTimeKind.Unspecified), 150, 2, new DateTime(2026, 5, 5, 9, 30, 0, 0, DateTimeKind.Unspecified), "Arrival" },
                    { 3, 3, new DateTime(2026, 5, 6, 10, 5, 0, 0, DateTimeKind.Unspecified), 220, 3, new DateTime(2026, 5, 6, 7, 15, 0, 0, DateTimeKind.Unspecified), "Departure" },
                    { 4, 1, new DateTime(2026, 5, 7, 17, 10, 0, 0, DateTimeKind.Unspecified), 160, 1, new DateTime(2026, 5, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), "Arrival" },
                    { 5, 3, new DateTime(2026, 5, 20, 15, 12, 0, 0, DateTimeKind.Unspecified), 50, 2, new DateTime(2026, 5, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), "Arrival" }
                });

            migrationBuilder.InsertData(
                table: "TicketSubcategories",
                columns: new[] { "Subcategory_Id", "ParentCategoryId", "External_Reference_Id", "Subcategory_Name" },
                values: new object[,]
                {
                    { 1, 1, 101, "Booking Error" },
                    { 2, 1, 102, "Cancellation" },
                    { 3, 2, 201, "Flight Info" },
                    { 4, 3, 301, "Card Declined" },
                    { 5, 3, 302, "Refund Status" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                column: "Sender_Id",
                values: new object[]
                {
                    101,
                    102,
                    103
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Chat_Id", "Chat_Status", "UserId" },
                values: new object[,]
                {
                    { 1, 0, 101 },
                    { 2, 0, 102 },
                    { 3, 0, 103 }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "Flight_Id", "Departure_Date", "Flight_Number", "GateId", "RouteId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "AC100", 1, 1 },
                    { 2, new DateTime(2026, 5, 5, 9, 30, 0, 0, DateTimeKind.Unspecified), "CT200", 2, 2 },
                    { 3, new DateTime(2026, 5, 6, 7, 15, 0, 0, DateTimeKind.Unspecified), "SK300", 3, 3 },
                    { 4, new DateTime(2026, 5, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), "AC400", 4, 4 },
                    { 5, new DateTime(2026, 5, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), "CT500", 4, 5 }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Ticket_Id", "CategoryId", "Creation_Timestamp", "CreatorId", "Status", "Description", "SubcategoryId", "Subject", "Urgency_Level" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), 101, 2, "System error when attempting to book", 1, "Cannot book flight", 2 },
                    { 2, 2, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Unspecified), 102, 0, "What is the baggage allowance?", 3, "Question about baggage", 0 },
                    { 3, 3, new DateTime(2026, 5, 4, 12, 0, 0, 0, DateTimeKind.Unspecified), 103, 0, "My card was declined during checkout", 4, "Payment failed", 2 },
                    { 4, 3, new DateTime(2026, 5, 4, 12, 30, 0, 0, DateTimeKind.Unspecified), 101, 2, "How long until my refund appears?", 5, "Refund pending", 1 }
                });

            migrationBuilder.InsertData(
                table: "FlightTickets",
                columns: new[] { "Ticket_Id", "FlightId", "Passenger_Email", "Passenger_First_Name", "Passenger_Last_Name", "Passenger_Phone", "Price", "Seat", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "johndoe@example.com", "John", "Doe", string.Empty, 199f, "12A", "Booked", 101 },
                    { 2, 2, "janeroe@example.com", "Jane", "Roe", string.Empty, 149f, "14C", "Booked", 102 },
                    { 3, 3, "mialane@example.com", "Mia", "Lane", string.Empty, 249f, "8F", "Booked", 103 },
                    { 4, 4, "liamstone@example.com", "Liam", "Stone", string.Empty, 179f, "5B", "Booked", 101 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Message_Id", "ChatId", "SenderId", "Message_Text", "Timestamp" },
                values: new object[,]
                {
                    { 1, 1, 101, "Hello! I need help with flights.", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)) },
                    { 2, 2, 102, "Hi, I have a question about membership.", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 5, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)) },
                    { 3, 3, 103, "Hello, I need support with my booking.", new DateTimeOffset(new DateTime(2026, 5, 4, 9, 10, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_MembershipId",
                table: "Customers",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_GateId",
                table: "Flights",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTicket_AddOns_AddOn_Id",
                table: "FlightTicket_AddOns",
                column: "AddOn_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_FlightId",
                table: "FlightTickets",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTickets_UserId",
                table: "FlightTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Gates_AirportId",
                table: "Gates",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Membership_Addon_Discounts_AddOn_Id",
                table: "Membership_Addon_Discounts",
                column: "AddOn_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_AirportId",
                table: "Routes",
                column: "AirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_CompanyId",
                table: "Routes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatorId",
                table: "Tickets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubcategoryId",
                table: "Tickets",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSubcategories_ParentCategoryId",
                table: "TicketSubcategories",
                column: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

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
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
