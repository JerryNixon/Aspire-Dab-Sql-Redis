using Microsoft.AspNetCore.Mvc.RazorPages;

using Web.Models;

namespace Web.Pages;

public class IndexModel : PageModel
{
    public Actor[] Actors { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var repo = new Repositories.ActorRepo();
            Actors = await repo.GetActorsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error fetching actors: {ex.Message}";
        }
    }
}