using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NSubstitute.Core;

namespace AirportApp.Mvc
{
    [Authorize]
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
        public async Task<IActionResult> Index(int? chatId)
        {
            IEnumerable<Message> messages;
            if (chatId.HasValue)
            {
                messages = await messageService.GetByChatIdAsync(chatId.Value);
            }
            else
            {
                messages = await messageService.GetAllAsync();
            }
            ViewBag.ChatId = chatId;
            return View(messages);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id, int? chatId)
        {
            ViewBag.ChatId = chatId;
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
        public IActionResult Create(int? chatId, int? userId)
        {
            ViewBag.ChatId = chatId;
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
                    return RedirectToAction(nameof(Index), new { chatId = resolvedChatId });
            }
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id, int? chatId)
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

            ViewBag.ChatId = chatId ?? message.Chat?.Id;
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, int? chatId, [Bind("Id,Text,Timestamp")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }
            if (message.Sender == null)
            {
                ModelState.AddModelError(string.Empty, "Sender is required.");
                return View(message);
            }
            if (!chatId.HasValue)
            {
                var existingMessage = await messageService.GetByIdAsync(message.Id);
                chatId = existingMessage?.Chat?.Id;
            }

            if (!chatId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "ChatId is required.");
                ViewBag.ChatId = null;
                return View(message);
            }

            ModelState.Remove(nameof(Message.Chat));
            message.Chat = new Chat { Id = chatId.Value };
            ModelState.Remove(nameof(Message.Sender));

            if (ModelState.IsValid)
            {
                await messageService.UpdateByIdAsync((int)id, message);
                return RedirectToAction(nameof(Index), new { chatId = chatId });
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id, int? chatId)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ChatId = chatId;

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
        public async Task<IActionResult> DeleteConfirmed(int id, int? chatId)
        {
            var message = await messageService.GetByIdAsync(id);
            if (message != null)
            {
                await messageService.DeleteByIdAsync(id);
            }

            return RedirectToAction(nameof(Index), new { chatId = chatId });
        }

        private async Task<bool> MessageExists(int id)
        {
            return await messageService.GetByIdAsync(id) != null;
        }
    }
}
