using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdvancedTaskTracker.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    
    [BindNever]
    public string? UserId { get; set; }
    [BindNever]
    public IdentityUser? User { get; set; }
}