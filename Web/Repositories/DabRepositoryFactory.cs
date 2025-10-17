using Microsoft.DataApiBuilder.Rest;

using Web.Models;

namespace Web.Repositories;

public static class DabRepositoryFactory
{
    private const string ServiceVar = "services__dab__http__0";

    private static readonly Lazy<string> BaseAddress = new(() =>
    {
        var baseUri = Environment.GetEnvironmentVariable(ServiceVar)
            ?? throw new Exception($"Environment variable {ServiceVar} not found");
        return baseUri.EndsWith('/') ? baseUri : baseUri + "/";
    });

    private static Uri BuildEntityUri(string entityName)
    {
        var full = BaseAddress.Value + "api/" + entityName;
        return Uri.TryCreate(full, UriKind.Absolute, out var uri)
            ? uri
            : throw new Exception($"Environment variable {ServiceVar} is not a valid URI");
    }

    public static TableRepository<Todo> CreateTodoRepository() => CreateRepository<Todo>("Todo");

    public static TableRepository<Category> CreateCategoryRepository() => CreateRepository<Category>("Category");

    private static TableRepository<T> CreateRepository<T>(string entityName) where T : class
    {
        var entityUri = BuildEntityUri(entityName);
        var repo = new TableRepository<T>(entityUri);
        return repo.IsAvailableAsync().GetAwaiter().GetResult() ? repo
            : throw new Exception($"Data API Builder service is not available at [{entityUri}].");
    }
}
