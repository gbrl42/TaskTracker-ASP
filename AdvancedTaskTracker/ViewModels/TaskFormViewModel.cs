using System.ComponentModel.DataAnnotations;
using AdvancedTaskTracker.Helpers;
using AdvancedTaskTracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdvancedTaskTracker.ViewModels;

public class TaskFormViewModel
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    
    public SelectList ProjectOptions { get; set; }
    public SelectList PriorityOptions { get; set; }
    public SelectList StatusOptions { get; set; }

    public TaskFormViewModel()
    {
        ProjectOptions = new SelectList(new List<SelectListItem>());
        PriorityOptions = new SelectList(new List<SelectListItem>());
        StatusOptions = new SelectList(new List<SelectListItem>());
    }

    public TaskFormViewModel(TaskItem taskItem, List<Project> projects)
    {
        Id = taskItem.Id;
        Title = taskItem.Title;
        Description = taskItem.Description;
        ProjectId = taskItem.ProjectId == 0 ? null : taskItem.ProjectId;
        DueDate = taskItem.DueDate;
        Priority = taskItem.Priority;
        Status = taskItem.Status;
        
        PopulateFormOptions(projects, ProjectId, Priority, Status);
    }
    public TaskFormViewModel(List<Project> projects, int? selectedProjectId = null, object? selectedPriority = null,
        object? selectedStatus = null)
    {
        PopulateFormOptions(projects, selectedProjectId, selectedPriority, selectedStatus);
    }

    public void PopulateFormOptions(List<Project> projects, int? selectedProjectId = null,
        object? selectedPriority = null, object? selectedStatus = null)
    {
        projects.Insert(0, new Project { ProjectId = 0, Name = "None" });

        ProjectOptions = new SelectList(projects, "ProjectId", "Name", selectedProjectId ?? 0);
        PriorityOptions = SelectListHelper.GetEnumSelectList<Priority>(selectedPriority);
        StatusOptions = SelectListHelper.GetEnumSelectList<Status>(selectedStatus);
    }
    
    public TaskItem ToTaskItem(string userId)
    {
        return new TaskItem
        {
            Title = this.Title,
            Description = this.Description,
            DueDate = this.DueDate,
            Priority = this.Priority,
            Status = this.Status, 
            // Set ProjectId to null when "None" option (id: 0) is selected in the form
            ProjectId = this.ProjectId == 0 ? null : this.ProjectId,
            UserId = userId
        };
    }

}