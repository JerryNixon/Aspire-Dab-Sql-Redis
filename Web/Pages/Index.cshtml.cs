using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Web.Models;
using Web.Repositories;

namespace Web.Pages;

public class IndexModel : PageModel
{
    public Todo[] PendingTodos { get; set; } = [];

    public Todo[] CompletedTodos { get; set; } = [];

    public Category[] Categories { get; set; } = [];

    public IReadOnlyDictionary<int, string> CategoryLookup { get; private set; } = new Dictionary<int, string>();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public int? EditingId { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var pendingTask = TodoRepository.GetAsync(false, new());
            var completedTask = TodoRepository.GetAsync(true, new());
            var categoriesTask = CategoryRepository.GetAsync(new());

            await Task.WhenAll(pendingTask, completedTask, categoriesTask);

            PendingTodos = await pendingTask;
            CompletedTodos = await completedTask;
            Categories = await categoriesTask;
            CategoryLookup = Categories.ToDictionary(c => c.Id, c => c.Name);
        }
        catch (Exception ex)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = $"Error fetching todos: {ex.Message}";
            }
        }
    }

    public async Task<IActionResult> OnPostAsync(string action, int? id, string? title)
    {
        try
        {
            // reload current todos so we can find the target item during POST
            // build todo from posted values instead of reloading from DB
            // posted 'title' and 'isCompleted' hidden fields are present on each item form
            bool postedIsCompleted = false;
            if (Request.HasFormContentType && Request.Form.TryGetValue("isCompleted", out var val))
            {
                bool.TryParse(val.FirstOrDefault(), out postedIsCompleted);
            }

            int postedCategoryId = 0;
            if (Request.HasFormContentType && Request.Form.TryGetValue("categoryId", out var categoryValues))
            {
                int.TryParse(categoryValues.FirstOrDefault(), out postedCategoryId);
            }

            var todo = id.HasValue
                ? new Todo
                {
                    Id = id.Value,
                    Title = title ?? Request.Form["title"].FirstOrDefault() ?? string.Empty,
                    IsCompleted = postedIsCompleted,
                    CategoryId = postedCategoryId
                }
                : null;

            var task = action switch
            {
                "Create" => string.IsNullOrWhiteSpace(title)
                    ? throw new ArgumentNullException(nameof(title), "Title cannot be empty.")
                    : postedCategoryId <= 0
                        ? throw new ArgumentOutOfRangeException(nameof(postedCategoryId), "Category is required.")
                        : TodoRepository.AddAsync(new Todo { Title = title!, CategoryId = postedCategoryId }, new()),

                "Edit" => !id.HasValue
                    ? throw new ArgumentNullException(nameof(id))
                    : Task.Run(() => EditingId = id.Value),

                "Update" => (!id.HasValue || string.IsNullOrWhiteSpace(title))
                    ? throw new ArgumentNullException("ID and title are required.")
                    : postedCategoryId <= 0
                        ? throw new ArgumentOutOfRangeException(nameof(postedCategoryId), "Category is required.")
                        : TodoRepository.UpdateAsync(todo! with { Title = title!, CategoryId = postedCategoryId, IsCompleted = todo.IsCompleted }, new()),

                "Toggle" => !id.HasValue
                    ? throw new ArgumentNullException(nameof(id))
                    : TodoRepository.UpdateAsync(todo! with { IsCompleted = !todo.IsCompleted }, new()),

                "Delete" => !id.HasValue
                    ? throw new ArgumentNullException(nameof(id))
                    : TodoRepository.DeleteAsync(todo!, new()),

                "CancelEdit" => Task.Run(() => EditingId = null),

                _ => throw new ArgumentOutOfRangeException(nameof(action), "Invalid action specified.")
            };

            await task;

            if (action is "Update" or "CancelEdit")
            {
                EditingId = null;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }

        return RedirectToPage();
    }
}