using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.Service;
using Microsoft.AspNetCore.Authorization;

namespace AirportApp.Mvc
{
    [Authorize]
    public class FlightTicketsController : Controller
    {
        private readonly IDashboardService dashboardService;
        private readonly IFlightSearchService flightSearchService;
        public FlightTicketsController(IDashboardService dashboard, IFlightSearchService flightSearch)
        {
            this.dashboardService = dashboard;
            this.flightSearchService = flightSearch;
        }

        private static Customer? GetCurrentUser()
        {
            return UserSession.CurrentUser;
        }

        private static int? ResolveUserId()
        {
            return GetCurrentUser()?.Id;
        }

        private async Task<FlightTicket?> GetCurrentUsersTicketAsync(int ticketId)
        {
            int? resolvedUserId = ResolveUserId();
            if (!resolvedUserId.HasValue)
            {
                return null;
            }

            IEnumerable<FlightTicket> tickets = await this.dashboardService.GetTicketsByUserIdAsync(resolvedUserId.Value);
            return tickets.FirstOrDefault(ticket => ticket.Id == ticketId);
        }

        // GET: FlightTickets
        public async Task<IActionResult> Index()
        {
            int? resolvedUserId = ResolveUserId();
            IEnumerable<FlightTicket> tickets = resolvedUserId.HasValue
                ? await this.dashboardService.GetTicketsByUserIdAsync(resolvedUserId.Value)
                : new List<FlightTicket>();

            ViewBag.UserId = resolvedUserId;
            return View(tickets);
        }

        // GET: FlightTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = ResolveUserId();

            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync((int)id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            return View(flightTicket);
        }

        // GET: FlightTickets/Create
        public IActionResult Create()
        {
            ViewBag.UserId = ResolveUserId();
            return View();
        }

        // POST: FlightTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Seat,Price,Status,PassengerFirstName,PassengerLastName,PassengerEmail,PassengerPhone")] FlightTicket flightTicket)
        {
            int? resolvedUserId = ResolveUserId();
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to create a flight ticket.");
                ViewBag.UserId = null;
                return View(flightTicket);
            }

            if (ModelState.IsValid)
            {
                flightTicket.User = new Customer { Id = resolvedUserId.Value };

                if (flightTicket.Flight == null)
                {
                    ModelState.AddModelError(string.Empty, "A flight must be selected before creating a ticket.");
                    ViewBag.UserId = resolvedUserId;
                    return View(flightTicket);
                }

                await this.dashboardService.AddTicketAsync(flightTicket);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = resolvedUserId;
            return View(flightTicket);
        }

        // GET: FlightTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = ResolveUserId();

            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync((int)id);
            if (flightTicket == null)
            {
                return NotFound();
            }
            return View(flightTicket);
        }

        // POST: FlightTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] FlightTicket flightTicket)
        {
            if (id != flightTicket.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(FlightTicket.Seat));
            ModelState.Remove(nameof(FlightTicket.Price));
            ModelState.Remove(nameof(FlightTicket.PassengerFirstName));
            ModelState.Remove(nameof(FlightTicket.PassengerLastName));
            ModelState.Remove(nameof(FlightTicket.PassengerEmail));
            ModelState.Remove(nameof(FlightTicket.PassengerPhone));
            ModelState.Remove(nameof(FlightTicket.User));
            ModelState.Remove(nameof(FlightTicket.Flight));

            int? resolvedUserId = ResolveUserId();
            if (!resolvedUserId.HasValue)
            {
                return BadRequest("A user id is required to edit a flight ticket.");
            }

            if (ModelState.IsValid)
            {
                FlightTicket? existingTicket = await GetCurrentUsersTicketAsync(id);
                if (existingTicket == null)
                {
                    return NotFound();
                }

                await this.dashboardService.UpdateTicketStatusAsync(id, flightTicket.Status);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = resolvedUserId;
            return View(flightTicket);
        }

        // GET: FlightTickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = ResolveUserId();

            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync((int)id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            return View(flightTicket);
        }

        // POST: FlightTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            FlightTicket? flightTicket = await GetCurrentUsersTicketAsync(id);
            if (flightTicket == null)
            {
                return NotFound();
            }

            await this.dashboardService.UpdateTicketStatusAsync(id, "Cancelled");
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FlightTicketExists(int id)
        {
            return await GetCurrentUsersTicketAsync(id) != null;
        }

        // GET: FlightTickets/Search
        public IActionResult Search()
        {
            ViewBag.UserId = ResolveUserId();
            return View();
        }

        // POST: FlightTickets/Search
        [HttpPost]
        public async Task<IActionResult> Search(string location, bool isDeparture, string date, string passengers)
        {
            ViewBag.UserId = ResolveUserId();
            ViewBag.Location = location;
            ViewBag.IsDeparture = isDeparture;
            ViewBag.Date = date;
            ViewBag.Passengers = passengers;

            if (string.IsNullOrEmpty(location))
            {
                ModelState.AddModelError(string.Empty, "Please enter a location.");
                return View();
            }

            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
            {
                parsedDate = d;
            }

            int? parsedPassengers = null;
            if (!string.IsNullOrEmpty(passengers))
            {
                parsedPassengers = flightSearchService.ParsePassengerCount(passengers);
            }

            try
            {
                var flights = await flightSearchService.SearchFlightsAsync(location, isDeparture, parsedDate, parsedPassengers);
                return View("SearchResults", flights);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error searching flights: {ex.Message}");
                return View();
            }
        }
    }
}
