using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Extensions
{
    public static class Utf8JsonWriterExtensions
    {
        /// <summary>
        /// Writes the object
        /// 
        /// Based on <see cref="JsonKeyValuePairConverter{TKey,TValue}.WriteProperty{T}"/>
        /// </summary>
        public static void WriteObject<T>(this Utf8JsonWriter writer, string propertyName, T value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WritePropertyName(propertyName);
            writer.WriteObject(value, options);
        }

        /// <summary>
        /// Writes the object
        /// 
        /// Based on <see cref="JsonKeyValuePairConverter{TKey,TValue}.WriteProperty{T}"/>
        /// </summary>
        public static void WriteObject<T>(this Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var typeToConvert = typeof(T);

            // Attempt to use existing converter first before re-entering through JsonSerializer.Serialize().
            // The default converter for object does not support writing.
            if (typeToConvert != typeof(object)
                && options?.GetConverter(typeToConvert) is JsonConverter<T> keyConverter)
            {
                keyConverter.Write(writer, value, options);
            }
            else
            {
                JsonSerializer.Serialize<T>(writer, value, options);
            }
        }
    }
}