using SqlCommander.Models;

namespace SqlCommander.Services;

public interface IQueryExecutionService
{
    Task<QueryResponse> ExecuteQueryAsync(string connectionString, QueryRequest request, CancellationToken cancellationToken = default);
    void CancelCurrentQuery();
}
