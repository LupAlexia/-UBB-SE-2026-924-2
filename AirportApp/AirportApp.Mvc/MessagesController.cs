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

namespace AirportApp.Mvc
{
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        private static int? ResolveUserId(int? userId)
        {
            return userId ?? UserSession.CurrentUser?.Id;
        }

        private static int? ResolveChatId(int? chatId)
        {
            return chatId ?? 1;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await messageService.GetAllAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Message message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create(int chatId, int userId)
        {
            ViewBag.ChatId = ResolveChatId(chatId);
            ViewBag.UserId = ResolveUserId(userId);
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? chatId, int? userId, [Bind("Id,Text,Timestamp")] Message message)
        {
            int? resolvedUserId = ResolveUserId(userId);
            int? resolvedChatId = ResolveChatId(chatId);

            if (!resolvedChatId.HasValue || !resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "ChatId and UserId are required.");
                ViewBag.ChatId = resolvedChatId;
                ViewBag.UserId = resolvedUserId;
                return View(message);
            }

            if (ModelState.IsValid)
            {
                await messageService.CreateMessageAsync(
                    resolvedChatId.Value,
                    resolvedUserId.Value,
                    message.Text,
                    DateTimeOffset.UtcNow);
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Message message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Timestamp")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await messageService.UpdateByIdAsync(id, message);
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await messageService.GetByIdAsync(id);
            if (message != null)
            {
                await messageService.DeleteByIdAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MessageExists(int id)
        {
            return await messageService.GetByIdAsync(id) != null;
        }
    }
}
