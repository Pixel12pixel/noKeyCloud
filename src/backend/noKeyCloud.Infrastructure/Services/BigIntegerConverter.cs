using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace noKeyCloud.Infrastructure.Services;

public class BigIntegerConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return BigInteger.Parse(reader.GetString()!);
            case JsonTokenType.Number:
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return BigInteger.Parse(doc.RootElement.GetRawText());
            }
            default:
                throw new JsonException($"Unexpected token when parsing BigInteger. Expected String or Number, got {reader.TokenType}.");
        }
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}