using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.Service.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NSubstitute.Core;

namespace AirportApp.Mvc
{
    public class FAQEntriesController : Controller
    {
        private readonly IFAQService fAQService;

        public FAQEntriesController(IFAQService fAQService)
        {
            this.fAQService = fAQService;
        }

        // GET: FAQEntries
        public async Task<IActionResult> Index()
        {
            return View(await fAQService.GetAllAsync());
        }

        // GET: FAQEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQEntry = all.FirstOrDefault(f => f.Id == id);
            if (fAQEntry == null)
            {
                return NotFound();
            }

            return View(fAQEntry);
        }

        // GET: FAQEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FAQEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,Answer,Category,ViewCount,HelpfulVotesCount,NotHelpfulVotesCount")] FAQEntry fAQEntry)
        {
            if (ModelState.IsValid)
            {
                await fAQService.AddFAQEntryAsync(fAQEntry);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQEntry);
        }

        // GET: FAQEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQntry = all.FirstOrDefault(f => f.Id == id);
            if (fAQntry == null)
            {
                return NotFound();
            }

            return View(fAQntry);
        }

        // POST: FAQEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,Answer,Category,ViewCount,HelpfulVotesCount,NotHelpfulVotesCount")] FAQEntry fAQEntry)
        {
            if (id != fAQEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await fAQService.EditFAQEntryAsync(fAQEntry, id);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQEntry);
        }

        // GET: FAQEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var all = await fAQService.GetAllAsync();
            var fAQEntry = all.FirstOrDefault(f => f.Id == id);

            if (fAQEntry == null)
            {
                return NotFound();
            }

            return View(fAQEntry);
        }

        // POST: FAQEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await fAQService.DeleteFAQEntryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
