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
    public class FAQNodesController : Controller
    {
        private readonly IDecisionTreeService decisionTreeService;

        public FAQNodesController(IDecisionTreeService decisionTreeService)
        {
            this.decisionTreeService = decisionTreeService;
        }

        // GET: FAQNodes
        public async Task<IActionResult> Index()
        {
            return View(await this.decisionTreeService.GetAllNodesAsync());
        }

        // GET: FAQNodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await this.decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }

            return View(fAQNode);
        }

        // GET: FAQNodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FAQNodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NodeId,QuestionText,IsFinalAnswer")] FAQNode fAQNode)
        {
            if (ModelState.IsValid)
            {
                await this.decisionTreeService.CreateNodeAsync(fAQNode);
                return RedirectToAction(nameof(Index));
            }
            return View(fAQNode);
        }

        // GET: FAQNodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await this.decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }
            return View(fAQNode);
        }

        // POST: FAQNodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NodeId,QuestionText,IsFinalAnswer")] FAQNode fAQNode)
        {
            if (id != fAQNode.NodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await this.decisionTreeService.UpdateNodeAsync(id, fAQNode);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FAQNodeExists(fAQNode.NodeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fAQNode);
        }

        // GET: FAQNodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQNode = await this.decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode == null)
            {
                return NotFound();
            }

            return View(fAQNode);
        }

        // POST: FAQNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fAQNode = await this.decisionTreeService.GetNodeByIdAsync((int)id);
            if (fAQNode != null)
            {
                await this.decisionTreeService.DeleteNodeAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FAQNodeExists(int id)
        {
            return this.decisionTreeService.GetNodeByIdAsync((int)id) != null;
        }
    }
}
