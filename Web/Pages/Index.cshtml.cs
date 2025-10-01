using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DataApiBuilder.Rest;

namespace Web.Pages;

public record Actor(int Id, string Name, int BirthYear)
{
    public int Age => DateTime.Now.Year - BirthYear;
}

public class IndexModel : PageModel
{
    public Actor[] Actors { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Actors = await GetDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error fetching actors: {ex.Message}";
        }
    }

    private static async Task<Actor[]> GetDataAsync()
    {
        var environmentVariableName = "services__dab__http__0";
        var environmentVariableValue = Environment.GetEnvironmentVariable(environmentVariableName)
            ?? throw new Exception($"Environment variable {environmentVariableName} not found");

        var baseUri = new Uri(environmentVariableValue + "/api/Actor");
        var apiRepository = new TableRepository<Actor>(baseUri);

        if (!await apiRepository.IsAvailableAsync())
        {
            throw new Exception("Data API Builder service is not available.");
        }

        var apiResult = await apiRepository.GetAsync();
        if (!apiResult.Success)
        {
            throw new Exception(apiResult.Error?.Message ?? "Unknown error");
        }

        return apiResult.Result;
    }
}
