using System.Text.Json.Serialization;

namespace WhichCam.Model;

public class ResultModel
{
    public required bool Success { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    public required string Filename { get; set; }

    public required DateResult? Detected { get; set; }
}
