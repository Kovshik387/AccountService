using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AccountService.Converters;

public sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => DateTimeOffset.Parse(reader.GetString()!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
}