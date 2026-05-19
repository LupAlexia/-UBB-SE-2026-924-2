using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AirportApp.Mvc
{
    [Authorize]
    public class ComplaintTicketsController : Controller
    {
        private readonly IComplaintTicketService complaintTicketService;

        public ComplaintTicketsController(IComplaintTicketService complaintTicketService)
        {
            this.complaintTicketService = complaintTicketService;
        }

        // GET: ComplaintTickets
        public async Task<IActionResult> Index()
        {
            return View(await this.complaintTicketService.GetAllTicketsAsync());
        }

        // GET: ComplaintTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await this.complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }

            return View(complaintTicket);
        }

        // GET: ComplaintTickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ComplaintTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,Description,CreationTimestamp,UrgencyLevel,CurrentStatus")] ComplaintTicket complaintTicket)
        {
            if (ModelState.IsValid)
            {
                await this.complaintTicketService.AddTicketAsync(complaintTicket);
                return RedirectToAction(nameof(Index));
            }
            return View(complaintTicket);
        }

        // GET: ComplaintTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await this.complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }
            return View(complaintTicket);
        }

        // POST: ComplaintTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UrgencyLevel,CurrentStatus")] ComplaintTicket complaintTicket)
        {
            if (id != complaintTicket.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Subject");
            ModelState.Remove("Description");
            ModelState.Remove("Creator");
            ModelState.Remove("Category");
            ModelState.Remove("Subcategory");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await ComplaintTicketExists(id))
                    {
                        return NotFound();
                    }

                    await this.complaintTicketService.UpdateUrgencyLevelAsync(id, complaintTicket.UrgencyLevel);
                    await this.complaintTicketService.UpdateStatusAsync(id, complaintTicket.CurrentStatus);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error saving in API: {ex.Message}");
                }
            }

            return View(complaintTicket);
        }

        // GET: ComplaintTickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintTicket = await this.complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket == null)
            {
                return NotFound();
            }

            return View(complaintTicket);
        }

        // POST: ComplaintTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var complaintTicket = await this.complaintTicketService.GetTicketByIdAsync((int)id);
            if (complaintTicket != null)
            {
                await this.complaintTicketService.DeleteTicketByIdAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ComplaintTicketExists(int id)
        {
            return await this.complaintTicketService.GetTicketByIdAsync((int)id) != null;
        }
    }
}
