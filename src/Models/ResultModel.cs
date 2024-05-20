using System.Text.Json.Serialization;

namespace Metadate.Model;

public class ResultModel
{
    public required bool Success { get; set; }
    public required string Path { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }
}
