using Microsoft.DataApiBuilder.Rest;
using Microsoft.DataApiBuilder.Rest.Options;

using Web.Models;

namespace Web.Repositories;

public static class TodoRepository
{
    private static readonly TableRepository<Todo> apiRepository;

    static TodoRepository()
    {
        var apiPath = GetApiPathFromEnvironment();
        var apiRepository = new TableRepository<Todo>(apiPath);
        TodoRepository.apiRepository = apiRepository.IsAvailableAsync().GetAwaiter().GetResult() ? apiRepository
            : throw new Exception($"Data API Builder service is not available at [{apiPath}].");

        static Uri GetApiPathFromEnvironment()
        {
            var varName = "services__dab__http__0";
            var baseUri = Environment.GetEnvironmentVariable(varName)
                ?? throw new Exception($"Environment variable {varName} not found");
            return Uri.TryCreate(baseUri + "/api/Todo", UriKind.Absolute, out Uri? uri) ? uri
                : throw new Exception($"Environment variable {varName} is not a valid URI");
        }
    }

    public static async Task UpdateAsync(Todo todo, CancellationToken token)
    {
        var updateResult = await apiRepository.PutAsync(todo, cancellationToken: token);
        if (!updateResult.Success)
            throw new Exception($"Failed to update: {updateResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task AddAsync(Todo todo, CancellationToken token)
    {
        var addResult = await apiRepository.PostAsync(todo, cancellationToken: token);
        if (!addResult.Success)
            throw new Exception($"Failed to add: {addResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task DeleteAsync(Todo todo, CancellationToken token)
    {
        var deleteResult = await apiRepository.DeleteAsync(todo, cancellationToken: token);
        if (!deleteResult.Success)
            throw new Exception($"Failed to delete: {deleteResult.Error?.Message ?? "Unknown error"}");
    }

    public static async Task<Todo[]> GetAsync(bool isCompleted, CancellationToken token)
    {
        var getOptions = new GetOptions { Filter = $"{nameof(Todo.IsCompleted)} eq {isCompleted.ToString().ToLower()}" };
        var getResult = await apiRepository.GetAsync(getOptions, cancellationToken: token);
        return getResult.Success ? getResult.Result
            : throw new Exception($"Failed to get data: {getResult.Error?.Message}" ?? "Unknown error");
    }
}
