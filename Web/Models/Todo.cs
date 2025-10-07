using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web.Models;

public record Todo
{
    [Key]
    [JsonPropertyName("Id")]
    public int Id { get; init; }

    [JsonPropertyName("Title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("IsCompleted")]
    public bool IsCompleted { get; init; }
}
