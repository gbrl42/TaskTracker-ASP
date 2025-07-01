using AdvancedTaskTracker.Helpers;
using AdvancedTaskTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdvancedTaskTracker.ViewModels;

public class TasksIndexViewModel
{
    public IEnumerable<TaskItem> Tasks { get; set; }
    public string? SortBy { get; set; }
    public bool Ascending { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    
    public SelectList PriorityOptions;
    public SelectList StatusOptions;

    public TasksIndexViewModel()
    {
        Tasks = new List<TaskItem>();
    }

    public TasksIndexViewModel(IEnumerable<TaskItem> tasks, string? sortBy, bool ascending, int page, int pageSize)
    {
        SortBy = sortBy;
        Ascending = ascending;
        Page = page;
        PageSize = pageSize;

        var sortedTasks = SortUserTasks(tasks);
        
        TotalPages = (int)Math.Ceiling(tasks.Count() / (double)pageSize);
        
        Tasks = sortedTasks.Skip((page - 1) * pageSize).Take(pageSize);
        
        PopulateSelectListOptions();
    }

    private IEnumerable<TaskItem> SortUserTasks(IEnumerable<TaskItem> tasks)
    {
        if (string.IsNullOrEmpty(SortBy))
        {
            SortBy = "DueDate";
        }

        return SortBy switch
        {
            "Title" => Ascending ? tasks.OrderBy(t => t.Title) : tasks.OrderByDescending(t => t.Title),
            "DueDate" => Ascending ? tasks.OrderBy(t => t.DueDate) : tasks.OrderByDescending(t => t.DueDate),
            "Priority" => Ascending ? tasks.OrderBy(t => t.Priority) : tasks.OrderByDescending(t => t.Priority),
            "Status" => Ascending ? tasks.OrderBy(t => t.Status) : tasks.OrderByDescending(t => t.Status),
            "Project" => Ascending ? tasks.OrderBy(t => t.Project.Name) : tasks.OrderByDescending(t => t.Project.Name),
            _ => Ascending ? tasks.OrderBy(t => t.DueDate) : tasks.OrderByDescending(t => t.DueDate)
        };
    }
    
    public IEnumerable<int> GetPageNumbers()
    {
        int maxPages = 5;
        int startPage = Math.Max(1, Page - maxPages / 2);
        int endPage = Math.Min(TotalPages, startPage + maxPages - 1);
        startPage = Math.Max(1, endPage - maxPages + 1);

        return Enumerable.Range(startPage, endPage - startPage + 1);
    }
    
    public void PopulateSelectListOptions(object? selectedPriority = null, object? selectedStatus = null)
    {
        PriorityOptions = SelectListHelper.GetEnumSelectList<Priority>(selectedPriority);
        StatusOptions = SelectListHelper.GetEnumSelectList<Status>(selectedStatus);
    }

    
    
}