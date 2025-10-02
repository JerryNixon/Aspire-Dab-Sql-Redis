using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DataApiBuilder.Rest;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web.Pages;

public record Todo
{
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; init; }

    [JsonPropertyName("Title")]
    public string Title { get; init; } = default!;

    [JsonPropertyName("IsCompleted")]
    public bool IsCompleted { get; init; }
}

public class IndexModel : PageModel
{
    public Todo[] Todos { get; set; } = [];

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var allTodos = await TodoRepository.GetDataAsync(new());
            if (allTodos.Length == 0 && string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "No todos found.";
            }
            Todos = allTodos;
        }
        catch (Exception ex)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = $"Error fetching todos: {ex.Message}";
            }
        }
    }

    public async Task<IActionResult> OnPostAsync(string action, int? id, string? title, bool? isCompleted)
    {
        try
        {
            var task = action switch
            {
                "Create" => string.IsNullOrWhiteSpace(title)
                    ? throw new ArgumentNullException(nameof(title), "Title cannot be empty.")
                    : TodoRepository.AddAsync(title, new()),

                "Toggle" => (!id.HasValue || string.IsNullOrWhiteSpace(title) || !isCompleted.HasValue)
                    ? throw new ArgumentNullException("ID, title, and completion status are required for toggle.")
                    : TodoRepository.ToggleCompletedAsync(id.Value, title, isCompleted.Value, new()),

                "Delete" => !id.HasValue
                    ? throw new ArgumentNullException(nameof(id))
                    : TodoRepository.DeleteAsync(id.Value, new()),

                _ => throw new ArgumentOutOfRangeException(nameof(action), "Invalid action specified.")
            };

            await task;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }

        return RedirectToPage();
    }
}

public static class TodoRepository
{
    private const string ApiPathEnvironmentVariableName = "services__dab__http__0";

    private static Uri GetApiPathFromEnvironment()
    {
        var baseUri = Environment.GetEnvironmentVariable(ApiPathEnvironmentVariableName)
            ?? throw new Exception($"Environment variable {ApiPathEnvironmentVariableName} not found");
        return Uri.TryCreate(baseUri + "/api/Todo", UriKind.Absolute, out Uri? uri) ? uri
            : throw new Exception($"Environment variable {ApiPathEnvironmentVariableName} is not a valid URI");
    }

    private static async Task<TableRepository<Todo>> BuildRepositoryAsync()
    {
        var apiPath = GetApiPathFromEnvironment();
        var apiRepository = new TableRepository<Todo>(apiPath);
        return await apiRepository.IsAvailableAsync() ? apiRepository
            : throw new Exception($"Data API Builder service is not available at [{apiPath}].");
    }

    public static async Task ToggleCompletedAsync(int id, string title, bool currentIsCompleted, CancellationToken token)
    {
        var apiRepository = await BuildRepositoryAsync();
        
        // Create todo with toggled status - no database lookup needed!
        var updatedTodo = new Todo { Id = id, Title = title, IsCompleted = !currentIsCompleted };
        
        var updateResult = await apiRepository.PutAsync(updatedTodo, cancellationToken: token);
        if (!updateResult.Success)
            throw new Exception($"Failed to update: {updateResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task AddAsync(string title, CancellationToken token)
    {
        var apiRepository = await BuildRepositoryAsync();
        var newTodo = new Todo { Title = title };
        var addResult = await apiRepository.PostAsync(newTodo, cancellationToken: token);
        if (!addResult.Success)
            throw new Exception($"Failed to add: {addResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task DeleteAsync(int id, CancellationToken token)
    {
        var todoToDelete = new Todo { Id = id };

        var apiRepository = await BuildRepositoryAsync();
        var deleteResult = await apiRepository.DeleteAsync(todoToDelete, cancellationToken: token);
        if (!deleteResult.Success)
            throw new Exception($"Failed to delete: {deleteResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task<Todo[]> GetDataAsync(CancellationToken token)
    {
        var apiRepository = await BuildRepositoryAsync();
        var apiResult = await apiRepository.GetAsync(cancellationToken: token);
        return apiResult.Success ? apiResult.Result
            : throw new Exception($"Failed to get data: {apiResult.Error?.Message}" ?? "Unknown error");
    }
}
