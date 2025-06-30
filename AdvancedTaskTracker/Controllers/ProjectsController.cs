using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdvancedTaskTracker.Data;
using AdvancedTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AdvancedTaskTracker.Controllers
{
    public class ProjectsController : BaseController
    {

        public ProjectsController(ApplicationDbContext context) : base(context) { }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await GetUserProjectsAsync());
        }

        // GET: Projects/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await GetUserProjectAsync(id.Value, true);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.UserId = UserId;
                _context.Add(project);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Create finished");
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await GetUserProjectAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name,Description")] Project editedProject)
        {
            if (id != editedProject.ProjectId)
            {
                return NotFound();
            }
            
            var project = await GetUserProjectAsync(editedProject.ProjectId);
            if (project == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.Name = editedProject.Name;
                    project.Description = editedProject.Description;
                    
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(editedProject.ProjectId))
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
            return View(editedProject);
        }

        // GET: Projects/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await GetUserProjectAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await GetUserProjectAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(p => p.ProjectId == id && p.UserId == UserId);
        }

        private async Task<Project?> GetUserProjectAsync(int id, bool includeTasks = false)
        {
            var query = _context.Projects.AsQueryable();

            if (includeTasks)
            {
                query = query.Include(p => p.Tasks);
            }
            
            return await query.FirstOrDefaultAsync(p => p.ProjectId == id && p.UserId == UserId);
        }
    }
}
