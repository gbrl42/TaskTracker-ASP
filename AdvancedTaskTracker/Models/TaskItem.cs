using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdvancedTaskTracker.Models;

public enum Priority
{
    Low,
    Medium,
    High
}

public enum Status
{
    Backlog,
    [Display(Name = "To Do")]
    ToDo,
    [Display(Name = "In Progress")]
    InProgress,
    Done
}

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    
    [BindNever]
    public string? UserId { get; set; }
    [BindNever]
    public IdentityUser? User { get; set; }
    
}