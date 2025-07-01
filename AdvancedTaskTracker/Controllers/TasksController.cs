using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdvancedTaskTracker.Data;
using AdvancedTaskTracker.Helpers;
using AdvancedTaskTracker.Models;
using AdvancedTaskTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AdvancedTaskTracker.Controllers
{
    public class TasksController : BaseController
    {

        public TasksController(ApplicationDbContext context) : base(context) { }

        // GET: Tasks
        [Authorize]
        public async Task<IActionResult> Index(string? sortBy, bool asc = true, int page = 1, int pageSize = 10)
        {
            var tasks = await GetUserTaskItemsAsync(true);
            
            var vm = new TasksIndexViewModel(tasks, sortBy, asc, page, pageSize);
            
            vm.PopulateSelectListOptions();
            
            return View(vm);
        }
        
        // POST: Tasks
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTaskStatusPriority(int TaskId, Priority Priority, Status Status,
            string? sortBy, bool asc = true, int page = 1, int pageSize = 10)
        {
            Console.WriteLine("POST CALLED UPDATE");
            var task = await GetUserTaskItemAsync(TaskId);
            if (task == null) return NotFound();

            task.Priority = Priority;
            task.Status = Status;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sortBy, asc, page, pageSize});
        }


        // GET: Tasks/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await GetUserTaskItemAsync(id.Value, true);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // GET: Tasks/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var vm = new TaskFormViewModel(await GetUserProjectsAsync());
            return View(vm);
        }

        // POST: Tasks/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var taskItem = vm.ToTaskItem(UserId);
                
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            vm.PopulateFormOptions(await GetUserProjectsAsync(), vm.ProjectId, vm.Priority, vm.Status);
            
            return View(vm);
        }

        // GET: Tasks/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await GetUserTaskItemAsync(id.Value, true);
            
            if (taskItem == null)
            {
                return NotFound();
            }
            
            var vm =  new TaskFormViewModel(taskItem, await GetUserProjectsAsync());
            return View(vm);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskFormViewModel vm)
        {
            var task = await GetUserTaskItemAsync(id);
            
            if (task == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Set ProjectId to null when "None" option (id: 0) is selected in the form
                if (vm.ProjectId == 0)
                    vm.ProjectId = null;
                try
                {
                    task.Title = vm.Title;
                    task.Description = vm.Description;
                    task.DueDate = vm.DueDate;
                    task.Priority = vm.Priority;
                    task.Status = vm.Status;
                    task.ProjectId = vm.ProjectId;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(vm.Id))
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
            
            vm.PopulateFormOptions(await GetUserProjectsAsync());
            return View(vm);
        }

        // GET: Tasks/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await GetUserTaskItemAsync(id.Value, true);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await GetUserTaskItemAsync(id);
            if (taskItem != null)
            {
                _context.TaskItems.Remove(taskItem);
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int? id)
        {
            return _context.TaskItems.Any(p => p.Id == id);
        }

        private async Task<TaskItem?> GetUserTaskItemAsync(int id, bool includeRelated = false)
        {
            var query = _context.TaskItems.AsQueryable();

            if (includeRelated)
            {
                query = query
                    .Include(t => t.Project)
                    .Include(t => t.User);
            }
            
            return await query.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        }

        private async Task<List<TaskItem>> GetUserTaskItemsAsync(bool includeRelated = false)
        {
            var query = _context.TaskItems.AsQueryable();

            if (includeRelated)
            {
                query = query
                    .Include(t => t.Project)
                    .Include(t => t.User);
            }
            
            return await query.Where(t => t.UserId == UserId).ToListAsync();
        }
        
    }
}
