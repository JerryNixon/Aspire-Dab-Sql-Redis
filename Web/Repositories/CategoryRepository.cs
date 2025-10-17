using Microsoft.DataApiBuilder.Rest;
using Microsoft.DataApiBuilder.Rest.Options;

using Web.Models;

namespace Web.Repositories;

public static class CategoryRepository
{
    private static readonly TableRepository<Category> apiRepository = DabRepositoryFactory.CreateCategoryRepository();

    public static async Task<Category[]> GetAsync(CancellationToken token)
    {
        var getResult = await apiRepository.GetAsync(new GetOptions(), cancellationToken: token);
        return getResult.Success ? getResult.Result
            : throw new Exception($"Failed to get data: {getResult.Error?.Message ?? "Unknown error"}");
    }
}