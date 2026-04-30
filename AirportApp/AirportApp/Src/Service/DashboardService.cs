using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AirportApp.Src.Domain;
using AirportApp.Src.Repository;

namespace AirportApp.Src.Service
{
    public class DashboardService : IDashboardService
    {
        private const string CancelledStatus = "Cancelled";
        private const int PdfDefaultFontSize = 12;
        private const int PdfHeaderFontSize = 28;
        private const int PdfTicketIdFontSize = 14;
        private const int PdfSectionHeaderFontSize = 16;
        private const int PdfPageMarginCentimetres = 2;
        private const int PdfColumnSpacing = 5;
        private const int PdfSectionPaddingTop = 10;
        private const int PdfTotalPricePaddingTop = 15;

        private readonly IFlightTicketRepository ticketRepository;

        public DashboardService(IFlightTicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IEnumerable<FlightTicket> GetUserTickets(int userId, string ticketFilter)
        {
            var now = DateTime.Now;
            var tickets = this.ticketRepository.GetTicketsByUserId(userId)
                .Where(FlightTicket => FlightTicket.Flight != null);

            return string.Equals(ticketFilter, "Past", StringComparison.OrdinalIgnoreCase)
                ? tickets.Where(FlightTicket => FlightTicket.Flight!.Date < now).OrderByDescending(FlightTicket => FlightTicket.Flight!.Date)
                : tickets.Where(FlightTicket => FlightTicket.Flight!.Date >= now).OrderBy(FlightTicket => FlightTicket.Flight!.Date);
        }

        public string GenerateTicketPdf(FlightTicket FlightTicket)
        {
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string filePath = Path.Combine(downloadsFolder, $"WizzErr_Ticket_{FlightTicket.TicketId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(PdfPageMarginCentimetres, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(textStyle => textStyle.FontSize(PdfDefaultFontSize));

                    page.Header()
                        .Text("WizzErr Boarding Pass")
                        .SemiBold().FontSize(PdfHeaderFontSize).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(PdfColumnSpacing);
                        col.Item().Text($"FlightTicket ID: {FlightTicket.TicketId}").FontSize(PdfTicketIdFontSize).SemiBold();
                        col.Item().Text($"Status: {FlightTicket.Status}").FontColor(FlightTicket.Status == CancelledStatus ? Colors.Red.Medium : Colors.Green.Darken1);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(PdfSectionPaddingTop).Text("Flight Details").FontSize(PdfSectionHeaderFontSize).SemiBold();
                        col.Item().Text($"Flight Number: {FlightTicket.Flight?.FlightNumber ?? "N/A"}");
                        col.Item().Text($"Date: {FlightTicket.Flight?.Date:dd MMM yyyy HH:mm}");
                        col.Item().Text($"Route: {FlightTicket.Flight?.Route?.Airport?.City ?? "N/A"} ({FlightTicket.Flight?.Route?.RouteType ?? "N/A"})");
                        col.Item().Text($"Departure: {FlightTicket.Flight?.Route?.DepartureTime:HH:mm}");
                        col.Item().Text($"Arrival: {FlightTicket.Flight?.Route?.ArrivalTime:HH:mm}");
                        col.Item().Text($"Gate: {FlightTicket.Flight?.Gate?.GateName ?? "N/A"}");
                        col.Item().Text($"Seat: {FlightTicket.Seat ?? "Unassigned"}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(PdfSectionPaddingTop).Text("Passenger Information").FontSize(PdfSectionHeaderFontSize).SemiBold();
                        col.Item().Text($"Name: {FlightTicket.PassengerFirstName} {FlightTicket.PassengerLastName}");
                        col.Item().Text($"Email: {FlightTicket.PassengerEmail}");
                        col.Item().Text($"Phone: {FlightTicket.PassengerPhone}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(PdfSectionPaddingTop).Text("Selected Add-Ons").FontSize(PdfSectionHeaderFontSize).SemiBold();
                        if (FlightTicket.SelectedAddOns != null && FlightTicket.SelectedAddOns.Count > 0)
                        {
                            foreach (var addOn in FlightTicket.SelectedAddOns)
                            {
                                col.Item().Text($"• {addOn.Name}");
                            }
                        }
                        else
                        {
                            col.Item().Text("No add-ons selected");
                        }

                        col.Item().PaddingTop(PdfTotalPricePaddingTop).Text($"Total Price: {FlightTicket.Price} EUR").FontSize(PdfSectionHeaderFontSize).SemiBold();
                    });

                    page.Footer().AlignCenter().Text(textDescriptor =>
                    {
                        textDescriptor.Span("Page ");
                        textDescriptor.CurrentPageNumber();
                        textDescriptor.Span(" of ");
                        textDescriptor.TotalPages();
                    });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}
