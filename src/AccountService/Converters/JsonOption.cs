using System.Text.Json;
using System.Text.Json.Serialization;

namespace AccountService.Converters;

public static class JsonOption
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters =
        {
            new DateTimeOffsetConverter(),
            new JsonStringEnumConverter()
        }
    };
}