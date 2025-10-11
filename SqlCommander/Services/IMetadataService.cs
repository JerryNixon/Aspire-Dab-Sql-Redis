using SqlCommander.Models;

namespace SqlCommander.Services;

public interface IMetadataService
{
    Task<DatabaseMetadata> GetMetadataAsync(string connectionString);
    Task<ConnectionTestResult> TestConnectionAsync(string connectionString);
}
