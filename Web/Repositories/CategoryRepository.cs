using Microsoft.DataApiBuilder.Rest;
using Microsoft.DataApiBuilder.Rest.Options;

using Web.Models;

namespace Web.Repositories;

public static class CategoryRepository
{
    private static readonly TableRepository<Category> apiRepository;

    static CategoryRepository()
    {
        var apiPath = GetApiPathFromEnvironment();
        var repository = new TableRepository<Category>(apiPath);
        apiRepository = repository.IsAvailableAsync().GetAwaiter().GetResult()
            ? repository
            : throw new Exception($"Data API Builder service is not available at [{apiPath}].");

        static Uri GetApiPathFromEnvironment()
        {
            var varName = "services__dab__http__0";
            var baseUri = Environment.GetEnvironmentVariable(varName)
                ?? throw new Exception($"Environment variable {varName} not found");
            return Uri.TryCreate(baseUri + "/api/Category", UriKind.Absolute, out var uri) ? uri
                : throw new Exception($"Environment variable {varName} is not a valid URI");
        }
    }

    public static async Task<Category[]> GetAsync(CancellationToken token)
    {
        var getResult = await apiRepository.GetAsync(new GetOptions(), cancellationToken: token);
        return getResult.Success ? getResult.Result
            : throw new Exception($"Failed to get data: {getResult.Error?.Message ?? "Unknown error"}");
    }
}