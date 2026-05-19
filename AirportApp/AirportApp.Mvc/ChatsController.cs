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
    public class ChatsController : Controller
    {
        private readonly IChatService chatService;

        public ChatsController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        // GET: Chats
        public async Task<IActionResult> Index()
        {
            var chats = await this.chatService.GetAllChatsAsync();
            return View(chats);
        }

        // GET: Chats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var chat = await this.chatService.GetChatByIdAsync((int)id);
                return RedirectToAction("Index", "Messages", new { chatId = id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: Chats/Create
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create()
        {
            int? resolvedUserId = UserSession.CurrentUser?.Id;
            if (resolvedUserId.HasValue)
            {
                var user = new User { Id = resolvedUserId.Value };
                await this.chatService.OpenChatAsync(user);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = null;
            return View();
        }

        // POST: Chats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] Chat chat)
        {
            int? resolvedUserId = UserSession.CurrentUser?.Id;
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to create a chat.");
                ViewBag.UserId = null;
                return View(chat);
            }

            if (ModelState.IsValid)
            {
                chat.User = new User { Id = resolvedUserId.Value };
                var openedChat = await this.chatService.OpenChatAsync(chat.User);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = resolvedUserId;
            return View(chat);
        }

        // GET: Chats/Edit/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var chat = await this.chatService.GetChatByIdAsync((int)id);
                return View(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Chats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] Chat chat)
        {
            if (id != chat.Id)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("User.Id");

            if (ModelState.IsValid)
            {
                if (!await ChatExists(id))
                {
                    return NotFound();
                }

                try
                {
                    var existing = await this.chatService.GetChatByIdAsync(id);
                    chat.User = existing?.User;

                    await this.chatService.UpdateChatAsync(id, chat);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
            }
            return View(chat);
        }

        // GET: Chats/Delete/5
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var chat = await this.chatService.GetChatByIdAsync((int)id);
                return View(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Chats/Delete/5
        [Authorize(Roles = "Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chat = await this.chatService.GetChatByIdAsync((int)id);
            if (chat != null)
            {
                await this.chatService.CloseChatAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ChatExists(int id)
        {
            return await this.chatService.GetChatByIdAsync(id) != null;
        }
    }
}
