using Microsoft.Data.SqlClient;
using SqlCommander.Models;
using System.Data;
using System.Diagnostics;

namespace SqlCommander.Services;

public class QueryExecutionService : IQueryExecutionService
{
    private readonly ILogger<QueryExecutionService> _logger;
    private SqlCommand? _currentCommand;

    public QueryExecutionService(ILogger<QueryExecutionService> logger)
    {
        _logger = logger;
    }

    public void CancelCurrentQuery()
    {
        try
        {
            _currentCommand?.Cancel();
            _logger.LogInformation("Query cancellation requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel query");
        }
    }

    public async Task<QueryResponse> ExecuteQueryAsync(string connectionString, QueryRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var resultSets = new List<ResultSet>();
        var messages = new List<string>();
        var totalRows = 0;
        var wasTruncated = false;
        var resultLimit = request.ResultLimit ?? 100;

        _logger.LogInformation("Executing query with result limit: {Limit}", resultLimit);

        try
        {
            await using var connection = new SqlConnection(connectionString);
            
            // Capture info messages (PRINT, RAISERROR with low severity, etc.)
            connection.InfoMessage += (sender, e) =>
            {
                foreach (SqlError error in e.Errors)
                {
                    messages.Add(error.Message);
                    _logger.LogInformation("SQL Info Message: {Message}", error.Message);
                }
            };
            
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(request.Sql, connection)
            {
                CommandTimeout = 300 // 5 minutes
            };

            _currentCommand = command;

            try
            {
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                do
                {
                    var resultSet = new ResultSet();
                    var columns = new List<string>();

                    // Get column names
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }
                    resultSet = resultSet with { Columns = columns };

                    // Read rows
                    var rows = new List<Dictionary<string, object?>>();
                    var rowCount = 0;

                    while (await reader.ReadAsync(cancellationToken) && rowCount < resultLimit)
                    {
                        var row = new Dictionary<string, object?>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[columns[i]] = value;
                        }
                        rows.Add(row);
                        rowCount++;
                        totalRows++;
                    }

                    // Check if there are more rows
                    if (await reader.ReadAsync(cancellationToken))
                    {
                        wasTruncated = true;
                        // Don't actually read the remaining rows, just note truncation
                    }

                    resultSet = resultSet with { Rows = rows, RowCount = rowCount };
                    resultSets.Add(resultSet);

                } while (await reader.NextResultAsync(cancellationToken));
            }
            finally
            {
                _currentCommand = null;
            }

            stopwatch.Stop();
            _logger.LogInformation("Query completed in {ElapsedMs}ms, returned {TotalRows} rows, {MessageCount} messages", 
                stopwatch.ElapsedMilliseconds, totalRows, messages.Count);

            return new QueryResponse
            {
                Success = true,
                Messages = messages,
                ResultSets = resultSets,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                TotalRowsReturned = totalRows,
                WasTruncated = wasTruncated
            };
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogInformation("Query was cancelled");
            return new QueryResponse
            {
                Success = false,
                ErrorMessage = "Query was cancelled",
                Messages = messages,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Query execution failed");
            return new QueryResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        finally
        {
            _currentCommand = null;
        }
    }
}
