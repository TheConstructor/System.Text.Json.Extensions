using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Extensions
{
    public static class Utf8JsonReaderExtensions
    {
        /// <summary>
        /// Tries to read the next token as object of type <typeparamref name="T"/>.
        /// 
        /// Based on <see cref="JsonKeyValuePairConverter{TKey,TValue}.ReadProperty{T}"/>
        /// </summary>
        public static T ReadObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            do
            {
                reader.Read();
            } while (reader.TokenType == JsonTokenType.Comment
                     && options.ReadCommentHandling == JsonCommentHandling.Skip);

            return GetObject<T>(ref reader, options);
        }

        /// <summary>
        /// Tries to read the current token as object of type <typeparamref name="T"/>.
        /// 
        /// Based on <see cref="JsonKeyValuePairConverter{TKey,TValue}.ReadProperty{T}"/>
        /// </summary>
        public static T GetObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var typeToConvert = typeof(T);

            // Attempt to use existing converter first before re-entering through JsonSerializer.Deserialize().
            // The default converter for objects does not parse null objects as null, so it is not used here.
            if (typeToConvert != typeof(object)
                && options?.GetConverter(typeToConvert) is JsonConverter<T> keyConverter)
            {
                return keyConverter.Read(ref reader, typeToConvert, options);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(ref reader, options);
            }
        }

        public static T Throw<T>(this ref Utf8JsonReader reader, string message)
        {
            throw GetException(ref reader, message);
        }

        public static void Throw(this ref Utf8JsonReader reader, string message)
        {
            throw GetException(ref reader, message);
        }

        public static JsonException GetException(this ref Utf8JsonReader reader, string message)
        {
            return new JsonException(message, null, null, reader.BytesConsumed);
        }
    }
}