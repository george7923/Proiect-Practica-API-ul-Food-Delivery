using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRACTICA_OFICIAL
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParseExact(reader.GetString(), _format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                return DateTime.Parse(reader.GetString());
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
