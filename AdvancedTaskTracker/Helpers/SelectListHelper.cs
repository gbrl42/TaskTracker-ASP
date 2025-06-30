using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdvancedTaskTracker.Helpers;

public static class SelectListHelper
{
    public static SelectList GetEnumSelectList<T>(object? selected = null) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>();

        var items = values.Select(value => new
        {
            Value = value,
            Text = value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttributes(false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault()?.Name ?? value.ToString()
        });
            
        return new SelectList(items, "Value", "Text", selected);
    }
}