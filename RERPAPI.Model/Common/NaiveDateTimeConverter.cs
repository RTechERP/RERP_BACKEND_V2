using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RERPAPI.Model.Common
{
    public class NaiveDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var val = reader.GetString();
                if (string.IsNullOrWhiteSpace(val))
                {
                    return null;
                }

                // Parse the face-value directly without converting or shifting time zones
                string cleanVal = val;
                
                int zIndex = val.IndexOf('Z');
                if (zIndex > 0)
                {
                    cleanVal = val.Substring(0, zIndex);
                }
                else
                {
                    int plusIndex = val.IndexOf('+', 10);
                    if (plusIndex > 0)
                    {
                        cleanVal = val.Substring(0, plusIndex);
                    }
                    else
                    {
                        int minusIndex = val.IndexOf('-', 10);
                        if (minusIndex > 0)
                        {
                            cleanVal = val.Substring(0, minusIndex);
                        }
                    }
                }

                string[] formats = {
                    "yyyy-MM-dd'T'HH:mm:ss.fff",
                    "yyyy-MM-dd'T'HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd"
                };

                if (DateTime.TryParseExact(cleanVal, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                if (DateTime.TryParse(val, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fallbackDate))
                {
                    return fallbackDate;
                }
            }

            try
            {
                return reader.GetDateTime();
            }
            catch
            {
                return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
