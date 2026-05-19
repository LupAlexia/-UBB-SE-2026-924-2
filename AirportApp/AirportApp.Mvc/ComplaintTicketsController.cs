using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Mvc.Models.ComplaintTicket;

namespace AirportApp.Mvc
{
    [Authorize]
    public class ComplaintTicketsController : Controller
    {
        private readonly IComplaintTicketService complaintTicketService;
        private readonly IUserService userService;
        private readonly IComplaintTicketCategoryService complaintTicketCategoryService;
        private readonly IComplaintTicketSubcategoryService complaintTicketSubcategoryService;

        public ComplaintTicketsController(
            IComplaintTicketService complaintTicketService,
            IUserService userService,
            IComplaintTicketCategoryService complaintTicketCategoryService,
            IComplaintTicketSubcategoryService complaintTicketSubcategoryService)
        {
            this.complaintTicketService = complaintTicketService;
            this.userService = userService;
            this.complaintTicketCategoryService = complaintTicketCategoryService;
            this.complaintTicketSubcategoryService = complaintTicketSubcategoryService;
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateOptionsAsync();
            return View(new CreateComplaintTicketViewModel());
        }

        // POST: ComplaintTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateComplaintTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User? creator = await ResolveCurrentUserAsync();
                    if (creator == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to resolve the current user as a ticket creator.");
                    }
                    else
                    {
                        ComplaintTicketCategory category = await this.complaintTicketCategoryService.GetCategoryByIdAsync(model.CategoryId);
                        ComplaintTicketSubcategory subcategory = await this.complaintTicketSubcategoryService.GetSubcategoryByIdAsync(model.SubcategoryId);

                        if (subcategory.ParentCategory.Id != category.Id)
                        {
                            ModelState.AddModelError(nameof(model.SubcategoryId), "The selected subcategory does not belong to the selected category.");
                        }
                        else
                        {
                            var complaintTicket = new ComplaintTicket
                            {
                                Creator = creator,
                                Category = category,
                                Subcategory = subcategory,
                                Subject = model.Subject,
                                Description = model.Description,
                                CreationTimestamp = DateTime.UtcNow,
                                CurrentStatus = ComplaintTicketStatusEnum.OPEN,
                                UrgencyLevel = model.UrgencyLevel ?? category.CategoryUrgencyLevel
                            };

                            await this.complaintTicketService.AddTicketAsync(complaintTicket);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                catch (KeyNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, "The selected category or subcategory was not found.");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await PopulateCreateOptionsAsync(model.CategoryId, model.SubcategoryId);
            return View(model);
        }

        // GET: ComplaintTickets/Edit/5
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
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

        private async Task<User?> ResolveCurrentUserAsync()
        {
            string? email = User.FindFirstValue(ClaimTypes.Email) ?? UserSession.CurrentUser?.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            List<User> users = await this.userService.GetAllUsersAsync();
            return users.FirstOrDefault(user =>
                string.Equals(user.EmailAddress, email, StringComparison.OrdinalIgnoreCase));
        }

        private async Task PopulateCreateOptionsAsync(int? selectedCategoryId = null, int? selectedSubcategoryId = null)
        {
            List<ComplaintTicketCategory> categories = (await this.complaintTicketCategoryService.GetAllCategoriesAsync()).ToList();
            List<SelectListItem> categoryItems = categories
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName,
                    Selected = selectedCategoryId.HasValue && selectedCategoryId.Value == category.Id
                })
                .ToList();

            List<SelectListItem> subcategoryItems = new List<SelectListItem>();
            foreach (ComplaintTicketCategory category in categories)
            {
                IEnumerable<ComplaintTicketSubcategory> subcategories = await this.complaintTicketSubcategoryService.GetSubcategoriesByCategoryIdAsync(category.Id);
                subcategoryItems.AddRange(subcategories.Select(subcategory => new SelectListItem
                {
                    Value = subcategory.Id.ToString(),
                    Text = $"{category.CategoryName} - {subcategory.SubcategoryName}",
                    Selected = selectedSubcategoryId.HasValue && selectedSubcategoryId.Value == subcategory.Id
                }));
            }

            ViewBag.Categories = categoryItems;
            ViewBag.Subcategories = subcategoryItems;
        }
    }
}
