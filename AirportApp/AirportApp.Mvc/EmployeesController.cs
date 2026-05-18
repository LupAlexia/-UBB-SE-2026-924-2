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
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await this.employeeService.GetAllEmployeesAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await this.employeeService.GetEmployeeByIdAsync((int)id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignedDepartment,Id,FullName,EmailAddress,Discriminator")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                await this.employeeService.AddEmployeeAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await this.employeeService.GetEmployeeByIdAsync((int)id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignedDepartment,Id,FullName,EmailAddress,Discriminator")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Employee employeeToUpdate = await this.employeeService.GetEmployeeByIdAsync(id);
                if (employeeToUpdate == null)
                {
                    return NotFound();
                }
                await this.employeeService.UpdateEmployeeByIdAsync(id, employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await this.employeeService.GetEmployeeByIdAsync((int)id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await this.employeeService.GetEmployeeByIdAsync((int)id);
            if (employee == null)
            {
                return NotFound();
            }
            await this.employeeService.DeleteEmployeeByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return await this.employeeService.GetEmployeeByIdAsync(id) != null;
        }
    }
}
