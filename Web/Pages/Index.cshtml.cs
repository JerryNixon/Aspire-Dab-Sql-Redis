using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Web.Models;
using Web.Repositories;

namespace Web.Pages;

public class IndexModel : PageModel
{
    public Todo[] PendingTodos { get; set; } = [];

    public Todo[] CompletedTodos { get; set; } = [];

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

            await Task.WhenAll(pendingTask, completedTask);

            PendingTodos = await pendingTask;
            CompletedTodos = await completedTask;
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

            var todo = id.HasValue
                ? new Todo { Id = id.Value, Title = title ?? Request.Form["title"].FirstOrDefault() ?? string.Empty, IsCompleted = postedIsCompleted }
                : null;

            var task = action switch
            {
                "Create" => string.IsNullOrWhiteSpace(title)
                    ? throw new ArgumentNullException(nameof(title), "Title cannot be empty.")
                    : TodoRepository.AddAsync(new Todo { Title = title! }, new()),

                "Edit" => !id.HasValue
                    ? throw new ArgumentNullException(nameof(id))
                    : Task.Run(() => EditingId = id.Value),

                "Update" => (!id.HasValue || string.IsNullOrWhiteSpace(title))
                    ? throw new ArgumentNullException("ID and title are required.")
                    : TodoRepository.UpdateAsync(todo! with { Title = title!, IsCompleted = todo.IsCompleted }, new()),

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