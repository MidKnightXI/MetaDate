using System.Text.Json.Serialization;

namespace Metadate.Model.JsonContext;

[JsonSourceGenerationOptions(WriteIndented = true,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(List<ResultModel>))]
internal partial class Context : JsonSerializerContext
{
}
