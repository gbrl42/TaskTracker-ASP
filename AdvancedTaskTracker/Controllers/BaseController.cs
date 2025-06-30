using System.Security.Claims;
using AdvancedTaskTracker.Data;
using AdvancedTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTaskTracker.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;

    protected BaseController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    protected async Task<List<Project>> GetUserProjectsAsync()
    {
        return await _context.Projects.Where(p => p.UserId == UserId).ToListAsync();
    }

}