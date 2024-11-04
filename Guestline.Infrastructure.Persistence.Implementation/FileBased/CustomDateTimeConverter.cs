using System.Text.Json;
using System.Text.Json.Serialization;

namespace Guestline.Infrastructure.Persistence.Implementation.FileBased;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyyMMdd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeString = reader.GetString();

        if (DateTime.TryParseExact(dateTimeString, _format, null, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTime))
        {
            return DateTime.SpecifyKind(dateTime.Date, DateTimeKind.Utc);
        }
        throw new JsonException($"DateTime format should be {_format}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}