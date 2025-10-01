using Microsoft.DataApiBuilder.Rest;

using Web.Models;

namespace Web.Repositories
{
    public class ActorRepo
    {
        private readonly TableRepository<Actor> repo;

        public ActorRepo()
        {
            var name = "services__dab__http__0";
            var baseUrl = Environment.GetEnvironmentVariable(name)
                ?? throw new Exception($"Environment variable {name} not found");
            var baseUri = new Uri(baseUrl + "/api/Actor");
            repo = new TableRepository<Actor>(baseUri);
            if (!repo.IsAvailableAsync().GetAwaiter().GetResult())
            {
                throw new Exception("Data API Builder service is not available.");
            }
        }

        public async Task<Actor[]> GetActorsAsync(CancellationToken token = default)
        {
            var result = await repo.GetAsync(cancellationToken: token);
            if (!result.Success)
            {
                throw new Exception(result.Error?.Message ?? "Unknown error");
            }
            return result.Result;
        }
    }
}
