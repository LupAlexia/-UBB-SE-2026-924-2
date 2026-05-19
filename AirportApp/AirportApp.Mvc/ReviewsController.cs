using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Mvc
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        private static Customer? GetCurrentUser()
        {
            return UserSession.CurrentUser;
        }

        private static int? ResolveUserId(int? userId)
        {
            return userId ?? GetCurrentUser()?.Id;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            return View(await reviewService.GetAllAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Review? review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Employee,Customer")]
        public IActionResult Create(int? userId)
        {
            ViewBag.UserId = ResolveUserId(userId);
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee,Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? userId, [Bind("Id,Message,DutyFreeRating,FlightExperienceRating,StaffFriendlinessRating,CleanlinessRating")] Review review)
        {
            int? resolvedUserId = ResolveUserId(userId);
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to create a review.");
                return View(review);
            }
            ModelState.Remove(nameof(Message.Chat)); // dupa auth sa fie sterse
            ModelState.Remove(nameof(Message.Sender)); // dupa auth sa fie sterse
            if (ModelState.IsValid)
            {
                review.User = new User { Id = resolvedUserId.Value }; // proxy-ul face review.User.Id
                await reviewService.AddAsync(review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,DutyFreeRating,FlightExperienceRating,StaffFriendlinessRating,CleanlinessRating")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Message.Chat)); // dupa auth sa fie sterse
            ModelState.Remove(nameof(Message.Sender)); // dupa auth sa fie sterse
            if (ModelState.IsValid)
            {
                await reviewService.UpdateByIdAsync(id, review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await reviewService.GetByIdAsync(id.Value);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await reviewService.GetByIdAsync(id);
            if (review != null)
            {
                await reviewService.DeleteByIdAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return reviewService.GetByIdAsync(id) != null;
        }
    }
}
