using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace System.Text.Json.Extensions.Converters
{
    public class KeyValuePairCollectionConverter<TValue, TCollection>
        : JsonConverter<TCollection>
        where TCollection : ICollection<KeyValuePair<string, TValue>>, new()
    {
        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw reader.GetException($"Expected StartObject but found {reader.TokenType}");
            }

            var collection = new TCollection();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return collection;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw reader.GetException($"Expected PropertyName but found {reader.TokenType}");
                }

                var propertyName = reader.GetString();
                var propertyValue = reader.ReadObject<TValue>(options);
                collection.Add(new KeyValuePair<string, TValue>(propertyName, propertyValue));
            }

            throw reader.GetException($"Expected EndObject but JSON ended");
        }

        public override void Write(Utf8JsonWriter writer, TCollection collection, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (collection == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            foreach (var (key, value) in collection)
            {
                writer.WriteObject(key, value, options);
            }

            writer.WriteEndObject();
        }
    }
}